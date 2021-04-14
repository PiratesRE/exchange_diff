using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class WacRequestHandler : IHttpHandler
	{
		bool IHttpHandler.IsReusable
		{
			get
			{
				return false;
			}
		}

		static WacRequestHandler()
		{
			MdbCache.GetInstance().ExecuteAfterAsyncUpdate = delegate(IList<string> directories)
			{
				WacRequestHandler.cobaltStoreCleaner.Clean(directories);
			};
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			RequestDetailsLogger logger = OwaApplication.GetRequestDetailsLogger;
			string text = context.Request.QueryString["UserEmail"];
			logger.ActivityScope.SetProperty(OwaServerLogger.LoggerData.PrimarySmtpAddress, text);
			WacUtilities.SetEventId(logger, "WAC.BadRequest");
			context.Response.TrySkipIisCustomErrors = true;
			try
			{
				if (WacActiveMonitoringHandler.IsWacActiveMonitoringRequest(context.Request, context.Response))
				{
					WacUtilities.SetEventId(logger, "WAC.ActiveMonitoring");
				}
				else
				{
					WacRequest wacRequest = WacRequest.ParseWacRequest(text, context.Request);
					WacRequestHandler.SetCommonResponseHeaders(wacRequest, context.Response);
					string text2 = wacRequest.GetElapsedTime().TotalHours.ToString("0.00");
					logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.SessionElapsedTime, text2);
					if (wacRequest.IsExpired())
					{
						throw new OwaInvalidRequestException(string.Concat(new object[]
						{
							"Can't process ",
							wacRequest.RequestType,
							" request because the Url has expired. Hours since start: ",
							text2
						}));
					}
					logger.Set(WacRequestHandlerMetadata.ExchangeSessionId, wacRequest.ExchangeSessionId);
					switch (wacRequest.RequestType)
					{
					case WacRequestType.CheckFile:
						WacUtilities.SetEventId(logger, "WAC.CheckFile");
						WacRequestHandler.ProcessCheckFileRequest(context, wacRequest, logger);
						goto IL_28B;
					case WacRequestType.GetFile:
						WacUtilities.SetEventId(logger, "WAC.GetFile");
						WacRequestHandler.ProcessGetFileRequest(context, wacRequest, logger);
						goto IL_28B;
					case WacRequestType.Lock:
						WacUtilities.SetEventId(logger, "WAC.Lock");
						WacRequestHandler.IncrementLockCount(wacRequest);
						goto IL_28B;
					case WacRequestType.UnLock:
						WacUtilities.SetEventId(logger, "WAC.Unlock");
						WacRequestHandler.DecrementLockCount(wacRequest);
						goto IL_28B;
					case WacRequestType.RefreshLock:
						WacUtilities.SetEventId(logger, "WAC.RefreshLock");
						goto IL_28B;
					case WacRequestType.PutFile:
						WacUtilities.SetEventId(logger, "WAC.PutFile");
						WacRequestHandler.ReplaceAttachmentContent(context.Request.InputStream, wacRequest);
						goto IL_28B;
					case WacRequestType.Cobalt:
						WacUtilities.SetEventId(logger, "WAC.Cobalt");
						WacRequestHandler.ProcessCobaltRequest(context, wacRequest, delegate(Enum key, string value)
						{
							logger.ActivityScope.SetProperty(key, value);
						});
						goto IL_28B;
					case WacRequestType.DeleteFile:
						WacUtilities.SetEventId(logger, "WAC.DeleteFile");
						throw new OwaInvalidRequestException("Exchange does not support WAC's DeleteFile operation.");
					}
					WacUtilities.SetEventId(logger, "WAC.Unknown");
					throw new OwaInvalidRequestException("Invalid request type");
					IL_28B:;
				}
			}
			catch (Exception ex)
			{
				logger.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, ex.ToString());
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.RequestUrl, context.Request.Url.ToString());
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.UserAgent, context.Request.UserAgent);
				context.Response.Headers["X-WOPI-ServerError"] = ex.ToString();
				if (ex is OwaInvalidRequestException || ex is OwaOperationNotSupportedException || ex is OverBudgetException)
				{
					context.Response.StatusCode = 404;
				}
				else
				{
					context.Response.StatusCode = 500;
				}
			}
			finally
			{
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.WopiServerName, context.Request.Headers["X-WOPI-ServerVersion"]);
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.WopiClientVersion, context.Request.Headers["X-WOPI-InterfaceVersion"]);
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.WopiCorrelationId, context.Request.Headers["X-WOPI-CorrelationID"]);
			}
		}

		internal static void OnCacheEntryExpired(CachedAttachmentInfo attachmentInfo)
		{
			SimulatedWebRequestContext.ExecuteWithoutUserContext("WAC.CacheEntryExpired", delegate(RequestDetailsLogger logger)
			{
				WacUtilities.SetEventId(logger, "WAC.CacheEntryExpired");
				logger.ActivityScope.SetProperty(OwaServerLogger.LoggerData.PrimarySmtpAddress, attachmentInfo.MailboxSmtpAddress);
				CobaltStore store = attachmentInfo.CobaltStore;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					store.Saver.SaveAndLogExceptions(logger);
				});
				store.Dispose();
				Exception exception = adoperationResult.Exception;
				if (exception != null)
				{
					logger.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, exception.ToString());
				}
			});
		}

		private static void SetCommonResponseHeaders(WacRequest data, HttpResponse response)
		{
			response.StatusCode = 200;
			response.Headers["X-WOPI-ServerMachineName"] = WacUtilities.LocalServerName;
			response.Headers["X-WOPI-ServerVersion"] = WacUtilities.LocalServerVersion;
			bool perfTraceRequested = data.PerfTraceRequested;
			response.AppendToLog("&wacBox=" + data.MachineName);
			response.AppendToLog("&wacVer=" + data.ClientVersion);
			response.AppendToLog("&wacCorrelID=" + data.CorrelationID);
		}

		private static void ProcessCheckFileRequest(HttpContext context, WacRequest wacRequest, RequestDetailsLogger logger)
		{
			WacRequestHandler.UpdateAttachment(wacRequest, logger);
			CachedAttachmentInfo cachedAttachmentInfo = WacRequestHandler.GetCachedAttachmentInfo(wacRequest);
			string attachmentStreamHash = null;
			long attachmentStreamLength = 0L;
			string attachmentFileName = null;
			string attachmentExtension = null;
			bool attachmentIsProtected = false;
			WacRequestHandler.ProcessAttachment(wacRequest, PropertyOpenMode.ReadOnly, delegate(IExchangePrincipal exchangePrincipal, Attachment attachment, Stream stream, bool isProtected)
			{
				attachmentStreamHash = WacUtilities.GenerateSHA256HashForStream(stream);
				attachmentStreamLength = stream.Length;
				attachmentFileName = attachment.FileName;
				attachmentExtension = attachment.FileExtension;
				attachmentIsProtected = isProtected;
			});
			WacFileRep wacFileRep = wacRequest.WacFileRep;
			string downloadUrl = WacRequestHandler.GetDownloadUrl(context.Request, wacFileRep, wacRequest);
			WacCheckFileResponse wacCheckFileResponse = new WacCheckFileResponse(attachmentFileName, attachmentStreamLength, attachmentStreamHash, downloadUrl, cachedAttachmentInfo.MailboxSmtpAddress, cachedAttachmentInfo.LogonSmtpAddress, cachedAttachmentInfo.LogonDisplayName, cachedAttachmentInfo.LogonPuid, attachmentIsProtected, wacFileRep.DirectFileAccessEnabled, wacFileRep.WacExternalServicesEnabled, wacFileRep.OMEXEnabled);
			if (wacFileRep.IsEdit)
			{
				wacCheckFileResponse.UserCanWrite = true;
				wacCheckFileResponse.ReadOnly = false;
				wacCheckFileResponse.SupportsUpdate = true;
				wacCheckFileResponse.SupportsLocks = true;
				wacCheckFileResponse.SupportsCobalt = true;
				wacCheckFileResponse.SupportsFolders = true;
			}
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(WacCheckFileResponse));
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, wacCheckFileResponse);
				memoryStream.Position = 0L;
				context.Response.OutputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
		}

		private static bool MessageIsDraft(WacRequest wacRequest)
		{
			ADSessionSettings adsessionSettings;
			ExchangePrincipal exchangePrincipal = WacUtilities.GetExchangePrincipal(wacRequest, out adsessionSettings, wacRequest.WacFileRep.IsArchive);
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(wacRequest.CultureName);
			if (exchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.PublicFolder)
			{
				return false;
			}
			bool result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, cultureInfo, "Client=OWA;Action=WAC"))
			{
				IdConverterDependencies converterDependencies = new IdConverterDependencies.FromRawData(false, false, null, null, wacRequest.MailboxSmtpAddress.ToString(), null, mailboxSession, null);
				using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(wacRequest.EwsAttachmentId, converterDependencies))
				{
					Item rootItem = attachmentRetriever.RootItem;
					Attachment attachment = attachmentRetriever.Attachment;
					string text;
					string text2;
					result = WacUtilities.ItemIsMessageDraft(rootItem, attachment, out text, out text2);
				}
			}
			return result;
		}

		private static string GetDownloadUrl(HttpRequest request, WacFileRep wacFileRep, WacRequest wacRequest)
		{
			if (wacFileRep.DirectFileAccessEnabled)
			{
				Uri url = request.Url;
				string text = request.Headers["X-OWA-ProxyUri"];
				string text2;
				if (wacRequest == null || wacRequest.WacFileRep.LogonSid == null || string.IsNullOrWhiteSpace(wacRequest.WacFileRep.LogonSid.Value))
				{
					text2 = "WAC_no_user_sid";
				}
				else
				{
					Canary15 canary = new Canary15(wacRequest.WacFileRep.LogonSid.Value);
					text2 = canary.ToString();
				}
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}?id={2}&{3}={4}", new object[]
				{
					text,
					"/service.svc/s/GetFileAttachment",
					HttpUtility.UrlEncode(wacRequest.EwsAttachmentId),
					"X-OWA-CANARY",
					text2
				});
			}
			return string.Empty;
		}

		private static void ProcessGetFileRequest(HttpContext context, WacRequest wacRequest, RequestDetailsLogger logger)
		{
			WacRequestHandler.UpdateAttachment(wacRequest, logger);
			WacRequestHandler.ProcessAttachment(wacRequest, PropertyOpenMode.ReadOnly, delegate(IExchangePrincipal exchangePrincipal, Attachment attachment, Stream stream, bool contentProtected)
			{
				WacUtilities.WriteStreamBody(context.Response, stream);
			});
		}

		private static void UpdateAttachment(WacRequest wacRequest, RequestDetailsLogger logger)
		{
			string mailboxSmtpAddress = wacRequest.MailboxSmtpAddress.ToString();
			string ewsAttachmentId = wacRequest.EwsAttachmentId;
			CobaltStoreSaver cobaltStoreSaver;
			if (WacUtilities.ShouldUpdateAttachment(mailboxSmtpAddress, ewsAttachmentId, out cobaltStoreSaver))
			{
				logger.Set(WacRequestHandlerMetadata.Updated, true);
				cobaltStoreSaver.SaveAndLogExceptions(logger);
				return;
			}
			logger.Set(WacRequestHandlerMetadata.Updated, false);
		}

		private static void ReplaceAttachmentContent(Stream stream, WacRequest wacRequest)
		{
			WacUtilities.ReplaceAttachmentContent(wacRequest.MailboxSmtpAddress.ToString(), wacRequest.CultureName, wacRequest.EwsAttachmentId, wacRequest.WacFileRep.IsArchive, stream);
		}

		private static void ProcessCobaltRequest(HttpContext context, WacRequest wacRequest, Action<Enum, string> logDetail)
		{
			Stream inputStream = context.Request.InputStream;
			Stream outputStream = context.Response.OutputStream;
			CobaltStore cobaltStore = WacRequestHandler.GetCobaltStore(wacRequest);
			cobaltStore.ProcessRequest(inputStream, outputStream, logDetail);
		}

		private static CobaltStore GetCobaltStore(WacRequest wacRequest)
		{
			CachedAttachmentInfo attachmentInfo = WacRequestHandler.GetCachedAttachmentInfo(wacRequest);
			CobaltStore cobaltStore = attachmentInfo.CobaltStore;
			if (cobaltStore == null)
			{
				lock (attachmentInfo)
				{
					cobaltStore = attachmentInfo.CobaltStore;
					if (cobaltStore == null)
					{
						WacRequestHandler.ProcessAttachment(wacRequest, PropertyOpenMode.ReadOnly, delegate(IExchangePrincipal exchangePrincipal, Attachment attachment, Stream stream, bool contentProtected)
						{
							if (!WacRequestHandler.MessageIsDraft(wacRequest))
							{
								throw new NotSupportedException("Cell storage requests are only supported for draft items.");
							}
							cobaltStore = WacRequestHandler.CreateCobaltStore(exchangePrincipal, attachment, wacRequest, attachmentInfo);
						});
						attachmentInfo.CobaltStore = cobaltStore;
					}
				}
			}
			return cobaltStore;
		}

		private static CobaltStore CreateCobaltStore(IExchangePrincipal exchangePrincipal, Attachment attachment, WacRequest wacRequest, CachedAttachmentInfo attachmentInfo)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentException("exchangePrincipal");
			}
			if (exchangePrincipal.MailboxInfo.IsRemote)
			{
				throw new OwaInvalidRequestException("Remote mailboxes are not supported.");
			}
			Guid objectGuid = exchangePrincipal.MailboxInfo.MailboxDatabase.ObjectGuid;
			bool diagnosticsEnabled = WacConfiguration.Instance.DiagnosticsEnabled;
			MdbCache instance = MdbCache.GetInstance();
			string path = instance.GetPath(objectGuid);
			string correlationId = HttpContext.Current.Request.Headers["X-WOPI-CorrelationID"];
			CobaltStore store = new CobaltStore(path, objectGuid.ToString(), correlationId, diagnosticsEnabled, WacConfiguration.Instance.BlobStoreMemoryBudget);
			using (Stream contentStream = ((StreamAttachment)attachment).GetContentStream(PropertyOpenMode.ReadOnly))
			{
				store.Save(contentStream);
			}
			store.Saver.Initialize((string)wacRequest.MailboxSmtpAddress, wacRequest.ExchangeSessionId, WacConfiguration.Instance.AutoSaveInterval, delegate
			{
				using (Stream documentStream = store.GetDocumentStream())
				{
					WacRequestHandler.ReplaceAttachmentContent(documentStream, wacRequest);
				}
				return true;
			}, delegate(Exception exception)
			{
				store.SaveFailed(exception);
			});
			return store;
		}

		private static void IncrementLockCount(WacRequest wacRequest)
		{
			CachedAttachmentInfo cachedAttachmentInfo = WacRequestHandler.GetCachedAttachmentInfo(wacRequest);
			cachedAttachmentInfo.IncrementLockCount();
		}

		private static void DecrementLockCount(WacRequest wacRequest)
		{
			CachedAttachmentInfo cachedAttachmentInfo = WacRequestHandler.GetCachedAttachmentInfo(wacRequest);
			cachedAttachmentInfo.DecrementLockCount();
		}

		private static CachedAttachmentInfo GetCachedAttachmentInfo(WacRequest wacRequest)
		{
			return CachedAttachmentInfo.GetInstance(wacRequest.MailboxSmtpAddress.ToString(), wacRequest.EwsAttachmentId, wacRequest.ExchangeSessionId, wacRequest.WacFileRep.LogonSid, wacRequest.CultureName);
		}

		private static void ProcessUsingBudget(SecurityIdentifier logonUserSid, Action anAction, ADSessionSettings adSessionSettings)
		{
			using (IStandardBudget standardBudget = StandardBudget.Acquire(logonUserSid, BudgetType.Owa, adSessionSettings))
			{
				standardBudget.CheckOverBudget();
				string callerInfo = OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId);
				standardBudget.StartConnection(callerInfo);
				standardBudget.StartLocal(callerInfo, default(TimeSpan));
				anAction();
			}
		}

		private static void ProcessAttachment(WacRequest wacRequest, PropertyOpenMode openMode, WacUtilities.AttachmentProcessor attachmentProcessor)
		{
			ADSessionSettings adSessionSettings;
			ExchangePrincipal exchangePrincipal = WacUtilities.GetExchangePrincipal(wacRequest, out adSessionSettings, wacRequest.WacFileRep.IsArchive);
			WacRequestHandler.ProcessUsingBudget(wacRequest.WacFileRep.LogonSid, delegate
			{
				string ewsAttachmentId = wacRequest.EwsAttachmentId;
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(wacRequest.CultureName);
				string clientInfoString = "Client=OWA;Action=WAC";
				if (exchangePrincipal.RecipientTypeDetails.HasFlag(RecipientTypeDetails.PublicFolder) || exchangePrincipal.RecipientTypeDetails.HasFlag(RecipientTypeDetails.PublicFolderMailbox))
				{
					using (PublicFolderSession publicFolderSession = PublicFolderSession.OpenAsAdmin(null, exchangePrincipal, null, cultureInfo, clientInfoString, null))
					{
						WacUtilities.ProcessAttachment(publicFolderSession, ewsAttachmentId, exchangePrincipal, openMode, attachmentProcessor);
						return;
					}
				}
				using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, cultureInfo, clientInfoString))
				{
					WacUtilities.ProcessAttachment(mailboxSession, ewsAttachmentId, exchangePrincipal, openMode, attachmentProcessor);
				}
			}, adSessionSettings);
		}

		public const string GetFileAttachmentUrlLocalPath = "/service.svc/s/GetFileAttachment";

		private static CobaltStoreCleaner cobaltStoreCleaner = CobaltStoreCleaner.GetInstance();
	}
}
