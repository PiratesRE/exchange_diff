using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Transcoding;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Compliance;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.Protectors;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class WebReadyViewUtilities
	{
		public WebReadyViewUtilities(OwaContext owaContext)
		{
			this.owaContext = owaContext;
		}

		public Stream LoadDocument(bool isLoadingStream, out RightsManagedMessageDecryptionStatus decryptionStatus)
		{
			decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
			this.documentIdStringBuilder = new StringBuilder();
			string queryStringParameter = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "t");
			string queryStringParameter2 = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "pn", false);
			if (string.IsNullOrEmpty(queryStringParameter2))
			{
				this.currentPageNumber = 1;
			}
			else if (!int.TryParse(queryStringParameter2, out this.currentPageNumber) || this.currentPageNumber < 0)
			{
				throw new OwaInvalidRequestException("The page number is invalid");
			}
			Stream result;
			if (queryStringParameter.Equals("wss"))
			{
				result = this.LoadWSSDocument(isLoadingStream);
			}
			else if (queryStringParameter.Equals("unc"))
			{
				result = this.LoadUNCDocument(isLoadingStream);
			}
			else
			{
				if (!queryStringParameter.Equals("att"))
				{
					throw new OwaInvalidRequestException("The type(t) parameter is invalid");
				}
				result = this.LoadAttachmentDocument(isLoadingStream, out decryptionStatus);
			}
			if (this.mimeType == null || this.mimeType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
			{
				this.mimeType = string.Empty;
			}
			if (string.IsNullOrEmpty(this.fileName))
			{
				this.fileName = LocalizedStrings.GetNonEncoded(1797976510);
			}
			int num = this.fileName.LastIndexOf('.');
			if (num > 0)
			{
				this.fileExtension = this.fileName.Substring(num);
			}
			else
			{
				this.fileExtension = string.Empty;
			}
			if (this.fileExtension == null)
			{
				this.fileExtension = string.Empty;
			}
			if (!OwaRegistryKeys.WebReadyDocumentViewingWithInlineImage)
			{
				this.isSupportPaging = false;
			}
			else if (!string.IsNullOrEmpty(this.fileExtension))
			{
				this.isSupportPaging = (Array.IndexOf<string>(WebReadyViewUtilities.wordFileType, this.fileExtension.ToLowerInvariant()) < 0);
			}
			else
			{
				this.isSupportPaging = (Array.IndexOf<string>(WebReadyViewUtilities.wordFileType, this.mimeType.ToLowerInvariant()) < 0);
			}
			return result;
		}

		public void RenderOpenLink(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			AttachmentPolicy.Level levelForAttachment = AttachmentLevelLookup.GetLevelForAttachment(this.fileExtension, this.mimeType, this.owaContext.UserContext);
			output.Write("<span id=\"spnLnk\" tabindex=\"-1\">");
			SmallIconManager.RenderFileIcon(output, this.owaContext.UserContext, this.fileExtension, "tbLh", new string[0]);
			output.Write("<span class=\"tbLh\">");
			if (levelForAttachment == AttachmentPolicy.Level.ForceSave || levelForAttachment == AttachmentPolicy.Level.Allow)
			{
				output.Write("<a id=\"lnk\" href=\"");
				output.Write(this.openLink);
				output.Write("\" target=_blank onclick=\"");
				output.Write("return onClkAtmt(");
				output.Write((int)levelForAttachment);
				output.Write(")\" title=\"");
				Utilities.HtmlEncode(this.fileName, output);
				output.Write("\">");
				Utilities.HtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(this.fileName, null, false), output);
				output.Write("</a>");
			}
			else
			{
				Utilities.HtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(this.fileName, null, false), output);
			}
			if (this.fileSize > 0L)
			{
				output.Write(this.owaContext.UserContext.DirectionMark);
				output.Write(" ");
				output.Write(LocalizedStrings.GetHtmlEncoded(6409762));
				Utilities.RenderSizeWithUnits(output, this.fileSize, true);
				output.Write(this.owaContext.UserContext.DirectionMark);
				output.Write(LocalizedStrings.GetHtmlEncoded(-1023695022));
			}
			output.Write("</span></span>");
		}

		public void InvokeTaskManager()
		{
			if (!AppSettings.GetConfiguredValue<bool>("TranscodingServiceEnabled", false))
			{
				this.HandleFault(LocalizedStrings.GetNonEncoded(500694802));
				return;
			}
			AttachmentPolicy attachmentPolicy = this.owaContext.UserContext.AttachmentPolicy;
			using (Stream stream = this.LoadDocument(true, out this.decryptionStatus))
			{
				if (this.decryptionStatus.Failed)
				{
					SanitizedHtmlString irmErrorDetails = WebReadyViewUtilities.GetIrmErrorDetails(this.owaContext.UserContext, this.decryptionStatus);
					this.HandleFault(irmErrorDetails.ToString());
				}
				else if (!attachmentPolicy.WebReadyDocumentViewingEnable)
				{
					this.HandleFault(LocalizedStrings.GetNonEncoded(500694802));
				}
				else if (!AttachmentUtility.IsWebReadyDocument(this.fileExtension, this.mimeType))
				{
					this.HandleFault(LocalizedStrings.GetNonEncoded(-1584334964));
				}
				else
				{
					try
					{
						byte[] bytes = Encoding.Default.GetBytes(this.documentIdStringBuilder.ToString());
						byte[] value = null;
						using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
						{
							value = messageDigestForNonCryptographicPurposes.ComputeHash(bytes);
						}
						WebReadyViewUtilities.InitializeTranscodingService();
						Utilities.MakePageNoCacheNoStore(this.owaContext.HttpContext.Response);
						string sourceDocType = string.IsNullOrEmpty(this.fileExtension) ? this.mimeType : this.fileExtension.TrimStart(new char[]
						{
							'.'
						});
						try
						{
							TranscodingTaskManager.Transcode(BitConverter.ToString(value), this.owaContext.UserContext.Key.UserContextId, stream, sourceDocType, this.currentPageNumber, out this.totalPageNumber, this.owaContext.HttpContext.Response);
						}
						catch (TranscodingCrashException innerException)
						{
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerProcessFails, string.Empty, new object[]
							{
								this.owaContext.LogonIdentity.GetLogonName(),
								this.owaContext.LogonIdentity.UserSid.Value,
								this.messageFrom,
								this.messageSubject,
								this.messageId,
								this.documentPath,
								this.documentIdStringBuilder.ToString()
							});
							throw new TranscodingUnconvertibleFileException("Transcoding service crash when converting the document.", innerException, this);
						}
						if (this.currentPageNumber == 0)
						{
							this.currentPageNumber = this.totalPageNumber;
						}
					}
					catch (TranscodingException ex)
					{
						ExTraceGlobals.TranscodingTracer.TraceDebug<Type, string>(0L, "Exception: Type: {0} Error: {1}.", ex.GetType(), ex.Message);
						ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(ex, this.owaContext.MailboxIdentity);
						this.HandleFault(exceptionHandlingInformation.Message);
					}
				}
			}
		}

		private static void InitializeTranscodingService()
		{
			if (!TranscodingTaskManager.IsInitialized)
			{
				int num = OwaRegistryKeys.WebReadyDocumentViewingConversionTimeout;
				int num2 = OwaRegistryKeys.WebReadyDocumentViewingRecycleByConversions;
				string text = OwaRegistryKeys.WebReadyDocumentViewingTempFolderLocation;
				int num3 = OwaRegistryKeys.WebReadyDocumentViewingCacheDiskQuota;
				int num4 = OwaRegistryKeys.WebReadyDocumentViewingExcelRowsPerPage;
				int num5 = OwaRegistryKeys.WebReadyDocumentViewingMaxDocumentInputSize;
				int num6 = OwaRegistryKeys.WebReadyDocumentViewingMaxDocumentOutputSize;
				bool webReadyDocumentViewingWithInlineImage = OwaRegistryKeys.WebReadyDocumentViewingWithInlineImage;
				int num7 = OwaRegistryKeys.WebReadyDocumentViewingMemoryLimitInMB;
				if (num <= 0)
				{
					num = 20;
				}
				if (num2 <= 0)
				{
					num2 = 150;
				}
				if (!text.EndsWith("\\"))
				{
					text += "\\";
				}
				if (Path.IsPathRooted(text) && text.Length <= TranscodingTaskManager.XCRootPathMaxLength)
				{
					if (Directory.Exists(text))
					{
						goto IL_ED;
					}
					try
					{
						Directory.CreateDirectory(text);
						goto IL_ED;
					}
					catch (IOException ex)
					{
						ExTraceGlobals.TranscodingTracer.TraceDebug<Type, string>(0L, "Fail to create the XC temp folder. Exception: Type: {0} Error: {1}.", ex.GetType(), ex.Message);
						text = OwaRegistryKeys.DefaultTempFolderLocation;
						goto IL_ED;
					}
					catch (UnauthorizedAccessException ex2)
					{
						ExTraceGlobals.TranscodingTracer.TraceDebug<Type, string>(0L, "Fail to create the XC temp folder. Exception: Type: {0} Error: {1}.", ex2.GetType(), ex2.Message);
						text = OwaRegistryKeys.DefaultTempFolderLocation;
						goto IL_ED;
					}
				}
				text = OwaRegistryKeys.DefaultTempFolderLocation;
				IL_ED:
				if (num3 <= 0)
				{
					num3 = 1000;
				}
				if (num4 <= 0)
				{
					num4 = 200;
				}
				if (num5 <= 0)
				{
					num5 = 5000;
				}
				if (num6 <= 0)
				{
					num6 = 5000;
				}
				if (num7 <= 0)
				{
					num7 = 200;
				}
				TranscodingTaskManager.Initialize(num, num2, text, num3, num4, num5, num6, webReadyDocumentViewingWithInlineImage, HtmlFormat.Version40, num7);
			}
		}

		private static SanitizedHtmlString GetOfficeDownloadAnchor(CultureInfo userCulture)
		{
			SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", new object[]
			{
				LocalizedStrings.GetNonEncoded(1124412272),
				LocalizedStrings.GetNonEncoded(34768154)
			});
			return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-539149404, userCulture), new object[]
			{
				sanitizedHtmlString
			});
		}

		private static SanitizedHtmlString GetIrmErrorDetails(UserContext userContext, RightsManagedMessageDecryptionStatus decryptionStatus)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			RightsManagementFailureCode failureCode = decryptionStatus.FailureCode;
			if (failureCode > RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				switch (failureCode)
				{
				case RightsManagementFailureCode.FailedToExtractTargetUriFromMex:
				case RightsManagementFailureCode.FailedToDownloadMexData:
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-843834599));
					goto IL_308;
				case RightsManagementFailureCode.GetServerInfoFailed:
					goto IL_19E;
				case RightsManagementFailureCode.InternalLicensingDisabled:
					break;
				case RightsManagementFailureCode.ExternalLicensingDisabled:
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(-641698444), new object[]
					{
						WebReadyViewUtilities.GetOfficeDownloadAnchor(userContext.UserCulture)
					});
					goto IL_308;
				default:
					switch (failureCode)
					{
					case RightsManagementFailureCode.ServerRightNotGranted:
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-271360565));
						goto IL_308;
					case RightsManagementFailureCode.InvalidLicensee:
						goto IL_189;
					case RightsManagementFailureCode.FeatureDisabled:
						break;
					case RightsManagementFailureCode.NotSupported:
						sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(-1308596751), new object[]
						{
							WebReadyViewUtilities.GetOfficeDownloadAnchor(userContext.UserCulture)
						});
						goto IL_308;
					case RightsManagementFailureCode.CorruptData:
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-987520932));
						goto IL_308;
					default:
						if (failureCode != RightsManagementFailureCode.Success)
						{
							goto IL_19E;
						}
						goto IL_308;
					}
					break;
				}
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(-1308596751), new object[]
				{
					WebReadyViewUtilities.GetOfficeDownloadAnchor(userContext.UserCulture)
				});
				goto IL_308;
			}
			if (failureCode == RightsManagementFailureCode.UserRightNotGranted)
			{
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(149546568), new object[]
				{
					WebReadyViewUtilities.GetOfficeDownloadAnchor(userContext.UserCulture)
				});
				goto IL_308;
			}
			if (failureCode != RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				goto IL_19E;
			}
			IL_189:
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-492186842));
			goto IL_308;
			IL_19E:
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-624237727));
			Exception exception = decryptionStatus.Exception;
			if (Globals.ShowDebugInformation && exception != null && exception.InnerException != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				userContext.RenderThemeImage(stringBuilder, ThemeFileId.Expand, string.Empty, new object[0]);
				sanitizingStringBuilder.AppendFormat("<div onclick=\"document.getElementById('divDtls').style.display='';this.style.display='none';adjHght();\" style=\"cursor: pointer; color: #3165cd;\">" + stringBuilder.ToString() + "&nbsp;{0}</div><div id=\"divDtls\" style='display:none'>", new object[]
				{
					LocalizedStrings.GetNonEncoded(-610047827)
				});
				string text = string.Empty;
				RightsManagementFailureCode failureCode2 = decryptionStatus.FailureCode;
				Exception innerException = exception.InnerException;
				if (innerException is RightsManagementException)
				{
					RightsManagementException ex = (RightsManagementException)innerException;
					text = ex.RmsUrl;
				}
				int num = 0;
				while (num < 10 && innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
					num++;
				}
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(1633606253), new object[]
				{
					innerException.Message
				});
				if (!string.IsNullOrEmpty(text))
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(2115316283), new object[]
					{
						text
					});
				}
				if (failureCode2 != RightsManagementFailureCode.Success)
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(970140031), new object[]
					{
						failureCode2
					});
				}
				sanitizingStringBuilder.Append("</div>");
			}
			IL_308:
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		private Stream LoadWSSDocument(bool isLoadingStream)
		{
			if (!DocumentLibraryUtilities.IsNavigationToWSSAllowed(this.owaContext.UserContext))
			{
				throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
			}
			DocumentLibraryObjectId documentLibraryObjectId = DocumentLibraryUtilities.CreateDocumentLibraryObjectId(this.owaContext);
			SharepointSession session = this.owaContext.UserContext.LogonIdentity.CreateSharepointSession(documentLibraryObjectId);
			SharepointDocument sharepointDocument = SharepointDocument.Read(session, documentLibraryObjectId);
			this.mimeType = sharepointDocument.FileType;
			this.fileName = Path.GetFileName(sharepointDocument.Uri.ToString());
			this.fileSize = sharepointDocument.Size;
			this.documentPath = sharepointDocument.Uri.ToString();
			string queryStringParameter = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "URL");
			this.openLink = string.Concat(new string[]
			{
				"ev.owa?ns=SharepointDocument&ev=GetDoc&allowLevel2=1&URL=",
				Utilities.UrlEncode(queryStringParameter),
				"&id=",
				Utilities.UrlEncode(documentLibraryObjectId.ToBase64String()),
				Utilities.GetCanaryRequestParameter()
			});
			this.documentIdStringBuilder.Append(documentLibraryObjectId.ToBase64String());
			this.documentIdStringBuilder.Append("-");
			this.documentIdStringBuilder.Append(sharepointDocument.VersionControl.TipVersion);
			if (!isLoadingStream)
			{
				return null;
			}
			return sharepointDocument.GetDocument();
		}

		private Stream LoadUNCDocument(bool isLoadingStream)
		{
			if (!DocumentLibraryUtilities.IsNavigationToUNCAllowed(this.owaContext.UserContext))
			{
				throw new OwaSegmentationException("Access to Unc documents is disabled");
			}
			DocumentLibraryObjectId documentLibraryObjectId = DocumentLibraryUtilities.CreateDocumentLibraryObjectId(this.owaContext);
			UncSession session = this.owaContext.UserContext.LogonIdentity.CreateUncSession(documentLibraryObjectId);
			UncDocument uncDocument = UncDocument.Read(session, documentLibraryObjectId);
			this.mimeType = (uncDocument[UncDocumentSchema.FileType] as string);
			this.fileName = Path.GetFileName(uncDocument.Uri.ToString());
			this.fileSize = uncDocument.Size;
			this.documentPath = uncDocument.Uri.ToString();
			string queryStringParameter = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "URL");
			this.openLink = string.Concat(new string[]
			{
				"ev.owa?ns=UncDocument&ev=GetDoc&allowLevel2=1&URL=",
				Utilities.UrlEncode(queryStringParameter),
				"&id=",
				Utilities.UrlEncode(documentLibraryObjectId.ToBase64String()),
				Utilities.GetCanaryRequestParameter()
			});
			this.documentIdStringBuilder.Append(documentLibraryObjectId.ToBase64String());
			this.documentIdStringBuilder.Append("-");
			object obj = uncDocument.TryGetProperty(UncItemSchema.LastModifiedDate);
			if (obj is DateTime)
			{
				this.documentIdStringBuilder.Append(((DateTime)obj).Ticks);
			}
			if (!isLoadingStream)
			{
				return null;
			}
			return uncDocument.GetDocument();
		}

		private Stream LoadAttachmentDocument(bool isLoadingStream, out RightsManagedMessageDecryptionStatus decryptionStatus)
		{
			decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
			StringBuilder stringBuilder = new StringBuilder();
			List<AttachmentId> list = new List<AttachmentId>();
			UserContext userContext = this.owaContext.UserContext;
			string queryStringParameter = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "ewsid", false);
			bool flag = string.IsNullOrEmpty(queryStringParameter);
			OwaStoreObjectId owaStoreObjectId;
			string text;
			if (!flag)
			{
				stringBuilder.Append("service.svc/s/GetFileAttachment?id=");
				stringBuilder.Append(Utilities.UrlEncode(queryStringParameter));
				string canary15CookieValue = Utilities.GetCanary15CookieValue();
				if (canary15CookieValue != null)
				{
					stringBuilder.Append("&X-OWA-CANARY=" + canary15CookieValue);
				}
				IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(queryStringParameter, BasicTypes.Attachment, list);
				owaStoreObjectId = OwaStoreObjectId.CreateFromMailboxItemId(idHeaderInformation.ToStoreObjectId());
				text = owaStoreObjectId.ToString();
			}
			else
			{
				text = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "id");
				owaStoreObjectId = OwaStoreObjectId.CreateFromString(text);
				stringBuilder.Append("attachment.ashx?attach=1&id=");
				stringBuilder.Append(Utilities.UrlEncode(text));
			}
			Stream result;
			using (Item item = Utilities.GetItem<Item>(userContext, owaStoreObjectId, new PropertyDefinition[]
			{
				ItemSchema.SentRepresentingDisplayName,
				ItemSchema.Subject
			}))
			{
				if (!Utilities.IsPublic(item) && userContext.IsIrmEnabled && isLoadingStream)
				{
					item.OpenAsReadWrite();
				}
				if (Utilities.IsIrmRestricted(item))
				{
					if (!userContext.IsIrmEnabled || userContext.IsBasicExperience)
					{
						decryptionStatus = RightsManagedMessageDecryptionStatus.FeatureDisabled;
						return null;
					}
					RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)item;
					if (!rightsManagedMessageItem.CanDecode)
					{
						decryptionStatus = RightsManagedMessageDecryptionStatus.NotSupported;
						return null;
					}
					try
					{
						Utilities.IrmDecryptIfRestricted(item, userContext, true);
					}
					catch (RightsManagementPermanentException exception)
					{
						decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
						return null;
					}
					catch (RightsManagementTransientException exception2)
					{
						decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception2);
						return null;
					}
				}
				this.messageFrom = ItemUtility.GetProperty<string>(item, ItemSchema.SentRepresentingDisplayName, string.Empty);
				this.messageSubject = ItemUtility.GetProperty<string>(item, ItemSchema.Subject, string.Empty);
				this.messageId = text;
				if (flag)
				{
					Utilities.FillAttachmentIdList(item, this.owaContext.HttpContext.Request, list);
				}
				using (StreamAttachment streamAttachment = Utilities.GetAttachment(item, list, userContext) as StreamAttachment)
				{
					if (streamAttachment == null)
					{
						throw new OwaInvalidRequestException("Attachment is not a stream attachment");
					}
					this.mimeType = (streamAttachment.ContentType ?? streamAttachment.CalculatedContentType);
					this.fileName = ((!string.IsNullOrEmpty(streamAttachment.DisplayName)) ? streamAttachment.DisplayName : streamAttachment.FileName);
					this.fileSize = streamAttachment.Size;
					this.documentPath = this.fileName;
					this.documentIdStringBuilder.Append(Convert.ToBase64String(userContext.MailboxSession.MailboxOwner.MailboxInfo.GetDatabaseGuid().ToByteArray()));
					this.documentIdStringBuilder.Append("-");
					for (int i = 0; i < list.Count; i++)
					{
						this.documentIdStringBuilder.Append(list[i].ToBase64String());
						this.documentIdStringBuilder.Append("-");
						if (flag)
						{
							stringBuilder.Append("&attid");
							stringBuilder.Append(i.ToString(CultureInfo.InstalledUICulture));
							stringBuilder.Append("=");
							stringBuilder.Append(Utilities.UrlEncode(list[i].ToBase64String()));
						}
					}
					if (flag)
					{
						stringBuilder.Append("&attcnt=");
						stringBuilder.Append(list.Count);
					}
					this.documentIdStringBuilder.Append(streamAttachment.LastModifiedTime.UtcTicks);
					this.openLink = stringBuilder.ToString();
					if (isLoadingStream)
					{
						Stream contentStream = streamAttachment.GetContentStream();
						MsoIpiResult msoIpiResult = MsoIpiResult.Unknown;
						try
						{
							msoIpiResult = ProtectorsManager.Instance.IsProtected(this.fileName, contentStream);
						}
						catch (AttachmentProtectionException exception3)
						{
							decryptionStatus = new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.CorruptData, exception3);
							return null;
						}
						if (msoIpiResult == MsoIpiResult.Protected)
						{
							this.isIrmProtected = true;
							contentStream.Dispose();
							if (!userContext.IsIrmEnabled || userContext.IsBasicExperience)
							{
								decryptionStatus = RightsManagedMessageDecryptionStatus.FeatureDisabled;
								result = null;
							}
							else
							{
								UseLicenseAndUsageRights useLicenseAndUsageRights;
								bool flag2;
								Stream stream;
								try
								{
									stream = StreamAttachment.OpenRestrictedAttachment(streamAttachment, this.owaContext.ExchangePrincipal.MailboxInfo.OrganizationId, this.owaContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.owaContext.LogonIdentity.UserSid, this.owaContext.ExchangePrincipal.RecipientTypeDetails, out useLicenseAndUsageRights, out flag2);
								}
								catch (RightsManagementPermanentException exception4)
								{
									decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception4);
									return null;
								}
								catch (RightsManagementTransientException exception5)
								{
									decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception5);
									return null;
								}
								if (flag2 && ObjectClass.IsMessage(item.ClassName, false) && !Utilities.IsIrmRestricted(item))
								{
									object obj = item.TryGetProperty(MessageItemSchema.IsDraft);
									if (obj is bool && !(bool)obj && !DrmClientUtils.IsCachingOfLicenseDisabled(useLicenseAndUsageRights.UseLicense))
									{
										streamAttachment[AttachmentSchema.DRMRights] = useLicenseAndUsageRights.UsageRights;
										streamAttachment[AttachmentSchema.DRMExpiryTime] = useLicenseAndUsageRights.ExpiryTime;
										using (Stream stream2 = streamAttachment.OpenPropertyStream(AttachmentSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
										{
											DrmEmailCompression.CompressUseLicense(useLicenseAndUsageRights.UseLicense, stream2);
										}
										streamAttachment[AttachmentSchema.DRMPropsSignature] = useLicenseAndUsageRights.DRMPropsSignature;
										streamAttachment.Save();
										item.Save(SaveMode.ResolveConflicts);
									}
								}
								string conversationOwnerFromPublishLicense = DrmClientUtils.GetConversationOwnerFromPublishLicense(useLicenseAndUsageRights.PublishingLicense);
								RmsTemplate rmsTemplate = RmsTemplate.CreateFromPublishLicense(useLicenseAndUsageRights.PublishingLicense);
								this.isPrintRestricted = !useLicenseAndUsageRights.UsageRights.IsUsageRightGranted(ContentRight.Print);
								this.isCopyRestricted = !useLicenseAndUsageRights.UsageRights.IsUsageRightGranted(ContentRight.Extract);
								SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
								string str = string.Format(LocalizedStrings.GetNonEncoded(-500320626), rmsTemplate.Name, rmsTemplate.Description);
								sanitizingStringBuilder.Append(str);
								if (!string.IsNullOrEmpty(conversationOwnerFromPublishLicense))
								{
									sanitizingStringBuilder.Append("<br>");
									sanitizingStringBuilder.Append(string.Format(LocalizedStrings.GetNonEncoded(1670455506), conversationOwnerFromPublishLicense));
								}
								this.irmInfobarMessage = sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
								result = stream;
							}
						}
						else
						{
							contentStream.Seek(0L, SeekOrigin.Begin);
							result = contentStream;
						}
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		private void HandleFault(string message)
		{
			this.errorMessage = message;
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string MimeType
		{
			get
			{
				return this.mimeType;
			}
		}

		public string FileExtension
		{
			get
			{
				return this.fileExtension;
			}
		}

		public bool IsCopyRestricted
		{
			get
			{
				return this.isCopyRestricted;
			}
		}

		public bool IsPrintRestricted
		{
			get
			{
				return this.isPrintRestricted;
			}
		}

		public bool IsIrmProtected
		{
			get
			{
				return this.isIrmProtected;
			}
		}

		public SanitizedHtmlString IrmInfobarMessage
		{
			get
			{
				return this.irmInfobarMessage;
			}
		}

		public bool IsSupportPaging
		{
			get
			{
				return this.isSupportPaging;
			}
		}

		public int CurrentPageNumber
		{
			get
			{
				return this.currentPageNumber;
			}
		}

		public int TotalPageNumber
		{
			get
			{
				return this.totalPageNumber;
			}
		}

		public bool HasError
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorMessage);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private const string SharepointType = "wss";

		private const string UncDocumentType = "unc";

		private const string AttachmentType = "att";

		private const string OctetStreamMimeType = "application/octet-stream";

		private const string IdDelimiter = "-";

		private static string[] wordFileType = new string[]
		{
			".doc",
			"application/msword",
			".dot",
			".rtf",
			".docx",
			"application/vnd.openxmlformats-officedocument.wordprocessingml.document"
		};

		private string fileExtension;

		private string mimeType;

		private string fileName;

		private long fileSize;

		private string openLink;

		private int currentPageNumber;

		private int totalPageNumber;

		private bool isSupportPaging;

		private bool isCopyRestricted;

		private bool isPrintRestricted;

		private bool isIrmProtected;

		private SanitizedHtmlString irmInfobarMessage;

		private RightsManagedMessageDecryptionStatus decryptionStatus;

		private StringBuilder documentIdStringBuilder;

		private OwaContext owaContext;

		private string errorMessage;

		private string messageFrom = string.Empty;

		private string messageSubject = string.Empty;

		private string messageId = string.Empty;

		private string documentPath = string.Empty;
	}
}
