using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Security.RightsManagement.Protectors;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetWacAttachmentInfo : ServiceCommand<WacAttachmentType>
	{
		public GetWacAttachmentInfo(CallContext callContext, string attachmentId, bool isEdit, string draftId) : base(callContext)
		{
			this.attachmentId = attachmentId;
			this.isEdit = isEdit;
			this.draftId = draftId;
			OwsLogRegistry.Register("GetWacAttachmentInfo", typeof(GetWacAttachmentInfoMetadata), new Type[0]);
		}

		protected override WacAttachmentType InternalExecute()
		{
			if (string.IsNullOrEmpty(this.attachmentId))
			{
				throw new OwaInvalidRequestException("You must provide an attachmentId when calling GetWacAttachmentInfo.");
			}
			WacAttachmentType result;
			using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(this.attachmentId, base.CallContext))
			{
				result = GetWacAttachmentInfo.Execute(base.CallContext, attachmentRetriever.RootItem.Session, attachmentRetriever.RootItem, attachmentRetriever.Attachment, this.draftId, this.attachmentId, this.isEdit);
			}
			return result;
		}

		public static WacAttachmentType Execute(CallContext callContext, IStoreSession originalAttachmentSession, IItem originalAttachmentItem, IAttachment originalAttachment, string draftId, string ewsAttachmentId, bool isEdit)
		{
			MdbCache.GetInstance().BeginAsyncUpdate();
			UserContext userContext = UserContextManager.GetUserContext(callContext.HttpContext, callContext.EffectiveCaller, true);
			if (userContext == null)
			{
				throw new OwaInvalidRequestException("Unable to determine user context.");
			}
			if (!userContext.IsWacEditingEnabled)
			{
				isEdit = false;
			}
			ConfigurationContext configurationContext = new ConfigurationContext(userContext);
			AttachmentPolicy attachmentPolicy = configurationContext.AttachmentPolicy;
			bool isPublicLogon = userContext.IsPublicLogon;
			if (!attachmentPolicy.GetWacViewingEnabled(isPublicLogon))
			{
				throw new OwaOperationNotSupportedException("WAC viewing not enabled for the current user");
			}
			MailboxSession mailboxSession = null;
			StoreObjectId draftObjectId = null;
			if (draftId != null)
			{
				IdAndSession idAndSession = GetWacAttachmentInfo.GetIdAndSession(callContext, draftId, false);
				mailboxSession = (idAndSession.Session as MailboxSession);
				draftObjectId = StoreId.EwsIdToStoreObjectId(draftId);
				if (mailboxSession == null)
				{
					throw new OwaOperationNotSupportedException("We need a MailboxSession to create the draft, but this a " + idAndSession.Session.GetType().Name);
				}
			}
			string text = originalAttachmentSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			string primarySmtpAddress = callContext.EffectiveCaller.PrimarySmtpAddress;
			RequestDetailsLogger protocolLog = callContext.ProtocolLog;
			protocolLog.Set(GetWacAttachmentInfoMetadata.LogonSmtpAddress, primarySmtpAddress);
			protocolLog.Set(GetWacAttachmentInfoMetadata.MailboxSmtpAddress, text);
			protocolLog.Set(GetWacAttachmentInfoMetadata.Edit, isEdit);
			protocolLog.Set(GetWacAttachmentInfoMetadata.Extension, originalAttachment.FileExtension);
			protocolLog.Set(GetWacAttachmentInfoMetadata.DraftProvided, draftId != null);
			string displayName = callContext.AccessingPrincipal.MailboxInfo.DisplayName;
			XSOFactory factory = new XSOFactory();
			AttachmentDataProvider defaultUploadDataProvider = new AttachmentDataProviderManager().GetDefaultUploadDataProvider(callContext);
			IReferenceAttachment referenceAttachment = originalAttachment as IReferenceAttachment;
			if (referenceAttachment != null)
			{
				GetWacAttachmentInfo.LogReferenceAttachmentProperties(protocolLog, referenceAttachment.ProviderEndpointUrl, GetWacAttachmentInfoMetadata.OriginalReferenceAttachmentServiceUrl, referenceAttachment.AttachLongPathName, GetWacAttachmentInfoMetadata.OriginalReferenceAttachmentUrl);
			}
			if (defaultUploadDataProvider != null)
			{
				protocolLog.Set(GetWacAttachmentInfoMetadata.AttachmentDataProvider, defaultUploadDataProvider.ToString());
			}
			WacAttachmentType wacAttachmentType;
			try
			{
				using (GetWacAttachmentInfo.Implementation implementation = new GetWacAttachmentInfo.Implementation(defaultUploadDataProvider, factory, originalAttachmentSession, originalAttachmentSession.MailboxOwner.ModernGroupType, originalAttachmentItem, originalAttachment, ewsAttachmentId, mailboxSession, draftObjectId, isEdit, displayName, (IStoreSession session, StoreId itemId, AttachmentId attachmentId) => new IdAndSession(itemId, (StoreSession)session)
				{
					AttachmentIds = 
					{
						attachmentId
					}
				}.GetConcatenatedId().Id))
				{
					implementation.Execute();
					protocolLog.Set(GetWacAttachmentInfoMetadata.OriginalAttachmentType, implementation.OriginalAttachmentType);
					protocolLog.Set(GetWacAttachmentInfoMetadata.ResultAttachmentType, implementation.ResultAttachmentType);
					protocolLog.Set(GetWacAttachmentInfoMetadata.ResultAttachmentCreation, implementation.ResultAttachmentCreation);
					if (implementation.ResultAttachmentType == AttachmentType.Reference)
					{
						IMailboxInfo mailboxInfo = originalAttachmentSession.MailboxOwner.MailboxInfo;
						string mailboxAddress = mailboxInfo.PrimarySmtpAddress.ToString();
						StoreId id = originalAttachmentItem.Id;
						BaseItemId itemIdFromStoreId = IdConverter.GetItemIdFromStoreId(id, new MailboxId(mailboxInfo.MailboxGuid));
						string exchangeSessionId = WacUtilities.GetExchangeSessionId(default(Guid).ToString());
						protocolLog.Set(GetWacAttachmentInfoMetadata.ExchangeSessionId, exchangeSessionId);
						wacAttachmentType = GetWacAttachmentInfo.GetResultForReferenceAttachment(callContext, userContext, implementation, defaultUploadDataProvider, mailboxAddress, itemIdFromStoreId, originalAttachment.FileName, isEdit, displayName, exchangeSessionId, protocolLog);
					}
					else
					{
						if (implementation.ResultAttachmentType != AttachmentType.Stream)
						{
							throw new OwaNotSupportedException("Unsupported attachment type " + implementation.ResultAttachmentType);
						}
						AttachmentIdType ewsAttachmentIdType = GetWacAttachmentInfo.GetEwsAttachmentIdType(callContext, implementation.ResultItemId, implementation.ResultAttachmentId);
						wacAttachmentType = GetWacAttachmentInfo.GetResultForStreamAttachment(callContext, userContext, configurationContext, attachmentPolicy, isPublicLogon, Thread.CurrentThread.CurrentCulture.Name, isEdit, (IStreamAttachment)implementation.ResultAttachment, implementation.ResultAttachmentExtension, ewsAttachmentIdType, implementation.ResultIsInDraft, implementation.ResultStoreSession, text, originalAttachmentSession.MailboxOwner.MailboxInfo.IsArchive);
					}
				}
			}
			catch (ServerException exception)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception, WacAttachmentStatus.UploadFailed);
			}
			catch (GetWacAttachmentInfo.AttachmentUploadException exception2)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception2, WacAttachmentStatus.UploadFailed);
			}
			catch (RightsManagementPermanentException exception3)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception3, WacAttachmentStatus.ProtectedByUnsupportedIrm);
			}
			catch (AttachmentProtectionException exception4)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception4, WacAttachmentStatus.ProtectedByUnsupportedIrm);
			}
			catch (ObjectNotFoundException exception5)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception5, WacAttachmentStatus.NotFound);
			}
			catch (OwaInvalidRequestException exception6)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception6, WacAttachmentStatus.InvalidRequest);
			}
			catch (WacDiscoveryFailureException exception7)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception7, WacAttachmentStatus.WacDiscoveryFailed);
			}
			catch (WebException exception8)
			{
				wacAttachmentType = GetWacAttachmentInfo.HandleException(protocolLog, isEdit, exception8, WacAttachmentStatus.AttachmentDataProviderError);
			}
			if (wacAttachmentType == null)
			{
				throw new OwaInvalidOperationException("There is no reason known for code to reach here without throwing an unhandled exception elsewhere");
			}
			protocolLog.Set(GetWacAttachmentInfoMetadata.Status, wacAttachmentType.Status.ToString());
			return wacAttachmentType;
		}

		private static WacAttachmentType HandleException(RequestDetailsLogger logger, bool isEdit, Exception exception, WacAttachmentStatus status)
		{
			logger.Set(GetWacAttachmentInfoMetadata.HandledException, exception);
			return new WacAttachmentType
			{
				AttachmentId = null,
				IsEdit = isEdit,
				IsInDraft = false,
				WacUrl = string.Empty,
				Status = status
			};
		}

		private static void LogReferenceAttachmentProperties(RequestDetailsLogger logger, string webServiceUrl, Enum webServiceUrlKey, string contentUrl, Enum contentUrlKey)
		{
			if (string.IsNullOrEmpty(webServiceUrl))
			{
				webServiceUrl = "(none)";
			}
			logger.Set(webServiceUrlKey, webServiceUrl);
			if (string.IsNullOrEmpty(contentUrl))
			{
				contentUrl = "(none)";
			}
			logger.Set(contentUrlKey, contentUrl);
		}

		private static void PostUploadMessage(string groupAddress, string userAddress, string userDisplayName, BaseItemId referenceItemId, string fileName, string contentUrl, string providerType, string endpointUrl, string sessionId)
		{
			BodyContentType bodyContentType = new BodyContentType();
			bodyContentType.Value = string.Format(Strings.ModernGroupAttachmentUploadNoticeBody, fileName, userDisplayName);
			ReferenceAttachmentType referenceAttachmentType = new ReferenceAttachmentType();
			referenceAttachmentType.AttachLongPathName = contentUrl;
			referenceAttachmentType.ProviderEndpointUrl = endpointUrl;
			referenceAttachmentType.ProviderType = providerType;
			referenceAttachmentType.Name = fileName;
			ReplyToItemType replyToItemType = new ReplyToItemType();
			replyToItemType.NewBodyContent = bodyContentType;
			replyToItemType.Attachments = new AttachmentType[1];
			replyToItemType.Attachments[0] = referenceAttachmentType;
			replyToItemType.ReferenceItemId = referenceItemId;
			PostModernGroupItemJsonRequest postModernGroupItemJsonRequest = new PostModernGroupItemJsonRequest();
			postModernGroupItemJsonRequest.Body = new PostModernGroupItemRequest();
			postModernGroupItemJsonRequest.Body.Items = new NonEmptyArrayOfAllItemsType();
			postModernGroupItemJsonRequest.Body.Items.Add(replyToItemType);
			postModernGroupItemJsonRequest.Body.ModernGroupEmailAddress = new EmailAddressWrapper();
			postModernGroupItemJsonRequest.Body.ModernGroupEmailAddress.EmailAddress = groupAddress;
			postModernGroupItemJsonRequest.Body.ModernGroupEmailAddress.MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString();
			OWAService owaservice = new OWAService();
			GetWacAttachmentInfo.PostUploadMessageAsyncState postUploadMessageAsyncState = new GetWacAttachmentInfo.PostUploadMessageAsyncState();
			postUploadMessageAsyncState.MailboxSmtpAddress = groupAddress;
			postUploadMessageAsyncState.LogonSmtpAddress = userAddress;
			postUploadMessageAsyncState.OwaService = owaservice;
			postUploadMessageAsyncState.SessionId = sessionId;
			IAsyncResult asyncResult = owaservice.BeginPostModernGroupItem(postModernGroupItemJsonRequest, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			PostModernGroupItemResponse body = owaservice.EndPostModernGroupItem(asyncResult).Body;
		}

		private static IdAndSession GetIdAndSession(CallContext callContext, string id, bool isAttachmentId)
		{
			IdAndSession idAndSession;
			try
			{
				if (isAttachmentId)
				{
					idAndSession = new IdConverter(callContext).ConvertAttachmentIdToIdAndSessionReadOnly(id);
				}
				else
				{
					idAndSession = new IdConverter(callContext).ConvertItemIdToIdAndSessionReadOnly(id);
				}
				if (idAndSession == null)
				{
					List<AttachmentId> attachmentIds = new List<AttachmentId>();
					IdHeaderInformation idHeaderInformation = IdConverter.ConvertFromConcatenatedId(id, BasicTypes.Attachment, attachmentIds, false);
					throw new OwaInvalidRequestException("Unsupported Attachment ID. Storage type: " + idHeaderInformation.IdStorageType);
				}
			}
			catch (InvalidStoreIdException)
			{
				throw new OwaInvalidRequestException("Invalid attachment ID: " + id);
			}
			catch (AccessDeniedException)
			{
				throw new OwaInvalidRequestException("You do not have permission to access this mailbox.");
			}
			if (idAndSession.Session == null)
			{
				throw new OwaInvalidRequestException("The mailbox was not specified.");
			}
			return idAndSession;
		}

		internal static AttachmentIdType GetEwsAttachmentIdType(CallContext callContext, VersionedId itemId, string ewsAttachmentId)
		{
			IdAndSession idAndSession = GetWacAttachmentInfo.GetIdAndSession(callContext, ewsAttachmentId, true);
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(itemId, idAndSession, null);
			return new AttachmentIdType
			{
				Id = ewsAttachmentId,
				RootItemId = concatenatedId.Id
			};
		}

		private static WacAttachmentType GetResultForReferenceAttachment(CallContext callContext, UserContext userContext, GetWacAttachmentInfo.Implementation implementation, AttachmentDataProvider provider, string mailboxAddress, BaseItemId referenceItemId, string fileName, bool isEdit, string userDisplayName, string sessionId, RequestDetailsLogger logger)
		{
			GetWacAttachmentInfo.LogReferenceAttachmentProperties(logger, implementation.ResultAttachmentWebServiceUrl, GetWacAttachmentInfoMetadata.ResultReferenceAttachmentServiceUrl, implementation.ResultAttachmentContentUrl, GetWacAttachmentInfoMetadata.ResultReferenceAttachmentUrl);
			AttachmentIdType ewsAttachmentIdType = GetWacAttachmentInfo.GetEwsAttachmentIdType(callContext, implementation.ResultItemId, implementation.ResultAttachmentId);
			WacAttachmentType result = GetWacAttachmentInfo.CreateWacAttachmentType(userContext.LogonIdentity, ewsAttachmentIdType, implementation.ResultAttachmentWebServiceUrl, implementation.ResultAttachmentContentUrl, isEdit, implementation.ResultIsInDraft);
			if (implementation.ResultAttachmentCreation == WacAttachmentCreationType.Upload)
			{
				try
				{
					GetWacAttachmentInfo.PostUploadMessage(mailboxAddress, userContext.LogonIdentity.PrimarySmtpAddress.ToString(), userDisplayName, referenceItemId, fileName, implementation.ResultAttachmentContentUrl, implementation.ResultAttachmentProviderType, implementation.ResultAttachmentWebServiceUrl, sessionId);
				}
				catch (Exception value)
				{
					logger.Set(GetWacAttachmentInfoMetadata.HandledException, value);
				}
			}
			return result;
		}

		private static WacAttachmentType CreateWacAttachmentType(OwaIdentity identity, AttachmentIdType attachmentIdType, string webServiceUrl, string contentUrl, bool isEdit, bool isInDraft)
		{
			string wacUrl;
			try
			{
				wacUrl = OneDriveProUtilities.GetWacUrl(identity, webServiceUrl, contentUrl, isEdit);
			}
			catch (Exception ex)
			{
				ex.ToString();
				throw;
			}
			return new WacAttachmentType
			{
				AttachmentId = attachmentIdType,
				IsEdit = isEdit,
				IsInDraft = isInDraft,
				WacUrl = wacUrl,
				Status = WacAttachmentStatus.Success
			};
		}

		private static WacAttachmentType GetResultForStreamAttachment(CallContext callContext, UserContext userContext, ConfigurationContext configurationContext, AttachmentPolicy attachmentPolicy, bool isPublicLogon, string cultureName, bool isEdit, IStreamAttachment attachment, string attachmentExtension, AttachmentIdType attachmentIdType, bool isInDraft, IStoreSession storeSession, string mailboxSmtpAddress, bool isArchive)
		{
			WacFileRep wacFileRep = GetWacAttachmentInfo.CreateWacFileRep(callContext, configurationContext, attachmentPolicy, isPublicLogon, isEdit, isArchive);
			HttpRequest request = callContext.HttpContext.Request;
			string text;
			string arg;
			GetWacAttachmentInfo.GenerateWopiSrcUrl(request, wacFileRep, mailboxSmtpAddress, out text, out arg);
			if (text == null)
			{
				throw new OwaInvalidOperationException("WOPI URL is null.");
			}
			string id = attachmentIdType.Id;
			TokenResult oauthToken = GetWacAttachmentInfo.GetOAuthToken(id, userContext, mailboxSmtpAddress, text);
			string exchangeSessionId = WacUtilities.GetExchangeSessionId(oauthToken.TokenString);
			callContext.ProtocolLog.Set(GetWacAttachmentInfoMetadata.ExchangeSessionId, exchangeSessionId);
			SecurityIdentifier effectiveCallerSid = callContext.EffectiveCallerSid;
			CachedAttachmentInfo.GetInstance(mailboxSmtpAddress, id, exchangeSessionId, effectiveCallerSid, cultureName);
			string wacUrl = GetWacAttachmentInfo.GetWacUrl(isEdit, cultureName, attachmentExtension);
			if (string.IsNullOrEmpty(wacUrl))
			{
				throw new OwaInvalidRequestException(string.Format("Wac Base Url is null for this given extension {0} and culture {1}", attachmentExtension, cultureName));
			}
			new Uri(wacUrl);
			string format = "{0}WOPISrc={1}&access_token={2}";
			string arg2 = HttpUtility.UrlEncode(oauthToken.TokenString);
			string text2 = string.Format(format, wacUrl, HttpUtility.UrlEncode(text), arg2);
			string value = string.Format(format, wacUrl, arg, arg2);
			callContext.ProtocolLog.Set(GetWacAttachmentInfoMetadata.WacUrl, value);
			if (!Uri.IsWellFormedUriString(text2, UriKind.Absolute))
			{
				throw new OwaInvalidOperationException("The WAC Iframe URL that was generated is not a well formed URI: " + text2);
			}
			return new WacAttachmentType
			{
				AttachmentId = attachmentIdType,
				IsEdit = isEdit,
				IsInDraft = isInDraft,
				WacUrl = text2,
				Status = WacAttachmentStatus.Success
			};
		}

		private static WacFileRep CreateWacFileRep(CallContext callContext, ConfigurationContext configurationContext, AttachmentPolicy attachmentPolicy, bool isPublicLogon, bool isEdit, bool isArchive)
		{
			bool directFileAccessEnabled = attachmentPolicy.GetDirectFileAccessEnabled(isPublicLogon);
			bool externalServicesEnabled = configurationContext.IsFeatureEnabled(Feature.WacExternalServicesEnabled);
			bool wacOMEXEnabled = configurationContext.IsFeatureEnabled(Feature.WacOMEXEnabled);
			return new WacFileRep(callContext.EffectiveCallerSid, directFileAccessEnabled, externalServicesEnabled, wacOMEXEnabled, isEdit, isArchive);
		}

		private static TokenResult GetOAuthToken(string ewsAttachmentId, UserContext userContext, string mailboxSmtpAddress, string wopiSrcUrl)
		{
			LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			TokenResult wacCallbackToken = localTokenIssuer.GetWacCallbackToken(new Uri(wopiSrcUrl, UriKind.Absolute), mailboxSmtpAddress, ewsAttachmentId);
			if (wacCallbackToken == null)
			{
				throw new InvalidOperationException("OAuth TokenResult is null.");
			}
			return wacCallbackToken;
		}

		private static void GenerateWopiSrcUrl(HttpRequest request, WacFileRep wacFileRep, string emailAddress, out string wopiSrcUrl, out string wopiSrcUrlForLogging)
		{
			string text = string.Format("owa/{0}/wopi/files/@/owaatt", HttpUtility.UrlEncode(emailAddress));
			string text2 = string.Format("owa/{0}/wopi/files/@/owaatt", ExtensibleLogger.FormatPIIValue(emailAddress));
			Uri requestUrlEvenIfProxied = request.GetRequestUrlEvenIfProxied();
			wopiSrcUrl = string.Format("{0}://{1}:{2}/{3}?{4}={5}", new object[]
			{
				requestUrlEvenIfProxied.Scheme,
				requestUrlEvenIfProxied.Host,
				requestUrlEvenIfProxied.Port,
				text,
				"owaatt",
				HttpUtility.UrlEncode(wacFileRep.Serialize())
			});
			wopiSrcUrlForLogging = string.Format("{0}://{1}:{2}/{3}?{4}={5}", new object[]
			{
				requestUrlEvenIfProxied.Scheme,
				requestUrlEvenIfProxied.Host,
				requestUrlEvenIfProxied.Port,
				text2,
				"owaatt",
				HttpUtility.UrlEncode(wacFileRep.Serialize())
			});
			if (Globals.IsPreCheckinApp && request.Cookies["X-DFPOWA-Vdir"] != null)
			{
				wopiSrcUrl = string.Format("{0}&vdir={1}", wopiSrcUrl, request.Cookies["X-DFPOWA-Vdir"].Value);
			}
		}

		private static string GetWacUrl(bool isEdit, string cultureName, string fileExtension)
		{
			string result = null;
			if (WacDiscoveryManager.Instance.WacDiscoveryResult != null)
			{
				if (isEdit)
				{
					WacDiscoveryManager.Instance.WacDiscoveryResult.TryGetEditUrlForFileExtension(fileExtension, cultureName, out result);
				}
				else
				{
					WacDiscoveryManager.Instance.WacDiscoveryResult.TryGetViewUrlForFileExtension(fileExtension, cultureName, out result);
				}
				return result;
			}
			throw new OwaInvalidOperationException("WacDiscoveryResult cannot be null, if it is, we have a bug in our code");
		}

		private const string AttachmentTypeReference = "Reference";

		private const string AttachmentTypeStream = "Stream";

		private const string ActionName = "GetWacAttachmentInfo";

		private const string DFPOWAVdirCookie = "X-DFPOWA-Vdir";

		private readonly string attachmentId;

		private readonly bool isEdit;

		private readonly string draftId;

		public class Implementation : DisposeTrackableBase
		{
			public IStoreSession ResultStoreSession { get; private set; }

			public IItem ResultItem { get; private set; }

			public VersionedId ResultItemId { get; private set; }

			public IAttachment ResultAttachment { get; private set; }

			public WacAttachmentCreationType ResultAttachmentCreation { get; private set; }

			public string ResultAttachmentId { get; private set; }

			public string ResultAttachmentContentUrl { get; private set; }

			public string ResultAttachmentWebServiceUrl { get; private set; }

			public string ResultAttachmentProviderType { get; private set; }

			public AttachmentType OriginalAttachmentType { get; private set; }

			public AttachmentType ResultAttachmentType { get; private set; }

			public bool ResultIsInDraft { get; private set; }

			public string ResultAttachmentExtension { get; private set; }

			public Implementation(AttachmentDataProvider provider, IXSOFactory factory, IStoreSession attachmentSession, ModernGroupObjectType attachmentSessionModernGroupType, IItem rootItem, IAttachment attachment, string attachmentId, IMailboxSession draftAttachmentSession, StoreObjectId draftObjectId, bool isEdit, string userDisplayName, Func<IStoreSession, StoreId, AttachmentId, string> idConverter)
			{
				this.provider = provider;
				this.factory = factory;
				this.originalAttachmentSession = attachmentSession;
				this.originalSessionModernGroupType = attachmentSessionModernGroupType;
				this.originalAttachmentRootItem = rootItem;
				this.originalAttachment = attachment;
				this.originalAttachmentId = attachmentId;
				this.draftAttachmentSession = draftAttachmentSession;
				this.draftObjectId = draftObjectId;
				this.isEdit = isEdit;
				this.userDisplayName = userDisplayName;
				this.idConverter = idConverter;
			}

			protected override void InternalDispose(bool isDisposing)
			{
				if (isDisposing)
				{
					if (this.ResultAttachment != null)
					{
						this.ResultAttachment.Dispose();
						this.ResultAttachment = null;
					}
					if (this.draftItem != null)
					{
						this.draftItem.Dispose();
						this.draftItem = null;
					}
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<GetWacAttachmentInfo.Implementation>(this);
			}

			public void Execute()
			{
				if (this.originalAttachment is IReferenceAttachment)
				{
					this.OriginalAttachmentType = AttachmentType.Reference;
				}
				else
				{
					if (!(this.originalAttachment is IStreamAttachment))
					{
						throw new OwaOperationNotSupportedException("Only StreamAttachment and ReferenceAttachment are supported.");
					}
					this.OriginalAttachmentType = AttachmentType.Stream;
				}
				if (this.isEdit)
				{
					if (this.draftObjectId != null)
					{
						this.draftItem = this.factory.BindToItem(this.draftAttachmentSession, this.draftObjectId, new PropertyDefinition[0]);
						this.ThrowIfNotEditable(this.draftItem);
						this.draftItem.OpenAsReadWrite();
						this.ResultStoreSession = this.draftAttachmentSession;
						this.ResultItem = this.draftItem;
						this.ResultItemId = this.ResultItem.Id;
						IAttachment resultAttachment;
						AttachmentId arg;
						if (this.ShouldUploadAttachment())
						{
							this.ResultAttachmentType = AttachmentType.Reference;
							this.ResultAttachmentCreation = WacAttachmentCreationType.Upload;
							this.UploadAttachment(this.originalAttachment, this.draftItem, out resultAttachment, out arg);
						}
						else
						{
							this.ResultAttachmentType = this.OriginalAttachmentType;
							this.ResultAttachmentCreation = WacAttachmentCreationType.Copy;
							this.CopyAttachment(this.originalAttachment, this.draftItem, out resultAttachment, out arg);
						}
						this.ResultAttachment = resultAttachment;
						this.ResultAttachmentId = this.idConverter(this.ResultStoreSession, this.ResultItemId, arg);
					}
					else
					{
						this.ThrowIfNotEditable(this.originalAttachmentRootItem);
						this.SetResultPropertiesFromOriginalAttachment();
					}
					this.ResultIsInDraft = true;
					return;
				}
				this.SetResultPropertiesFromOriginalAttachment();
				this.ResultIsInDraft = this.IsEditable(this.ResultItem);
			}

			private void SetResultPropertiesFromOriginalAttachment()
			{
				this.ResultAttachmentType = this.OriginalAttachmentType;
				this.ResultAttachmentCreation = WacAttachmentCreationType.UseExisting;
				this.ResultStoreSession = this.originalAttachmentSession;
				this.ResultItem = this.originalAttachmentRootItem;
				this.ResultItemId = this.ResultItem.Id;
				this.ResultAttachment = this.originalAttachment;
				this.ResultAttachmentId = this.originalAttachmentId;
				this.ResultAttachmentExtension = this.originalAttachment.FileExtension;
				IReferenceAttachment referenceAttachment = this.originalAttachment as IReferenceAttachment;
				if (referenceAttachment != null)
				{
					this.ResultAttachmentContentUrl = referenceAttachment.AttachLongPathName;
					this.ResultAttachmentWebServiceUrl = referenceAttachment.ProviderEndpointUrl;
					this.ResultAttachmentProviderType = referenceAttachment.ProviderType;
				}
			}

			private void ThrowIfNotEditable(IItem item)
			{
				if (!this.IsEditable(item))
				{
					throw new OwaInvalidOperationException("The given item should be a draft or task. It is a " + item.GetType().Name);
				}
			}

			private bool IsEditable(IItem item)
			{
				if (item is Task)
				{
					return true;
				}
				IMessageItem messageItem = item as IMessageItem;
				return messageItem != null && messageItem.IsDraft;
			}

			private bool ShouldUploadAttachment()
			{
				bool flag = this.originalSessionModernGroupType != ModernGroupObjectType.None;
				bool flag2 = this.OriginalAttachmentType == AttachmentType.Stream;
				return flag2 && this.isEdit && flag;
			}

			private void UploadAttachment(IAttachment attachment, IItem draftItem, out IAttachment newAttachment, out AttachmentId newAttachmentId)
			{
				IStreamAttachment streamAttachment = attachment as IStreamAttachment;
				if (streamAttachment == null)
				{
					throw new InvalidOperationException("UploadAttachment requires a stream attachment, but was given a " + attachment.GetType().Name);
				}
				UploadItemAsyncResult uploadItemAsyncResult;
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					byte[] array = new byte[contentStream.Length];
					while (contentStream.Position != contentStream.Length)
					{
						int count = (int)Math.Min(4000L, contentStream.Length - contentStream.Position);
						contentStream.Read(array, (int)contentStream.Position, count);
					}
					CancellationToken cancellationToken = default(CancellationToken);
					uploadItemAsyncResult = this.provider.UploadItemSync(array, attachment.FileName, cancellationToken);
				}
				if (uploadItemAsyncResult.ResultCode != AttachmentResultCode.Success)
				{
					throw new GetWacAttachmentInfo.AttachmentUploadException(uploadItemAsyncResult.ResultCode.ToString());
				}
				UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
				this.ResultAttachmentWebServiceUrl = uploadItemAsyncResult.Item.ProviderEndpointUrl;
				this.ResultAttachmentProviderType = uploadItemAsyncResult.Item.ProviderType.ToString();
				this.ResultAttachmentContentUrl = this.provider.GetItemAbsoulteUrl(userContext, uploadItemAsyncResult.Item.Location, uploadItemAsyncResult.Item.ProviderEndpointUrl, null, null);
				this.ResultAttachmentExtension = attachment.FileExtension;
				IReferenceAttachment referenceAttachment = (IReferenceAttachment)this.factory.CreateAttachment(draftItem, AttachmentType.Reference);
				newAttachment = referenceAttachment;
				newAttachmentId = newAttachment.Id;
				referenceAttachment.AttachLongPathName = this.ResultAttachmentContentUrl;
				referenceAttachment.ProviderEndpointUrl = this.ResultAttachmentWebServiceUrl;
				referenceAttachment.ProviderType = this.ResultAttachmentProviderType;
				referenceAttachment.FileName = attachment.FileName;
				newAttachment.IsInline = false;
				newAttachment.Save();
				draftItem.Save(SaveMode.NoConflictResolutionForceSave);
			}

			private void CopyAttachment(IAttachment attachment, IItem draftItem, out IAttachment newAttachment, out AttachmentId newAttachmentId)
			{
				newAttachment = this.factory.CloneAttachment(attachment, draftItem);
				if (newAttachment is IStreamAttachment)
				{
					newAttachment.FileName = GetWacAttachmentInfo.Implementation.GenerateDocumentNameForEditing(attachment.FileName, this.userDisplayName);
				}
				else
				{
					newAttachment.FileName = attachment.FileName;
				}
				newAttachmentId = newAttachment.Id;
				this.ResultAttachmentExtension = newAttachment.FileExtension;
				IReferenceAttachment referenceAttachment = newAttachment as IReferenceAttachment;
				if (referenceAttachment != null)
				{
					this.ResultAttachmentContentUrl = referenceAttachment.AttachLongPathName;
					this.ResultAttachmentWebServiceUrl = referenceAttachment.ProviderEndpointUrl;
					this.ResultAttachmentProviderType = referenceAttachment.ProviderType;
				}
				newAttachment.IsInline = false;
				newAttachment.Save();
				draftItem.Save(SaveMode.NoConflictResolutionForceSave);
			}

			internal static string GenerateDocumentNameForEditing(string fileName, string userDisplayName)
			{
				string extension = Path.GetExtension(fileName);
				string text = Path.GetFileNameWithoutExtension(fileName);
				string text2 = GetWacAttachmentInfo.Implementation.GenerateDocumentNameForEditing(text, userDisplayName, extension);
				int length = text2.Length;
				if (length > 150)
				{
					string ellipsis = Strings.Ellipsis;
					int num = length - 150;
					if (userDisplayName.Length > 75)
					{
						num += ellipsis.Length * 2;
						if (num % 2 == 1)
						{
							num++;
						}
						text = text.Substring(0, text.Length - num / 2);
						text += ellipsis;
						userDisplayName = userDisplayName.Substring(0, userDisplayName.Length - num / 2);
						userDisplayName += ellipsis;
					}
					else
					{
						num += ellipsis.Length;
						text = text.Substring(0, text.Length - num);
						text += ellipsis;
					}
					text2 = GetWacAttachmentInfo.Implementation.GenerateDocumentNameForEditing(text, userDisplayName, extension);
				}
				return text2;
			}

			private static string GenerateDocumentNameForEditing(string baseName, string displayName, string extension)
			{
				return string.Format(Strings.DocumentEditFormat, baseName, displayName) + extension;
			}

			private readonly AttachmentDataProvider provider;

			private readonly IXSOFactory factory;

			private readonly IStoreSession originalAttachmentSession;

			private readonly ModernGroupObjectType originalSessionModernGroupType;

			private readonly IMailboxSession draftAttachmentSession;

			private readonly StoreObjectId draftObjectId;

			private readonly IItem originalAttachmentRootItem;

			private readonly IAttachment originalAttachment;

			private readonly string originalAttachmentId;

			private readonly bool isEdit;

			private readonly string userDisplayName;

			private readonly Func<IStoreSession, StoreId, AttachmentId, string> idConverter;

			private IItem draftItem;
		}

		private class PostUploadMessageAsyncState
		{
			public string MailboxSmtpAddress { get; set; }

			public string LogonSmtpAddress { get; set; }

			public OWAService OwaService { get; set; }

			public string SessionId { get; set; }
		}

		public class AttachmentUploadException : Exception
		{
			public AttachmentUploadException(string message) : base(message)
			{
			}
		}
	}
}
