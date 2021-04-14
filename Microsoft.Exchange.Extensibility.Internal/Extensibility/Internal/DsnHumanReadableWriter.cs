using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.DsnAndQuotaGeneration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class DsnHumanReadableWriter
	{
		public DsnHumanReadableWriter(Dictionary<string, string> configuredStatusCodes)
		{
			this.configuredStatusCodes = configuredStatusCodes;
			this.defaultCulture = MessageLanguageParser.GetDefaultCulture();
		}

		private DsnHumanReadableWriter()
		{
			this.configuredStatusCodes = new Dictionary<string, string>(0);
			this.defaultCulture = MessageLanguageParser.GetDefaultCulture();
		}

		public static DsnHumanReadableWriter DefaultDsnHumanReadableWriter
		{
			get
			{
				if (DsnHumanReadableWriter.dsnHumanReadableWriterWithoutConfig == null)
				{
					DsnHumanReadableWriter.dsnHumanReadableWriterWithoutConfig = new DsnHumanReadableWriter();
				}
				return DsnHumanReadableWriter.dsnHumanReadableWriterWithoutConfig;
			}
		}

		internal CultureInfo DefaultCulture
		{
			get
			{
				return this.defaultCulture;
			}
			set
			{
				this.defaultCulture = value;
			}
		}

		public static LocalizedString GetLocalizedSubjectPrefix(DsnFlags dsnType)
		{
			if (dsnType <= DsnFlags.Relay)
			{
				switch (dsnType)
				{
				case DsnFlags.Delivery:
					return SystemMessages.DeliveredSubject;
				case DsnFlags.Delay:
					return SystemMessages.DelayedSubject;
				case DsnFlags.Delivery | DsnFlags.Delay:
					goto IL_4C;
				case DsnFlags.Failure:
					break;
				default:
					if (dsnType != DsnFlags.Relay)
					{
						goto IL_4C;
					}
					return SystemMessages.RelayedSubject;
				}
			}
			else
			{
				if (dsnType == DsnFlags.Expansion)
				{
					return SystemMessages.ExpandedSubject;
				}
				if (dsnType != DsnFlags.Quarantine)
				{
					goto IL_4C;
				}
			}
			return SystemMessages.FailedSubject;
			IL_4C:
			throw new InvalidOperationException("Unexpected dsn type.");
		}

		public static LocalizedString GetLocalizedApprovalPrefix(ApprovalInformation.ApprovalNotificationType approvalType)
		{
			switch (approvalType)
			{
			case ApprovalInformation.ApprovalNotificationType.DecisionConflict:
				return SystemMessages.DecisionConflictNotificationSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ApprovalRequest:
				return SystemMessages.ApprovalRequestSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ModeratedTransportReject:
			case ApprovalInformation.ApprovalNotificationType.ModeratedTransportRejectWithComments:
				return SystemMessages.RejectedNotificationSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.DecisionUpdate:
				return SystemMessages.DecisionProcessedNotificationSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ApprovalRequestExpiry:
				return SystemMessages.ApprovalRequestExpiredNotificationSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ExpiryNotification:
			case ApprovalInformation.ApprovalNotificationType.ModeratorExpiryNotification:
				return SystemMessages.ApprovalRequestExpiredNotificationSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ModeratorsNdrNotification:
				return SystemMessages.ModeratorsNdrSubjectPrefix;
			case ApprovalInformation.ApprovalNotificationType.ModeratorsOofNotification:
				return SystemMessages.ModeratorsOofSubjectPrefix;
			default:
				throw new InvalidOperationException("Unexpected approval type.");
			}
		}

		public static string[] GetDsnParamTexts(DsnParamText dsnParamText, DsnParameters dsnParameters, CultureInfo cultureInfo, OutboundCodePageDetector detector, out bool overwriteDefault)
		{
			string[] array = dsnParamText.GenerateTexts(dsnParameters, cultureInfo, out overwriteDefault);
			if (array != null && array.Length != 0)
			{
				foreach (string value in array)
				{
					if (!string.IsNullOrEmpty(value))
					{
						detector.AddText(value);
					}
				}
			}
			return array;
		}

		public void CreateDsnHumanReadableBody(Stream dsnBodyStream, IEnumerable<CultureInfo> cultureInfoList, string originalSubject, List<DsnRecipientInfo> recipientList, DsnFlags dsnFlag, string reportingServer, string remoteMta, DateTime? expiryTime, HeaderList originalHeaders, out CultureInfo chosenCultureInfo, out Charset charset)
		{
			CultureInfo cultureInfo = null;
			foreach (CultureInfo cultureInfo2 in cultureInfoList)
			{
				if (MessageLanguageParser.IsCultureSupported(cultureInfo2))
				{
					cultureInfo = cultureInfo2;
					break;
				}
			}
			if (cultureInfo == null)
			{
				cultureInfo = this.DefaultCulture;
			}
			chosenCultureInfo = cultureInfo;
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			IdnMapping idnMapping = new IdnMapping();
			foreach (DsnRecipientInfo dsnRecipientInfo in recipientList)
			{
				if (dsnFlag == DsnFlags.Failure)
				{
					if (string.Equals(dsnRecipientInfo.EnhancedStatusCode, "5.2.2", StringComparison.Ordinal) && !string.IsNullOrEmpty(dsnRecipientInfo.StatusText) && dsnRecipientInfo.StatusText.StartsWith("MSEXCH:MSExchangeIS:"))
					{
						dsnRecipientInfo.DsnHumanReadableExplanation = SystemMessages.HumanText5_2_2_StoreNDR.ToString(cultureInfo);
						dsnRecipientInfo.IsCustomizedDsn = false;
					}
					else
					{
						bool isCustomizedDsn;
						dsnRecipientInfo.DsnHumanReadableExplanation = this.GetNDRHumanReadableExplanation(cultureInfo, dsnRecipientInfo.EnhancedStatusCode, false, out isCustomizedDsn);
						dsnRecipientInfo.IsCustomizedDsn = isCustomizedDsn;
					}
					outboundCodePageDetector.AddText(dsnRecipientInfo.DsnHumanReadableExplanation);
				}
				if (!string.IsNullOrEmpty(dsnRecipientInfo.DisplayName))
				{
					outboundCodePageDetector.AddText(dsnRecipientInfo.DisplayName);
				}
				else
				{
					RoutingAddress routingAddress = (RoutingAddress)dsnRecipientInfo.Address;
					string domainPart = routingAddress.DomainPart;
					if (!string.IsNullOrEmpty(domainPart))
					{
						try
						{
							string unicode = idnMapping.GetUnicode(domainPart);
							outboundCodePageDetector.AddText(unicode);
							dsnRecipientInfo.DecodedAddress = string.Concat(new string[]
							{
								routingAddress.LocalPart + "@" + unicode
							});
						}
						catch (ArgumentException arg)
						{
							ExTraceGlobals.DsnTracer.TraceDebug<string, ArgumentException>(0L, "Cannot decode domain '{0}' as IDN, exception: {1}", domainPart, arg);
						}
					}
				}
			}
			this.CreateDsnHumanReadableBody(dsnBodyStream, cultureInfo, originalSubject, recipientList, outboundCodePageDetector, dsnFlag, reportingServer, remoteMta, expiryTime, originalHeaders, out charset, null, false, true);
		}

		public void CreateDsnHumanReadableBody(Stream dsnBodyStream, CultureInfo cultureInfo, string originalSubject, List<DsnRecipientInfo> recipientList, OutboundCodePageDetector codepageDetector, DsnFlags dsnFlag, string reportingServer, string remoteMta, DateTime? expiryTime, HeaderList originalHeaders, out Charset charset)
		{
			this.CreateDsnHumanReadableBody(dsnBodyStream, cultureInfo, originalSubject, recipientList, codepageDetector, dsnFlag, reportingServer, remoteMta, expiryTime, originalHeaders, out charset, null, false, true);
		}

		public void CreateDsnHumanReadableBody(Stream dsnBodyStream, CultureInfo cultureInfo, string originalSubject, List<DsnRecipientInfo> recipientList, OutboundCodePageDetector codepageDetector, DsnFlags dsnFlag, string reportingServer, string remoteMta, DateTime? expiryTime, HeaderList originalHeaders, out Charset charset, DsnParameters dsnParameters, bool isExternal, bool isNDRDiagnosticInfoEnabled)
		{
			DsnHumanReadableWriter.DsnLocalizedTexts dsnLocalizedTexts = DsnHumanReadableWriter.DsnLocalizedTexts.GetDsnLocalizedTexts(dsnFlag);
			string text = null;
			string text2 = null;
			string text3;
			if (isExternal && dsnFlag == DsnFlags.Failure && !string.IsNullOrEmpty(remoteMta))
			{
				text3 = SystemMessages.ExternalFailedHumanReadableTopText(remoteMta).ToString(cultureInfo);
				text = SystemMessages.ExternalFailedHumanReadableErrorText(remoteMta).ToString(cultureInfo);
				text2 = SystemMessages.ExternalFailedHumanReadableErrorNoDetailText(remoteMta).ToString(cultureInfo);
				codepageDetector.AddText(text);
				codepageDetector.AddText(text2);
			}
			else
			{
				text3 = dsnLocalizedTexts.TopText.ToString(cultureInfo);
			}
			string value = dsnLocalizedTexts.Subject.ToString(cultureInfo);
			string[] bottomTexts = DsnHumanReadableWriter.GetBottomTexts(dsnFlag, originalSubject, remoteMta, expiryTime, cultureInfo);
			bool flag;
			string[] dsnParamTexts = DsnHumanReadableWriter.GetDsnParamTexts(DsnParamText.PerMessageDsnParamText, dsnParameters, cultureInfo, codepageDetector, out flag);
			string text4 = SystemMessages.BodyHeaderFontTag.ToString(cultureInfo);
			string text5 = SystemMessages.BodyBlockFontTag.ToString(cultureInfo);
			string text6 = SystemMessages.EnhancedDsnTextFontTag.ToString(cultureInfo);
			string text7 = null;
			string text8 = null;
			string text9 = null;
			string text10 = null;
			string text11 = null;
			bool flag2 = (dsnFlag == DsnFlags.Failure || dsnFlag == DsnFlags.Quarantine || dsnFlag == DsnFlags.Delay) && isNDRDiagnosticInfoEnabled;
			codepageDetector.AddText(text3);
			if (bottomTexts != null)
			{
				foreach (string value2 in bottomTexts)
				{
					if (!string.IsNullOrEmpty(value2))
					{
						codepageDetector.AddText(value2);
					}
				}
			}
			codepageDetector.AddText(text4);
			codepageDetector.AddText(text5);
			codepageDetector.AddText(text6);
			if (flag2)
			{
				text7 = SystemMessages.OriginalHeadersTitle.ToString(cultureInfo);
				text8 = SystemMessages.DiagnosticInformationTitle.ToString(cultureInfo);
				text9 = SystemMessages.GeneratingServerTitle.ToString(cultureInfo);
				text10 = SystemMessages.ReceivingServerTitle.ToString(cultureInfo);
				text11 = SystemMessages.DiagnosticsFontTag.ToString(cultureInfo);
				codepageDetector.AddText(text7);
				codepageDetector.AddText(text8);
				codepageDetector.AddText(text9);
				codepageDetector.AddText(text10);
				codepageDetector.AddText(text11);
			}
			codepageDetector.AddText(value);
			if (!string.IsNullOrEmpty(originalSubject))
			{
				codepageDetector.AddText(originalSubject);
			}
			charset = DsnHumanReadableWriter.GetCharset(codepageDetector, cultureInfo);
			DsnHumanReadableWriter.EncodedTextWriter encodedTextWriter = new DsnHumanReadableWriter.EncodedTextWriter(dsnBodyStream, charset.GetEncoding());
			encodedTextWriter.WriteToStream("<html>\r\n<Head></head>");
			if (dsnParamTexts != null && dsnParamTexts.Length > 0)
			{
				if (!flag)
				{
					DsnHumanReadableWriter.WriteTopBlock(encodedTextWriter, text3, text4);
				}
				foreach (string topText in dsnParamTexts)
				{
					DsnHumanReadableWriter.WriteTopBlock(encodedTextWriter, topText, text4);
				}
			}
			else
			{
				DsnHumanReadableWriter.WriteTopBlock(encodedTextWriter, text3, text4);
			}
			encodedTextWriter.WriteToStream(text5);
			DsnHumanReadableWriter.WriteRecipientsBlock(encodedTextWriter, dsnFlag, recipientList, remoteMta, text5, text6, text, text2, isExternal);
			if (!isExternal || dsnFlag != DsnFlags.Failure)
			{
				DsnHumanReadableWriter.WriteBottomBlock(encodedTextWriter, bottomTexts, true);
			}
			encodedTextWriter.WriteToStream("</font>\r\n");
			if (flag2)
			{
				this.WriteDiagnosticBlock(encodedTextWriter, text8, text7, text9, text10, recipientList, remoteMta, reportingServer, originalHeaders, text11);
			}
			encodedTextWriter.WriteToStream("</body>\r\n</html>");
			dsnBodyStream.Flush();
		}

		public bool TryGetCustomizedQuotaExplanation(CultureInfo culture, QuotaMessageType quotaToGet, out string explanation)
		{
			explanation = null;
			string name;
			if (culture.IsNeutralCulture || MessageLanguageParser.IsCultureSupportedForCustomization(culture))
			{
				name = culture.Name;
			}
			else
			{
				name = culture.Parent.Name;
			}
			return this.configuredStatusCodes.TryGetValue(name + quotaToGet.ToString(), out explanation);
		}

		public string GetNDRHumanReadableExplanation(CultureInfo culture, string enhancedStatusCode, bool isInitMsg, out bool isCustomized)
		{
			isCustomized = false;
			if (isInitMsg)
			{
				return SystemMessages.HumanText_InitMsg.ToString(culture);
			}
			string name;
			if (culture.IsNeutralCulture || MessageLanguageParser.IsCultureSupportedForCustomization(culture))
			{
				name = culture.Name;
			}
			else
			{
				name = culture.Parent.Name;
			}
			if (string.IsNullOrEmpty(enhancedStatusCode))
			{
				string result;
				if (this.TryGetConfiguredOrDefaultExplanation(name, "5.0.0", culture, out result, ref isCustomized))
				{
					return result;
				}
				throw new InvalidOperationException("Cannot find string for 5.0.0");
			}
			else
			{
				string result;
				if (this.TryGetConfiguredOrDefaultExplanation(name, enhancedStatusCode, culture, out result, ref isCustomized))
				{
					return result;
				}
				if (enhancedStatusCode[0] != '5')
				{
					enhancedStatusCode = "5" + enhancedStatusCode.Substring(1);
					if (this.TryGetConfiguredOrDefaultExplanation(name, enhancedStatusCode, culture, out result, ref isCustomized))
					{
						return result;
					}
				}
				int num = enhancedStatusCode.LastIndexOf('.');
				if (enhancedStatusCode[num + 1] != '0' || num + 2 != enhancedStatusCode.Length)
				{
					string enhancedStatusCode2 = enhancedStatusCode.Substring(0, num + 1) + "0";
					if (this.TryGetConfiguredOrDefaultExplanation(name, enhancedStatusCode2, culture, out result, ref isCustomized))
					{
						return result;
					}
				}
				if (this.TryGetConfiguredOrDefaultExplanation(name, "5.0.0", culture, out result, ref isCustomized))
				{
					return result;
				}
				throw new InvalidOperationException("Cannot find string for 5.0.0");
			}
		}

		public QuotaInformation GetQuotaInformation(QuotaType quotaType, int? localeId, int currentSize, int? maxSize, string folderName, bool isPrimaryMailbox, bool isPublicFolderMailbox)
		{
			CultureInfo cultureInfo = null;
			if (localeId != null)
			{
				try
				{
					cultureInfo = Culture.GetCulture(localeId.Value).GetCultureInfo();
				}
				catch (ArgumentException arg)
				{
					cultureInfo = null;
					ExTraceGlobals.DsnTracer.TraceDebug<int?, ArgumentException>(0L, "Ignoring invalid loacale id: {0}. Exception is {1}", localeId, arg);
				}
			}
			if (cultureInfo == null || !MessageLanguageParser.IsCultureSupported(cultureInfo))
			{
				cultureInfo = this.DefaultCulture;
			}
			bool flag = maxSize != null;
			QuotaMessageType quotaMessageType = DsnHumanReadableWriter.GetQuotaMessageType(quotaType, flag, isPublicFolderMailbox);
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			string text = null;
			string text2 = null;
			float percentFull = 0f;
			string maxSizeText = null;
			string text3 = null;
			if (flag)
			{
				text = SystemMessages.QuotaCurrentSize.ToString(cultureInfo);
				text2 = SystemMessages.QuotaMaxSize.ToString(cultureInfo);
				if (maxSize.Value == 0)
				{
					percentFull = 1f;
				}
				else
				{
					percentFull = (float)currentSize / (float)maxSize.Value;
				}
			}
			DsnHumanReadableWriter.GetCurrentSizeText(quotaType, currentSize, maxSize, cultureInfo, out text3, out maxSizeText);
			if (flag)
			{
				outboundCodePageDetector.AddText(text);
				outboundCodePageDetector.AddText(text2);
			}
			QuotaLocalizedTexts quotaLocalizedTexts = QuotaLocalizedTexts.GetQuotaLocalizedTexts(quotaMessageType, folderName, text3, isPrimaryMailbox);
			string text4 = quotaLocalizedTexts.Subject.ToString(cultureInfo);
			string text5 = quotaLocalizedTexts.TopText.ToString(cultureInfo);
			string text6;
			if (!isPrimaryMailbox || !this.TryGetCustomizedQuotaExplanation(cultureInfo, quotaMessageType, out text6))
			{
				text6 = quotaLocalizedTexts.Details.ToString(cultureInfo);
			}
			string text7 = SystemMessages.BodyHeaderFontTag.ToString(cultureInfo);
			string text8 = SystemMessages.BodyBlockFontTag.ToString(cultureInfo);
			outboundCodePageDetector.AddText(text4);
			outboundCodePageDetector.AddText(text5);
			outboundCodePageDetector.AddText(text7);
			outboundCodePageDetector.AddText(text6);
			outboundCodePageDetector.AddText(text8);
			Charset charset = DsnHumanReadableWriter.GetCharset(outboundCodePageDetector, cultureInfo);
			return new QuotaInformation(charset, cultureInfo, text4, text5, text6, string.Empty, text7, string.Empty, text8, text, text3, text2, maxSizeText, flag, DsnHumanReadableWriter.IsWarningMessage(quotaMessageType), percentFull);
		}

		private static bool IsWarningMessage(QuotaMessageType quotaMessageType)
		{
			return quotaMessageType == QuotaMessageType.WarningMailbox || quotaMessageType == QuotaMessageType.WarningMailboxUnlimitedSize || quotaMessageType == QuotaMessageType.WarningPublicFolder || quotaMessageType == QuotaMessageType.WarningPublicFolderUnlimitedSize || quotaMessageType == QuotaMessageType.WarningMailboxMessagesPerFolderCount || quotaMessageType == QuotaMessageType.WarningMailboxMessagesPerFolderUnlimitedCount || quotaMessageType == QuotaMessageType.WarningFolderHierarchyChildrenCount || quotaMessageType == QuotaMessageType.WarningFolderHierarchyChildrenUnlimitedCount || quotaMessageType == QuotaMessageType.WarningFolderHierarchyDepth || quotaMessageType == QuotaMessageType.WarningFolderHierarchyDepthUnlimited || quotaMessageType == QuotaMessageType.WarningFoldersCount || quotaMessageType == QuotaMessageType.WarningFoldersCountUnlimited;
		}

		private static void GetCurrentSizeText(QuotaType quotaType, int currentSize, int? maxSize, CultureInfo culture, out string currentSizeText, out string maxSizeText)
		{
			currentSizeText = null;
			maxSizeText = null;
			bool flag = maxSize != null;
			switch (quotaType)
			{
			case QuotaType.StorageWarningLimit:
			case QuotaType.StorageOverQuotaLimit:
			case QuotaType.StorageShutoff:
				currentSize >>= 10;
				if (flag)
				{
					maxSize >>= 10;
				}
				break;
			}
			currentSizeText = (culture.IsNeutralCulture ? currentSize.ToString() : currentSize.ToString(culture));
			if (flag)
			{
				maxSizeText = (culture.IsNeutralCulture ? maxSize.Value.ToString() : maxSize.Value.ToString(culture));
			}
			switch (quotaType)
			{
			case QuotaType.StorageWarningLimit:
			case QuotaType.StorageOverQuotaLimit:
			case QuotaType.StorageShutoff:
				currentSizeText = SystemMessages.SizeInMB(currentSizeText).ToString(culture);
				if (flag)
				{
					maxSizeText = SystemMessages.SizeInMB(maxSizeText).ToString(culture);
					return;
				}
				return;
			case QuotaType.MailboxMessagesPerFolderCountWarningQuota:
			case QuotaType.MailboxMessagesPerFolderCountReceiveQuota:
				currentSizeText = SystemMessages.SizeInMessages(currentSizeText).ToString(culture);
				if (flag)
				{
					maxSizeText = SystemMessages.SizeInMessages(maxSizeText).ToString(culture);
					return;
				}
				return;
			case QuotaType.FolderHierarchyChildrenCountWarningQuota:
			case QuotaType.FolderHierarchyChildrenCountReceiveQuota:
			case QuotaType.FolderHierarchyDepthWarningQuota:
			case QuotaType.FolderHierarchyDepthReceiveQuota:
			case QuotaType.FoldersCountWarningQuota:
			case QuotaType.FoldersCountReceiveQuota:
				currentSizeText = SystemMessages.SizeInFolders(currentSizeText).ToString(culture);
				if (flag)
				{
					maxSizeText = SystemMessages.SizeInFolders(maxSizeText).ToString(culture);
					return;
				}
				return;
			}
			throw new NotSupportedException("quotaType invalid");
		}

		public void WriteHtmlQuotaBody(Stream quotaBodyStream, QuotaInformation info)
		{
			DsnHumanReadableWriter.EncodedTextWriter encodedTextWriter = new DsnHumanReadableWriter.EncodedTextWriter(quotaBodyStream, info.MessageCharset.GetEncoding());
			encodedTextWriter.WriteToStream("<html>\r\n<Head></head>");
			DsnHumanReadableWriter.WriteTopBlock(encodedTextWriter, info.TopText, info.TopTextFont);
			encodedTextWriter.WriteToStream(info.BodyTextFont);
			if (info.HasMaxSize)
			{
				DsnHumanReadableWriter.WriteQuotaBar(encodedTextWriter, info.IsWarning, info.PercentFull, info.CurrentSizeText, info.MaxSizeText, info.CurrentSizeTitle, info.MaxSizeTitle, info.BodyTextFont);
			}
			DsnHumanReadableWriter.WriteBottomBlock(encodedTextWriter, new string[]
			{
				info.Details
			}, false);
			encodedTextWriter.WriteToStream("</font>\r\n");
			if (!string.IsNullOrEmpty(info.FinalText) && !string.IsNullOrEmpty(info.FinalTextFont))
			{
				DsnHumanReadableWriter.WriteFinalBlock(encodedTextWriter, info.FinalText, info.FinalTextFont);
			}
			encodedTextWriter.WriteToStream("</body>\r\n</html>");
		}

		public void WriteTextQuotaBody(Stream quotaBodyStream, QuotaInformation info)
		{
			Encoding encoding = info.MessageCharset.GetEncoding();
			DsnHumanReadableWriter.EncodedTextWriter encodedTextWriter = new DsnHumanReadableWriter.EncodedTextWriter(quotaBodyStream, encoding);
			HtmlToText htmlToText = new HtmlToText();
			htmlToText.OutputEncoding = encoding;
			htmlToText.Convert(new StringReader(info.TopText), quotaBodyStream);
			encodedTextWriter.WriteToStream("\r\n");
			if (info.HasMaxSize)
			{
				encodedTextWriter.WriteToStream(string.Format("{0} {1}\r\n", info.CurrentSizeText, info.CurrentSizeTitle));
				encodedTextWriter.WriteToStream(string.Format("{0} {1}\r\n", info.MaxSizeText, info.MaxSizeTitle));
			}
			htmlToText.Convert(new StringReader(info.Details), quotaBodyStream);
			encodedTextWriter.WriteToStream("\r\n\r\n");
			encodedTextWriter.WriteToStream(info.FinalText);
		}

		public ApprovalInformation GetMessageInModerationExpiredNdrOofInformation(ApprovalInformation.ApprovalNotificationType notificationType, string subject, ICollection<string> moderatedRecipientDisplayNames, string originalMessageId, string acceptLanguageValue)
		{
			if (notificationType != ApprovalInformation.ApprovalNotificationType.ExpiryNotification && notificationType != ApprovalInformation.ApprovalNotificationType.ModeratorsNdrNotification && notificationType != ApprovalInformation.ApprovalNotificationType.ModeratorsOofNotification)
			{
				throw new ArgumentException("Notification type not supported by this method");
			}
			if (moderatedRecipientDisplayNames == null)
			{
				throw new ArgumentNullException("moderatedRecipientDisplayNames");
			}
			return this.GetApprovalInformation(notificationType, subject, null, null, null, moderatedRecipientDisplayNames, null, null, null, acceptLanguageValue, null, null, null, originalMessageId, false);
		}

		public ApprovalInformation GetMessageInModerationModeratorExpiredInformation(ApprovalInformation.ApprovalNotificationType notificationType, string subject, ICollection<string> moderatedRecipientDisplayNames, string sender, int? retentionPeriod, string acceptLanguageValue, CultureInfo defaultFallBackCulture)
		{
			if (notificationType != ApprovalInformation.ApprovalNotificationType.ModeratorExpiryNotification)
			{
				throw new ArgumentException("Notification type not supported by this method");
			}
			if (moderatedRecipientDisplayNames == null)
			{
				throw new ArgumentNullException("moderatedRecipientDisplayNames");
			}
			if (moderatedRecipientDisplayNames.Count == 0)
			{
				throw new ArgumentException("There must be at least 1 moderated recipient");
			}
			return this.GetApprovalInformation(notificationType, subject, sender, null, null, moderatedRecipientDisplayNames, null, null, null, acceptLanguageValue, null, defaultFallBackCulture, retentionPeriod, null, false);
		}

		public ApprovalInformation GetModerateRejectInformation(string subject, ICollection<string> moderatedRecipientDisplayNames, bool hasComments, string originalMessageId, string acceptLanguageValue)
		{
			return this.GetApprovalInformation(hasComments ? ApprovalInformation.ApprovalNotificationType.ModeratedTransportRejectWithComments : ApprovalInformation.ApprovalNotificationType.ModeratedTransportReject, subject, null, null, null, moderatedRecipientDisplayNames, null, null, null, acceptLanguageValue, null, null, null, originalMessageId, false);
		}

		public ApprovalInformation GetDecisionConflictInformation(string subject, string decisionMaker, bool? decision, ExDateTime? decisionTime, Header acceptLanguageHeader, Header contentLanguageHeader, CultureInfo defaultCulture)
		{
			return this.GetApprovalInformation(ApprovalInformation.ApprovalNotificationType.DecisionConflict, subject, null, null, null, null, decisionMaker, decision, decisionTime, MessageLanguageParser.GetLanguageHeaderValue(acceptLanguageHeader), MessageLanguageParser.GetLanguageHeaderValue(contentLanguageHeader), defaultCulture, null, null, false);
		}

		public ApprovalInformation GetDecisionUpdateInformation(string subject, string decisionMaker, bool? decision, ExDateTime? decisionTime)
		{
			if (string.IsNullOrEmpty(decisionMaker))
			{
				throw new ArgumentException("decisionMaker must be present");
			}
			if (decision == null)
			{
				throw new ArgumentException("decision must be present");
			}
			if (decisionTime == null)
			{
				throw new ArgumentException("decisionTime must be present");
			}
			return this.GetApprovalInformation(ApprovalInformation.ApprovalNotificationType.DecisionUpdate, subject, null, null, null, null, decisionMaker, decision, decisionTime, null, null, null, null, null, false);
		}

		public ApprovalInformation GetApprovalRequestExpiryInformation(string subject)
		{
			return this.GetApprovalInformation(ApprovalInformation.ApprovalNotificationType.ApprovalRequestExpiry, subject, null, null, null, null, null, null, null, null, null, null, null, null, false);
		}

		public ApprovalInformation GetApprovalRequestMessageInformation(string subject, string sender, string toRecipients, string ccRecipients, ICollection<string> moderatedRecipients, bool showApprovalRequestPreview, CultureInfo cultureInfo)
		{
			return this.GetApprovalInformation(ApprovalInformation.ApprovalNotificationType.ApprovalRequest, subject, sender, toRecipients, ccRecipients, moderatedRecipients, null, null, null, null, null, cultureInfo, null, null, showApprovalRequestPreview);
		}

		public void WriteHtmlModerationBody(Stream bodyStream, ApprovalInformation info)
		{
			DsnHumanReadableWriter.EncodedTextWriter encodedTextWriter = new DsnHumanReadableWriter.EncodedTextWriter(bodyStream, info.MessageCharset.GetEncoding());
			encodedTextWriter.WriteToStream("<html>\r\n<Head></head>");
			DsnHumanReadableWriter.WriteTopBlock(encodedTextWriter, info.TopText, info.TopTextFont);
			encodedTextWriter.WriteToStream(info.BodyTextFont);
			DsnHumanReadableWriter.WriteBottomBlock(encodedTextWriter, info.Details, false);
			if (!string.IsNullOrEmpty(info.FinalText))
			{
				DsnHumanReadableWriter.WriteFinalBlock(encodedTextWriter, info.FinalText, info.FinalTextFont);
			}
			encodedTextWriter.WriteToStream("</font>\r\n");
			encodedTextWriter.WriteToStream("</body>\r\n</html>");
		}

		public string GetModeratorsCommentFileName(CultureInfo cultureInfo)
		{
			return SystemMessages.ApprovalCommentAttachmentFilename.ToString(cultureInfo);
		}

		public string GetHtmlModerationBody(ApprovalInformation info)
		{
			int num = "<html>\r\n<Head></head>".Length + "<body>\r\n<p><b>".Length + info.TopTextFont.Length + info.TopText.Length + "</font></b></p>\r\n".Length + info.BodyTextFont.Length + info.BodyTextFont.Length + "<HR/>".Length + info.FinalTextFont.Length + info.FinalTextFont.Length + info.FinalText.Length + "</font>\r\n".Length + "</body>\r\n</html>".Length;
			foreach (string text in info.Details)
			{
				if (!string.IsNullOrEmpty(text))
				{
					num += text.Length + "<p>".Length + "</p>\r\n".Length;
				}
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append("<html>\r\n<Head></head>");
			stringBuilder.Append("<body>\r\n<p><b>");
			stringBuilder.Append(info.TopTextFont);
			stringBuilder.Append(info.TopText);
			stringBuilder.Append("</font></b></p>\r\n");
			stringBuilder.Append(info.BodyTextFont);
			foreach (string value in info.Details)
			{
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.Append("<p>");
					stringBuilder.Append(value);
					stringBuilder.Append("</p>\r\n");
				}
			}
			if (!string.IsNullOrEmpty(info.FinalText))
			{
				stringBuilder.Append("<HR/>");
				stringBuilder.Append(info.FinalTextFont);
				stringBuilder.Append(info.FinalText);
				stringBuilder.Append("</font>\r\n");
			}
			stringBuilder.Append("</body>\r\n</html>");
			return stringBuilder.ToString();
		}

		public CultureInfo GetDsnCulture(Header acceptLanguageHeader, Header contentLanguageHeader, bool enableLanguageDetection, CultureInfo configuredDefaultCulture)
		{
			return MessageLanguageParser.GetCulture(acceptLanguageHeader, contentLanguageHeader, enableLanguageDetection, configuredDefaultCulture);
		}

		public CultureInfo FindLanguage(string languages, bool useQValue)
		{
			return MessageLanguageParser.FindLanguage(languages, useQValue);
		}

		private static string HtmlEscape(TextToText t2tConverter, string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length);
			using (StringReader stringReader = new StringReader(input))
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					try
					{
						t2tConverter.Convert(stringReader, stringWriter);
					}
					catch (ExchangeDataException arg)
					{
						ExTraceGlobals.DsnTracer.TraceDebug<ExchangeDataException, string>(0L, "HtmlEscape caught {0} on {1}", arg, input);
						return string.Empty;
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetRecipientString(ICollection<string> recipientlist, CultureInfo culture)
		{
			if (recipientlist == null || recipientlist.Count == 0)
			{
				return string.Empty;
			}
			if (recipientlist.Count <= 5)
			{
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder(recipientlist.Count * 30);
				foreach (string value in recipientlist)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append("; ");
					}
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString();
			}
			int num = (recipientlist.Count > 50) ? 40 : recipientlist.Count;
			int num2 = 0;
			StringBuilder stringBuilder2 = new StringBuilder(num * 20);
			foreach (string text in recipientlist)
			{
				if (num2 == 0)
				{
					stringBuilder2.Append(text);
				}
				else
				{
					stringBuilder2.AppendFormat("; {0}", text);
				}
				num2++;
				if (num2 == num)
				{
					break;
				}
			}
			if (recipientlist.Count != num)
			{
				stringBuilder2.AppendFormat("; {0}", SystemMessages.ModeratedDLApprovalRequestRecipientList(recipientlist.Count - num).ToString(culture));
			}
			return stringBuilder2.ToString();
		}

		private static IEnumerable<string> GetModeratedDLApprovalRequestDetails(TextToText t2tConverter, string sender, string escapedSubject, string toRecipients, string ccRecipients, ICollection<string> moderatedRecipientsDisplayName, CultureInfo culture, bool showApprovalRequestPreview)
		{
			sender = DsnHumanReadableWriter.HtmlEscape(t2tConverter, sender);
			toRecipients = DsnHumanReadableWriter.HtmlEscape(t2tConverter, toRecipients);
			ccRecipients = DsnHumanReadableWriter.HtmlEscape(t2tConverter, ccRecipients);
			List<string> list = new List<string>(12);
			list.Add(SystemMessages.ModeratedDLApprovalRequest(sender).ToString(culture));
			string recipientString = DsnHumanReadableWriter.GetRecipientString(moderatedRecipientsDisplayName, culture);
			list.Add(DsnHumanReadableWriter.HtmlEscape(t2tConverter, recipientString));
			list.Add("<br>\r\n");
			if (showApprovalRequestPreview)
			{
				list.Add("<HR>");
				list.Add(SystemMessages.ApprovalRequestPreview.ToString(culture));
				list.Add("<HR>");
				list.Add(SystemMessages.HumanReadableBoldedFromLine(sender).ToString(culture));
				list.Add(SystemMessages.HumanReadableBoldedToLine(toRecipients).ToString(culture));
				if (!string.IsNullOrEmpty(ccRecipients))
				{
					list.Add(SystemMessages.HumanReadableBoldedCcLine(ccRecipients).ToString(culture));
				}
				list.Add(SystemMessages.HumanReadableBoldedSubjectLine(escapedSubject).ToString(culture));
				list.Add("<br>\r\n");
			}
			return list;
		}

		private static QuotaMessageType GetQuotaMessageType(QuotaType quotaType, bool hasMax, bool isPublicFolder)
		{
			switch (quotaType)
			{
			case QuotaType.StorageWarningLimit:
				if (!hasMax)
				{
					if (!isPublicFolder)
					{
						return QuotaMessageType.WarningMailboxUnlimitedSize;
					}
					return QuotaMessageType.WarningPublicFolderUnlimitedSize;
				}
				else
				{
					if (!isPublicFolder)
					{
						return QuotaMessageType.WarningMailbox;
					}
					return QuotaMessageType.WarningPublicFolder;
				}
				break;
			case QuotaType.StorageOverQuotaLimit:
				if (!isPublicFolder)
				{
					return QuotaMessageType.ProhibitSendMailbox;
				}
				return QuotaMessageType.ProhibitPostPublicFolder;
			case QuotaType.StorageShutoff:
				return QuotaMessageType.ProhibitSendReceiveMailBox;
			case QuotaType.MailboxMessagesPerFolderCountWarningQuota:
				if (!hasMax)
				{
					return QuotaMessageType.WarningMailboxMessagesPerFolderUnlimitedCount;
				}
				return QuotaMessageType.WarningMailboxMessagesPerFolderCount;
			case QuotaType.MailboxMessagesPerFolderCountReceiveQuota:
				return QuotaMessageType.ProhibitReceiveMailboxMessagesPerFolderCount;
			case QuotaType.FolderHierarchyChildrenCountWarningQuota:
				if (!hasMax)
				{
					return QuotaMessageType.WarningFolderHierarchyChildrenUnlimitedCount;
				}
				return QuotaMessageType.WarningFolderHierarchyChildrenCount;
			case QuotaType.FolderHierarchyChildrenCountReceiveQuota:
				return QuotaMessageType.ProhibitReceiveFolderHierarchyChildrenCountCount;
			case QuotaType.FolderHierarchyDepthWarningQuota:
				if (!hasMax)
				{
					return QuotaMessageType.WarningFolderHierarchyDepthUnlimited;
				}
				return QuotaMessageType.WarningFolderHierarchyDepth;
			case QuotaType.FolderHierarchyDepthReceiveQuota:
				return QuotaMessageType.ProhibitReceiveFolderHierarchyDepth;
			case QuotaType.FoldersCountWarningQuota:
				if (!hasMax)
				{
					return QuotaMessageType.WarningFoldersCountUnlimited;
				}
				return QuotaMessageType.WarningFoldersCount;
			case QuotaType.FoldersCountReceiveQuota:
				return QuotaMessageType.ProhibitReceiveFoldersCount;
			}
			throw new InvalidOperationException("Unexpected quota type type.");
		}

		private static IEnumerable<string> GetNotificationDetails(TextToText t2tConverter, string escapedSubject, ICollection<string> moderatedRecipientsDisplayName, CultureInfo culture)
		{
			List<string> list = new List<string>(4);
			string recipientString = DsnHumanReadableWriter.GetRecipientString(moderatedRecipientsDisplayName, culture);
			list.Add(DsnHumanReadableWriter.HtmlEscape(t2tConverter, recipientString));
			list.Add(SystemMessages.HumanReadableSubjectLine(escapedSubject).ToString(culture));
			return list;
		}

		private static string[] GetBottomTexts(DsnFlags dsnFlag, string originalSubject, string remoteMta, DateTime? expiryTime, CultureInfo cultureInfo)
		{
			string[] array = null;
			if (string.IsNullOrEmpty(originalSubject))
			{
				originalSubject = string.Empty;
			}
			if (dsnFlag == DsnFlags.Failure && !string.IsNullOrEmpty(remoteMta))
			{
				array = new string[]
				{
					SystemMessages.HumanReadableRemoteServerText(remoteMta).ToString(cultureInfo)
				};
			}
			else if (dsnFlag == DsnFlags.Delay)
			{
				if (expiryTime != null)
				{
					TimeSpan t = expiryTime.Value.Subtract(DateTime.UtcNow.ToLocalTime());
					if (t > TimeSpan.Zero)
					{
						array = new string[3];
						if (t.Days > 0)
						{
							array[2] = SystemMessages.DelayedHumanReadableBottomText(t.Days, t.Hours, t.Minutes).ToString(MessageLanguageParser.GetCultureToUse(cultureInfo));
						}
						else
						{
							array[2] = SystemMessages.DelayedHumanReadableBottomTextHours(t.Hours, t.Minutes).ToString(MessageLanguageParser.GetCultureToUse(cultureInfo));
						}
					}
					else
					{
						array = new string[2];
					}
				}
				else
				{
					array = new string[2];
				}
				array[0] = SystemMessages.HumanReadableSubjectLine(originalSubject).ToString(cultureInfo);
				array[1] = SystemMessages.DelayHumanReadableExplanation.ToString(cultureInfo);
			}
			else if (dsnFlag != DsnFlags.Failure)
			{
				array = new string[]
				{
					SystemMessages.HumanReadableSubjectLine(originalSubject).ToString(cultureInfo)
				};
			}
			return array;
		}

		private static bool TryGetTimeString(DateTime? time, CultureInfo cultureInfo, out string timeText)
		{
			timeText = null;
			if (time != null)
			{
				try
				{
					timeText = time.Value.ToString(MessageLanguageParser.GetCultureToUse(cultureInfo));
					return true;
				}
				catch (ArgumentOutOfRangeException arg)
				{
					ExTraceGlobals.DsnTracer.TraceError<long, ArgumentOutOfRangeException>(0L, "Cannot turn expiry time '{0}' to string. exception: {1}", time.Value.Ticks, arg);
				}
				return false;
			}
			return false;
		}

		private static Charset GetCharset(OutboundCodePageDetector detector, CultureInfo cultureInfo)
		{
			Culture culture;
			int codePage;
			if (Culture.TryGetCulture(cultureInfo.Name, out culture))
			{
				codePage = detector.GetCodePage(culture, false);
			}
			else
			{
				codePage = detector.GetCodePage();
			}
			Charset charset;
			if (Charset.TryGetCharset(codePage, out charset) && charset.IsAvailable)
			{
				return charset;
			}
			return Charset.GetCharset(65001);
		}

		private static void WriteQuotaBar(DsnHumanReadableWriter.EncodedTextWriter writer, bool isWarning, float percentFull, string currentSize, string maxSize, string currentSizeTitle, string maxSizeTitle, string bodyfontTags)
		{
			if (percentFull < 0.1f)
			{
				percentFull = 0.1f;
			}
			int num = Math.Min(200, (int)(200f * percentFull));
			int num2 = 200 - num;
			if (num2 > 4)
			{
				writer.WriteToStream(string.Format("<table cellspacing=0;><tr><td style=\"background-color:{0};width:{1};border-left-style:solid;border-top-style:solid;border-bottom-style:solid;border-color:black;border-width:1\">{7}{2}</font></td><td style=\"background-color:#ffffff;width:{3};border-right-style:solid;border-top-style:solid;border-bottom-style:solid;border-color:black;border-width:1\">&nbsp</td><td>{7}<b>{4}</b></font></td></tr><tr><td style=\"width:{1}\"><font color=\"#ffffff\" size=\"0\">{5}</font></td><td style=\"width:{3}\">&nbsp</td><td><font color=\"#ffffff\" size=\"0\">{6}</font></td></tr></table>", new object[]
				{
					isWarning ? "#FFCC00" : "#FF0000",
					num,
					currentSize,
					num2,
					maxSize,
					currentSizeTitle,
					maxSizeTitle,
					bodyfontTags
				}));
				return;
			}
			writer.WriteToStream(string.Format("<table cellspacing=0;><tr><td style=\"background-color:{0};width:200;border-style:solid;border-color:black;border-width:1\">{5}{1}</font></td><td>{5}<b>{2}</b></font></td></tr><tr><td style=\"width:200\"><font color=\"#ffffff\" size=\"0\">{3}</font></td><td><font color=\"#ffffff\" size=\"0\">{4}</font></td></tr></table>", new object[]
			{
				isWarning ? "#FFCC00" : "#FF0000",
				currentSize,
				maxSize,
				currentSizeTitle,
				maxSizeTitle,
				bodyfontTags
			}));
		}

		private static void WriteTopBlock(DsnHumanReadableWriter.EncodedTextWriter writer, string topText, string bodyTopTextFont)
		{
			writer.WriteToStream("<body>\r\n<p><b>");
			writer.WriteToStream(bodyTopTextFont);
			writer.WriteToStream(topText);
			writer.WriteToStream("</font></b></p>\r\n");
		}

		private static void WriteFinalBlock(DsnHumanReadableWriter.EncodedTextWriter writer, string finalText, string finalTextFont)
		{
			writer.WriteToStream("<HR/>");
			writer.WriteToStream(finalTextFont);
			writer.WriteToStream(finalText);
			writer.WriteToStream("</font>\r\n");
		}

		private static void WriteRecipientsBlock(DsnHumanReadableWriter.EncodedTextWriter writer, DsnFlags dsnFlag, List<DsnRecipientInfo> recipientList, string remoteMta, string bodyTextFont, string enhancedDsnTextFont, string externalErrorText, string externalNoDetailText, bool isExternal)
		{
			if (recipientList.Count > 0)
			{
				List<DsnRecipientInfo> list = new List<DsnRecipientInfo>(recipientList.Count);
				using (List<DsnRecipientInfo>.Enumerator enumerator = recipientList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DsnRecipientInfo dsnRecipientInfo = enumerator.Current;
						if (dsnRecipientInfo.ModeratedRecipients != null && dsnRecipientInfo.ModeratedRecipients.Count > 0)
						{
							using (EmailRecipientCollection.Enumerator enumerator2 = dsnRecipientInfo.ModeratedRecipients.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									EmailRecipient emailRecipient = enumerator2.Current;
									DsnRecipientInfo item = new DsnRecipientInfo(emailRecipient.DisplayName, emailRecipient.NativeAddress, emailRecipient.NativeAddressType, dsnRecipientInfo.EnhancedStatusCode, dsnRecipientInfo.StatusText, dsnRecipientInfo.ORecip, dsnRecipientInfo.DsnHumanReadableExplanation, dsnRecipientInfo.DsnRecipientExplanation, null, null);
									list.Add(item);
								}
								continue;
							}
						}
						list.Add(dsnRecipientInfo);
					}
					goto IL_D6;
				}
				return;
				IL_D6:
				for (int i = 0; i < list.Count; i++)
				{
					DsnHumanReadableWriter.WriteEachRecipient(writer, dsnFlag, list[i], remoteMta, bodyTextFont, enhancedDsnTextFont, externalErrorText, externalNoDetailText, isExternal);
				}
				return;
			}
		}

		private static void WriteEachRecipient(DsnHumanReadableWriter.EncodedTextWriter writer, DsnFlags dsnFlag, DsnRecipientInfo recipientInfo, string remoteMta, string bodyTextFont, string enhancedDsnTextFont, string externalErrorText, string externalNoDetailText, bool isExternal)
		{
			writer.WriteToStream("<p><a href=\"mailto:");
			try
			{
				string text = Uri.EscapeDataString(recipientInfo.Address);
				writer.WriteToStream(text.Replace("%40", "@"));
			}
			catch (UriFormatException)
			{
			}
			writer.WriteToStream("\">");
			string displayName = recipientInfo.DisplayName;
			if (string.IsNullOrEmpty(displayName))
			{
				if (!string.IsNullOrEmpty(recipientInfo.DecodedAddress))
				{
					writer.EscapeAndWriteToStream(recipientInfo.DecodedAddress);
				}
				else
				{
					writer.EscapeAndWriteToStream(recipientInfo.Address);
				}
			}
			else if (string.IsNullOrEmpty(recipientInfo.DecodedAddress))
			{
				writer.EscapeAndWriteToStream(displayName);
			}
			else
			{
				writer.EscapeAndWriteToStream(displayName + " (" + recipientInfo.DecodedAddress + ")");
			}
			writer.WriteToStream("</a>");
			if (dsnFlag == DsnFlags.Failure && !recipientInfo.OverwriteDefault)
			{
				writer.WriteToStream("<br>\r\n");
				string text2 = null;
				if (!string.IsNullOrEmpty(recipientInfo.NdrEnhancedText))
				{
					text2 = recipientInfo.NdrEnhancedText;
				}
				else if (!string.IsNullOrEmpty(recipientInfo.DsnHumanReadableExplanation))
				{
					text2 = recipientInfo.DsnHumanReadableExplanation;
				}
				if (!string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(recipientInfo.DsnRecipientExplanation))
				{
					writer.WriteToStream("</font>\r\n");
					writer.WriteToStream(enhancedDsnTextFont);
					if (!string.IsNullOrEmpty(recipientInfo.DsnRecipientExplanation))
					{
						writer.EscapeAndWriteToStream(recipientInfo.DsnRecipientExplanation);
						writer.WriteToStream("<br>\r\n<br>\r\n");
					}
					if (!string.IsNullOrEmpty(text2))
					{
						writer.WriteToStream(text2);
						writer.WriteToStream("<br>\r\n");
					}
					writer.WriteToStream("</font>\r\n");
					writer.WriteToStream(bodyTextFont);
					writer.WriteToStream("<br>\r\n");
				}
				if (isExternal && !string.IsNullOrEmpty(remoteMta) && !string.IsNullOrEmpty(externalErrorText))
				{
					writer.WriteToStream("\r\n<p><b>");
					string[] array;
					if (SmtpResponseParser.Split(recipientInfo.StatusText, out array) && array.Length >= 3)
					{
						writer.EscapeAndWriteToStream(externalErrorText);
						writer.WriteToStream("<br>\r\n");
						for (int i = 2; i < array.Length; i++)
						{
							writer.EscapeAndWriteToStream(array[i] + " ");
						}
					}
					else
					{
						writer.EscapeAndWriteToStream(externalNoDetailText);
					}
					writer.WriteToStream("<br>\r\n");
					writer.WriteToStream("</b></p>\r\n");
				}
			}
			if (recipientInfo.DsnParamTexts != null && recipientInfo.DsnParamTexts.Length > 0)
			{
				foreach (string text3 in recipientInfo.DsnParamTexts)
				{
					writer.WriteToStream("<br>\r\n");
					writer.WriteToStream(text3);
				}
				writer.WriteToStream("<br>\r\n");
				writer.WriteToStream("<br>\r\n");
			}
			writer.WriteToStream("</p>\r\n");
		}

		private static void WriteBottomBlock(DsnHumanReadableWriter.EncodedTextWriter textWriter, IEnumerable<string> bottomTexts, bool escape)
		{
			if (bottomTexts != null)
			{
				foreach (string text in bottomTexts)
				{
					if (!string.IsNullOrEmpty(text))
					{
						textWriter.WriteToStream("<p>");
						if (escape)
						{
							textWriter.EscapeAndWriteToStream(text);
						}
						else
						{
							textWriter.WriteToStream(text);
						}
						textWriter.WriteToStream("</p>\r\n");
					}
				}
			}
		}

		private static string GetFormattedRetryCountString(DsnRecipientInfo recipientInfo)
		{
			if (recipientInfo == null)
			{
				return string.Empty;
			}
			return string.Format("Total retry attempts: {0}", recipientInfo.RetryCount);
		}

		private static string GetFormattedSmtpResponse(DsnRecipientInfo recipientInfo)
		{
			StringBuilder stringBuilder = new StringBuilder("Remote Server  ");
			if (!string.IsNullOrEmpty(recipientInfo.ReceivingServerDetails))
			{
				stringBuilder.AppendFormat("at {0} ", recipientInfo.ReceivingServerDetails);
			}
			stringBuilder.AppendFormat("returned '{0}'", recipientInfo.StatusText);
			return stringBuilder.ToString();
		}

		private ApprovalInformation GetApprovalInformation(ApprovalInformation.ApprovalNotificationType notificationType, string subject, string sender, string toRecipients, string ccRecipients, ICollection<string> moderatedRecipientsDisplayName, string decisionMaker, bool? decision, ExDateTime? decisionTime, string acceptLanguageValue, string contentLanguageValue, CultureInfo fallbackCulture, int? retentionPeriod, string originalMessageId, bool showApprovalRequestPreview)
		{
			CultureInfo cultureFromLanguageHeaderValues = MessageLanguageParser.GetCultureFromLanguageHeaderValues(acceptLanguageValue, contentLanguageValue, true, fallbackCulture);
			if (string.IsNullOrEmpty(subject))
			{
				subject = string.Empty;
			}
			string text = SystemMessages.BodyHeaderFontTag.ToString(cultureFromLanguageHeaderValues);
			string text2 = SystemMessages.FinalTextFontTag.ToString(cultureFromLanguageHeaderValues);
			string text3 = SystemMessages.BodyBlockFontTag.ToString(cultureFromLanguageHeaderValues);
			string text4;
			if (decision != null)
			{
				if (decision.Value)
				{
					text4 = SystemMessages.ApproveButtonText.ToString(cultureFromLanguageHeaderValues);
				}
				else
				{
					text4 = SystemMessages.RejectButtonText.ToString(cultureFromLanguageHeaderValues);
				}
			}
			else
			{
				text4 = string.Empty;
			}
			TextToText textToText = new TextToText();
			textToText.HtmlEscapeOutput = true;
			string text5 = DsnHumanReadableWriter.HtmlEscape(textToText, subject);
			string text6 = DsnHumanReadableWriter.GetLocalizedApprovalPrefix(notificationType).ToString(cultureFromLanguageHeaderValues) + subject;
			string text7;
			IEnumerable<string> enumerable;
			string text9;
			switch (notificationType)
			{
			case ApprovalInformation.ApprovalNotificationType.DecisionConflict:
			case ApprovalInformation.ApprovalNotificationType.DecisionUpdate:
				if (notificationType == ApprovalInformation.ApprovalNotificationType.DecisionConflict)
				{
					text7 = SystemMessages.DecisionConflictTopText.ToString(cultureFromLanguageHeaderValues);
				}
				else
				{
					text7 = SystemMessages.DecisionUpdateTopText.ToString(cultureFromLanguageHeaderValues);
				}
				if (string.IsNullOrEmpty(decisionMaker) || string.IsNullOrEmpty(text4) || decisionTime == null)
				{
					enumerable = new string[]
					{
						SystemMessages.DecisionConflictNotification(text5).ToString(cultureFromLanguageHeaderValues)
					};
				}
				else
				{
					string timeZone = ExTimeZone.CurrentTimeZone.LocalizableDisplayName.ToString(cultureFromLanguageHeaderValues);
					string empty;
					if (!DsnHumanReadableWriter.TryGetTimeString(new DateTime?(decisionTime.Value.LocalTime), cultureFromLanguageHeaderValues, out empty))
					{
						empty = string.Empty;
						timeZone = string.Empty;
					}
					string text8 = SystemMessages.DecisionConflictWithDetailsNotification(DsnHumanReadableWriter.HtmlEscape(textToText, decisionMaker), DsnHumanReadableWriter.HtmlEscape(textToText, text4), empty, timeZone).ToString(cultureFromLanguageHeaderValues);
					enumerable = new string[]
					{
						text8
					};
				}
				text9 = string.Empty;
				break;
			case ApprovalInformation.ApprovalNotificationType.ApprovalRequest:
				text7 = SystemMessages.ApprovalRequestTopText.ToString(cultureFromLanguageHeaderValues);
				enumerable = DsnHumanReadableWriter.GetModeratedDLApprovalRequestDetails(textToText, sender, text5, toRecipients, ccRecipients, moderatedRecipientsDisplayName, cultureFromLanguageHeaderValues, showApprovalRequestPreview);
				text9 = string.Empty;
				break;
			case ApprovalInformation.ApprovalNotificationType.ModeratedTransportReject:
			case ApprovalInformation.ApprovalNotificationType.ModeratedTransportRejectWithComments:
				text7 = SystemMessages.ModeratorRejectTopText.ToString(cultureFromLanguageHeaderValues);
				enumerable = DsnHumanReadableWriter.GetNotificationDetails(textToText, text5, moderatedRecipientsDisplayName, cultureFromLanguageHeaderValues);
				text9 = ((notificationType == ApprovalInformation.ApprovalNotificationType.ModeratedTransportRejectWithComments) ? SystemMessages.ModeratorCommentsHeader.ToString(cultureFromLanguageHeaderValues) : string.Empty);
				break;
			case ApprovalInformation.ApprovalNotificationType.ApprovalRequestExpiry:
				text7 = SystemMessages.ApprovalRequestExpiryTopText.ToString(cultureFromLanguageHeaderValues);
				enumerable = new string[]
				{
					SystemMessages.ApprovalRequestExpiryNotification(text5).ToString(cultureFromLanguageHeaderValues)
				};
				text9 = string.Empty;
				break;
			case ApprovalInformation.ApprovalNotificationType.ExpiryNotification:
			case ApprovalInformation.ApprovalNotificationType.ModeratorsNdrNotification:
			case ApprovalInformation.ApprovalNotificationType.ModeratorsOofNotification:
				if (notificationType == ApprovalInformation.ApprovalNotificationType.ExpiryNotification)
				{
					text7 = SystemMessages.ModerationExpiryNoticationForSenderTopText.ToString(cultureFromLanguageHeaderValues);
				}
				else if (notificationType == ApprovalInformation.ApprovalNotificationType.ModeratorsOofNotification)
				{
					text7 = SystemMessages.ModerationOofNotificationForSenderTopText.ToString(cultureFromLanguageHeaderValues);
				}
				else
				{
					if (notificationType != ApprovalInformation.ApprovalNotificationType.ModeratorsNdrNotification)
					{
						throw new InvalidOperationException("Unexpected dsn type.");
					}
					text7 = SystemMessages.ModerationNdrNotificationForSenderTopText.ToString(cultureFromLanguageHeaderValues);
				}
				enumerable = DsnHumanReadableWriter.GetNotificationDetails(textToText, text5, moderatedRecipientsDisplayName, cultureFromLanguageHeaderValues);
				text9 = string.Empty;
				break;
			case ApprovalInformation.ApprovalNotificationType.ModeratorExpiryNotification:
				text7 = SystemMessages.ModerationExpiryNoticationForModeratorTopText.ToString(cultureFromLanguageHeaderValues);
				enumerable = new string[]
				{
					"<br>\r\n",
					DsnHumanReadableWriter.HtmlEscape(textToText, SystemMessages.ModeratorExpiryNotification(sender, retentionPeriod.Value).ToString(cultureFromLanguageHeaderValues)),
					"<br>\r\n",
					DsnHumanReadableWriter.HtmlEscape(textToText, DsnHumanReadableWriter.GetRecipientString(moderatedRecipientsDisplayName, cultureFromLanguageHeaderValues))
				};
				text9 = string.Empty;
				break;
			default:
				throw new NotImplementedException("Notification type not supported");
			}
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(text9);
			outboundCodePageDetector.AddText(text2);
			outboundCodePageDetector.AddText(text3);
			outboundCodePageDetector.AddText(text7);
			outboundCodePageDetector.AddText(text);
			foreach (string value in enumerable)
			{
				outboundCodePageDetector.AddText(value);
			}
			outboundCodePageDetector.AddText(text6);
			Charset charset = DsnHumanReadableWriter.GetCharset(outboundCodePageDetector, cultureFromLanguageHeaderValues);
			int[] codePages = outboundCodePageDetector.GetCodePages();
			return new ApprovalInformation(charset, cultureFromLanguageHeaderValues, text6, text7, text, enumerable, text9, text2, text3, codePages);
		}

		private bool TryGetConfiguredOrDefaultExplanation(string cultureName, string enhancedStatusCode, CultureInfo culture, out string explanation, ref bool isCustomized)
		{
			if (this.configuredStatusCodes.TryGetValue(cultureName + enhancedStatusCode, out explanation))
			{
				isCustomized = true;
				return true;
			}
			LocalizedString localizedString;
			if (DsnDefaultMessages.TryGetResourceRecipientExplanation(enhancedStatusCode, out localizedString))
			{
				explanation = localizedString.ToString(culture);
				return true;
			}
			return false;
		}

		private void WriteDiagnosticBlock(DsnHumanReadableWriter.EncodedTextWriter writer, string diagnosticTitle, string headersTitle, string generatingServerTitle, string receivingServerTitle, List<DsnRecipientInfo> recipientList, string remoteMta, string reportingServer, HeaderList originalHeaders, string diagnosticFont)
		{
			writer.WriteToStream("<br><br><br><br><br><br>\r\n");
			writer.WriteToStream(diagnosticFont);
			writer.WriteToStream("<p><b>");
			writer.WriteToStream(diagnosticTitle);
			writer.WriteToStream("</b></p>\r\n");
			writer.WriteToStream("<p>");
			writer.WriteToStream(generatingServerTitle);
			writer.EscapeAndWriteToStream(reportingServer);
			if (recipientList != null && recipientList.Count > 0)
			{
				DsnRecipientInfo dsnRecipientInfo = recipientList[0];
				if (dsnRecipientInfo != null)
				{
					writer.WriteToStream("<br>\r\n");
					string receivingServerDetails = dsnRecipientInfo.ReceivingServerDetails;
					if (!string.IsNullOrEmpty(receivingServerDetails))
					{
						writer.WriteToStream(receivingServerTitle);
						writer.WriteToStream(receivingServerDetails);
						writer.WriteToStream("<br>\r\n");
					}
					if (dsnRecipientInfo.RetryCount > 0)
					{
						writer.WriteToStream(DsnHumanReadableWriter.GetFormattedRetryCountString(dsnRecipientInfo));
					}
				}
			}
			writer.WriteToStream("</p>\r\n");
			foreach (DsnRecipientInfo dsnRecipientInfo2 in ((IEnumerable<DsnRecipientInfo>)(recipientList ?? Enumerable.Empty<DsnRecipientInfo>())))
			{
				writer.WriteToStream("<p>");
				writer.EscapeAndWriteToStream(dsnRecipientInfo2.Address);
				writer.WriteToStream("<br>\r\n");
				if (!string.IsNullOrEmpty(remoteMta))
				{
					writer.EscapeAndWriteToStream(remoteMta);
					writer.WriteToStream("<br>\r\n");
				}
				if (!string.IsNullOrEmpty(dsnRecipientInfo2.LastPermanentErrorDetails))
				{
					writer.EscapeAndWriteToStream(dsnRecipientInfo2.LastPermanentErrorDetails);
				}
				else
				{
					writer.EscapeAndWriteToStream(DsnHumanReadableWriter.GetFormattedSmtpResponse(dsnRecipientInfo2));
				}
				writer.WriteToStream("<br>\r\n");
				if (!string.IsNullOrEmpty(dsnRecipientInfo2.LastTransientErrorDetails))
				{
					writer.EscapeAndWriteToStream(dsnRecipientInfo2.LastTransientErrorDetails);
				}
				writer.WriteToStream("</p>\r\n");
			}
			if (originalHeaders != null)
			{
				writer.WriteToStream("<p>");
				writer.WriteToStream(headersTitle);
				writer.WriteToStream("</p>\r\n");
				writer.WriteToStream("<pre>");
				byte[] array = new byte[2048];
				using (Stream stream = new MemoryStream())
				{
					foreach (Header header in originalHeaders)
					{
						stream.SetLength(0L);
						stream.Position = 0L;
						header.WriteTo(stream);
						stream.Position = 0L;
						int num;
						do
						{
							num = stream.Read(array, 0, 2048);
							writer.EscapeAndWriteToStream(array, num);
						}
						while (num == 2048);
					}
				}
			}
			writer.WriteToStream("</pre>\r\n");
			writer.WriteToStream("</font>\r\n");
		}

		private const string CRLF = "\r\n";

		private const string BodyTopTextBeginTags = "<body>\r\n<p><b>";

		private const string BodyTopTextEndTags = "</font></b></p>\r\n";

		private const string FontEndTags = "</font>\r\n";

		private const string ExternalErrorTextBeginTags = "\r\n<p><b>";

		private const string ExternalErrorTextEndTags = "</b></p>\r\n";

		private const string BodyBlockEndTags = "</font>\r\n";

		private const string SingleRecipientBeginTags = "<p><a href=\"mailto:";

		private const string SingleRecipientDisplayBeginTags = "\">";

		private const string SingleRecipientDisplayEndTags = "</a>";

		private const string LineBreakTag = "<br>\r\n";

		private const string SingleRecipientEndTags = "</p>\r\n";

		private const string DiagnosticsBeginTags = "<br><br><br><br><br><br>\r\n";

		private const string DiagnosticsEndTags = "</font>\r\n";

		private const string BoldedParagraphBeginTags = "<p><b>";

		private const string BoldedParagraphEndTags = "</b></p>\r\n";

		private const string ParagraphBeginTags = "<p>";

		private const string SeparatorTag = "<HR>";

		private const string ParagraphEndTags = "</p>\r\n";

		private const string HeaderBeginTags = "<pre>";

		private const string HeaderEndTags = "</pre>\r\n";

		private const string FinalTextBeginTags = "<HR/>";

		private const string SmtpResponseSeparator = " #";

		private const string FinalTextEndTags = "</font>\r\n";

		private const string HtmlEndTags = "</body>\r\n</html>";

		private const string DefaultNDRCode = "5.0.0";

		private const int Utf8codepage = 65001;

		private const string HtmlBeginTags = "<html>\r\n<Head></head>";

		private const string Red = "#FF0000";

		private const string Yellow = "#FFCC00";

		private const int QuotaBarMaxSize = 200;

		private const float MinQuotaFillPercent = 0.1f;

		private const string QuotaBarMaxSizeString = "200";

		private const int QuotaBarMinWhitePartSize = 4;

		private const string NonFullQuotaBar = "<table cellspacing=0;><tr><td style=\"background-color:{0};width:{1};border-left-style:solid;border-top-style:solid;border-bottom-style:solid;border-color:black;border-width:1\">{7}{2}</font></td><td style=\"background-color:#ffffff;width:{3};border-right-style:solid;border-top-style:solid;border-bottom-style:solid;border-color:black;border-width:1\">&nbsp</td><td>{7}<b>{4}</b></font></td></tr><tr><td style=\"width:{1}\"><font color=\"#ffffff\" size=\"0\">{5}</font></td><td style=\"width:{3}\">&nbsp</td><td><font color=\"#ffffff\" size=\"0\">{6}</font></td></tr></table>";

		private const string FullQuotaBar = "<table cellspacing=0;><tr><td style=\"background-color:{0};width:200;border-style:solid;border-color:black;border-width:1\">{5}{1}</font></td><td>{5}<b>{2}</b></font></td></tr><tr><td style=\"width:200\"><font color=\"#ffffff\" size=\"0\">{3}</font></td><td><font color=\"#ffffff\" size=\"0\">{4}</font></td></tr></table>";

		private const string PlainTextSize = "{0} {1}\r\n";

		private const string QuotaWarningPostfix = ".warning";

		private const string QuotaSendPostfix = ".send";

		private const string QuotaSendReceivePostfix = ".sendreceive";

		private const string StoreNDRStatusTextPrefix = "MSEXCH:MSExchangeIS:";

		private static List<CultureInfo> localizedLanguages = new List<CultureInfo>();

		private static DsnHumanReadableWriter dsnHumanReadableWriterWithoutConfig;

		private Dictionary<string, string> configuredStatusCodes;

		private CultureInfo defaultCulture;

		internal class EncodedTextWriter
		{
			public EncodedTextWriter(Stream stream, Encoding charsetEncoding)
			{
				this.charsetBuffer = new byte[charsetEncoding.GetMaxByteCount(2048)];
				this.charsetEncoding = charsetEncoding;
				this.stream = stream;
			}

			public void EscapeAndWriteToStream(byte[] data, int count)
			{
				for (int i = 0; i < count; i++)
				{
					byte b = data[i];
					if (b != 34)
					{
						if (b != 38)
						{
							switch (b)
							{
							case 60:
								this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedLessThanBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedLessThanBytes.Length);
								goto IL_A2;
							case 62:
								this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedGreaterThanBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedGreaterThanBytes.Length);
								goto IL_A2;
							}
							this.stream.WriteByte(data[i]);
						}
						else
						{
							this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedAmpBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedAmpBytes.Length);
						}
					}
					else
					{
						this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedQuoteBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedQuoteBytes.Length);
					}
					IL_A2:;
				}
			}

			public void EscapeAndWriteToStream(string text)
			{
				int length = text.Length;
				int num = 0;
				int num2;
				while (num < length && -1 != (num2 = text.IndexOfAny(DsnHumanReadableWriter.EncodedTextWriter.HtmlEscapedCharacters, num)))
				{
					this.WriteToStream(text, num, num2 - num);
					char c = text[num2];
					if (c != '"')
					{
						if (c != '&')
						{
							switch (c)
							{
							case '<':
								this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedLessThanBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedLessThanBytes.Length);
								break;
							case '>':
								this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedGreaterThanBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedGreaterThanBytes.Length);
								break;
							}
						}
						else
						{
							this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedAmpBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedAmpBytes.Length);
						}
					}
					else
					{
						this.stream.Write(DsnHumanReadableWriter.EncodedTextWriter.EscapedQuoteBytes, 0, DsnHumanReadableWriter.EncodedTextWriter.EscapedQuoteBytes.Length);
					}
					num = num2 + 1;
				}
				if (num < length)
				{
					this.WriteToStream(text, num, length - num);
				}
			}

			public void WriteToStream(string text)
			{
				this.WriteToStream(text, 0, text.Length);
			}

			private void WriteToStream(string text, int beginIndex, int length)
			{
				int i = beginIndex;
				int num = beginIndex + length;
				while (i < num)
				{
					int num2;
					if (num > i + 2048)
					{
						num2 = 2048;
					}
					else
					{
						num2 = num - i;
					}
					int bytes = this.charsetEncoding.GetBytes(text, i, num2, this.charsetBuffer, 0);
					this.stream.Write(this.charsetBuffer, 0, bytes);
					i += num2;
				}
			}

			private const byte LessThanByte = 60;

			private const byte GreaterThanByte = 62;

			private const byte QuoteByte = 34;

			private const byte AmpByte = 38;

			private const int CharsetConvertCharacterCount = 2048;

			private static readonly char[] HtmlEscapedCharacters = new char[]
			{
				'<',
				'>',
				'"',
				'&'
			};

			private static readonly byte[] EscapedLessThanBytes = CTSGlobals.AsciiEncoding.GetBytes("&lt;");

			private static readonly byte[] EscapedGreaterThanBytes = CTSGlobals.AsciiEncoding.GetBytes("&gt;");

			private static readonly byte[] EscapedQuoteBytes = CTSGlobals.AsciiEncoding.GetBytes("&quot;");

			private static readonly byte[] EscapedAmpBytes = CTSGlobals.AsciiEncoding.GetBytes("&amp;");

			private byte[] charsetBuffer;

			private Encoding charsetEncoding;

			private Stream stream;
		}

		private class DsnLocalizedTexts
		{
			private DsnLocalizedTexts(LocalizedString subject, LocalizedString topText)
			{
				this.subject = subject;
				this.topText = topText;
			}

			public LocalizedString TopText
			{
				get
				{
					return this.topText;
				}
			}

			public LocalizedString Subject
			{
				get
				{
					return this.subject;
				}
			}

			public static DsnHumanReadableWriter.DsnLocalizedTexts GetDsnLocalizedTexts(DsnFlags dsnFlag)
			{
				if (dsnFlag <= DsnFlags.Relay)
				{
					switch (dsnFlag)
					{
					case DsnFlags.Delivery:
						return DsnHumanReadableWriter.DsnLocalizedTexts.deliveredTexts;
					case DsnFlags.Delay:
						return DsnHumanReadableWriter.DsnLocalizedTexts.delayedTexts;
					case DsnFlags.Delivery | DsnFlags.Delay:
						break;
					case DsnFlags.Failure:
						return DsnHumanReadableWriter.DsnLocalizedTexts.failedTexts;
					default:
						if (dsnFlag == DsnFlags.Relay)
						{
							return DsnHumanReadableWriter.DsnLocalizedTexts.relayedTexts;
						}
						break;
					}
				}
				else
				{
					if (dsnFlag == DsnFlags.Expansion)
					{
						return DsnHumanReadableWriter.DsnLocalizedTexts.expandedTexts;
					}
					if (dsnFlag == DsnFlags.Quarantine)
					{
						return DsnHumanReadableWriter.DsnLocalizedTexts.quarantinedTexts;
					}
				}
				throw new InvalidOperationException("dsnFlag not expected");
			}

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts failedTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.FailedSubject, SystemMessages.FailedHumanReadableTopText);

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts delayedTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.DelayedSubject, SystemMessages.DelayedHumanReadableTopText);

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts relayedTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.RelayedSubject, SystemMessages.RelayedHumanReadableTopText);

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts deliveredTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.DeliveredSubject, SystemMessages.DeliveredHumanReadableTopText);

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts expandedTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.ExpandedSubject, SystemMessages.ExpandedHumanReadableTopText);

			private static readonly DsnHumanReadableWriter.DsnLocalizedTexts quarantinedTexts = new DsnHumanReadableWriter.DsnLocalizedTexts(SystemMessages.FailedSubject, SystemMessages.QuarantinedHumanReadableTopText);

			private LocalizedString subject;

			private LocalizedString topText;
		}
	}
}
