using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MimeConstants
	{
		internal static bool IsInReservedHeaderNamespace(string headerName)
		{
			foreach (string value in MimeConstants.reservedHeaderPrefixes)
			{
				if (headerName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsReservedHeaderAllowedOnDelivery(string headerName)
		{
			foreach (string value in MimeConstants.reservedHeadersAllowedOnDelivery)
			{
				if (headerName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal const string MessageRFC822Type = "message/rfc822";

		internal const string MessageExternalBody = "message/external-body";

		internal const string MessageHeadersRFC822Type = "text/rfc822-headers";

		internal const string MessagePartial = "message/partial";

		internal const string MessageDeliveryStatus = "message/delivery-status";

		internal const string MessageDispositionNotification = "message/disposition-notification";

		internal const string TextPlainBodyType = "text/plain";

		internal const string TextEnrichedBodyType = "text/enriched";

		internal const string TextHtmlBodyType = "text/html";

		internal const string TextCalendarType = "text/calendar";

		internal const string TextDirectoryType = "text/directory";

		internal const string TextVCardType = "text/x-vcard";

		internal const string TextMediaType = "text";

		internal const string MultipartMediaType = "multipart";

		internal const string ApplicationOpenMailType = "application/x-openmail";

		internal const string ApplicationMsTnefType = "application/ms-tnef";

		internal const string ApplicationXPkcs7MimeType = "application/x-pkcs7-mime";

		internal const string ApplicationPkcs7MimeType = "application/pkcs7-mime";

		internal const string ApplicationOctetStreamType = "application/octet-stream";

		internal const string ApplicationMacBinHex40 = "application/mac-binhex40";

		internal const string ApplicationAppleFile = "application/applefile";

		internal const string ImageContentType = "image/";

		internal const string ImageJpegContentType = "image/jpeg";

		internal const string ImagePjpegContentType = "image/pjpeg";

		internal const string ImageGifContentType = "image/gif";

		internal const string ImageBmpContentType = "image/bmp";

		internal const string ImagePngContentType = "image/png";

		internal const string ImageXpngContentType = "image/x-png";

		internal const string MultiPart = "multipart/";

		internal const string MultiPartAlternative = "multipart/alternative";

		internal const string MultiPartMixed = "multipart/mixed";

		internal const string MultiPartRelated = "multipart/related";

		internal const string MultiPartParallel = "multipart/parallel";

		internal const string MultiPartDigest = "multipart/digest";

		internal const string MultiPartSigned = "multipart/signed";

		internal const string MultiPartReport = "multipart/report";

		internal const string MultiPartFormData = "multipart/form-data";

		internal const string MultiPartAppleDouble = "multipart/appledouble";

		internal const string ContentClassVoice = "voice";

		internal const string ContentClassVoiceCa = "voice-ca";

		internal const string ContentClassVoiceUc = "voice-uc";

		internal const string ContentClassTranscription = "voice+transcript";

		internal const string ContentClassUMPartner = "MS-Exchange-UM-Partner";

		internal const string ContentClassFax = "fax";

		internal const string ContentClassFaxCa = "fax-ca";

		internal const string ContentClassMissedCall = "missedcall";

		internal const string ContentClassRss = "RSS";

		internal const string ContentClassSharing = "Sharing";

		internal const string ContentClassCustomPrefix = "urn:content-class:custom.";

		internal const string InfoPathContentClassPrefix = "InfoPathForm.";

		internal const string ThreadTopic = "Thread-Topic";

		internal const string ThreadIndex = "Thread-Index";

		internal const string TnefCorrelator = "X-MS-TNEF-Correlator";

		internal const string MsHasAttach = "X-MS-Has-Attach";

		internal const string XMimeOle = "X-MimeOle";

		internal const string XAutoResponseSuppress = "X-Auto-Response-Suppress";

		internal const string XSendOutlookRecallReport = "X-MS-Exchange-Send-Outlook-Recall-Report";

		internal const string AcceptLanguage = "Accept-Language";

		internal const string XAcceptLanguage = "X-Accept-Language";

		internal const string XNotesItem = "X-Notes-Item";

		internal const string XMicrosoftClassified = "x-microsoft-classified";

		internal const string XMicrosoftClassification = "x-microsoft-classification";

		internal const string XMicrosoftClassificationDescription = "x-microsoft-classDesc";

		internal const string XMicrosoftClassificationGuid = "x-microsoft-classID";

		internal const string XMicrosoftClassificationKeep = "X-microsoft-classKeep";

		internal const string XQuarantineOriginalSender = "X-MS-Exchange-Organization-Original-Sender";

		internal const string XPayloadProviderGuid = "X-Payload-Provider-Guid";

		internal const string XPayloadClass = "X-Payload-Class";

		internal const string CallingTelephoneNumber = "X-CallingTelephoneNumber";

		internal const string VoiceMessageDuration = "X-VoiceMessageDuration";

		internal const string VoiceMessageSenderName = "X-VoiceMessageSenderName";

		internal const string FaxNumberOfPages = "X-FaxNumberOfPages";

		internal const string AttachmentOrder = "X-AttachmentOrder";

		internal const string CallId = "X-CallID";

		internal const string RequireProtectedPlayOnPhone = "X-RequireProtectedPlayOnPhone";

		internal const string XMSJournalReport = "X-MS-Journal-Report";

		internal const string XMSOutlookProtectionRuleVersion = "X-MS-Outlook-Client-Rule-Addin-Version";

		internal const string XMSOutlookProtectionRuleTimestamp = "X-MS-Outlook-Client-Rule-Config-Timestamp";

		internal const string XMSOutlookProtectionRuleOverridden = "X-MS-Outlook-Client-Rule-Overridden";

		internal const string XMessageFlag = "X-Message-Flag";

		internal const string XListHelp = "X-List-Help";

		internal const string XListSubscribe = "X-List-Subscribe";

		internal const string XListUnsubscribe = "X-List-Unsubscribe";

		internal const string MimeSkeletonContentId = "X-Exchange-Mime-Skeleton-Content-Id";

		internal const string XLoop = "X-MS-Exchange-Inbox-Rules-Loop";

		internal const string XSharingBrowseUrl = "x-sharing-browse-url";

		internal const string XSharingCapabilities = "x-sharing-capabilities";

		internal const string XSharingFlavor = "x-sharing-flavor";

		internal const string XSharingInstanceGuid = "x-sharing-instance-guid";

		internal const string XSharingLocalType = "x-sharing-local-type";

		internal const string XSharingProviderGuid = "x-sharing-provider-guid";

		internal const string XSharingProviderName = "x-sharing-provider-name";

		internal const string XSharingProviderUrl = "x-sharing-provider-url";

		internal const string XSharingRemoteName = "x-sharing-remote-name";

		internal const string XSharingRemotePath = "x-sharing-remote-path";

		internal const string XSharingRemoteType = "x-sharing-remote-type";

		internal const string XExchangeApplicationFlags = "X-MS-Exchange-ApplicationFlags";

		internal const string XGroupMailboxId = "X-MS-Exchange-GroupMailbox-Id";

		internal const string XMessageSentRepresentingType = "X-MS-Exchange-MessageSentRepresentingType";

		internal const string XMSExchangeSharedMailboxSentItemsRoutingAgentProcessed = "X-MS-Exchange-SharedMailbox-RoutingAgent-Processed";

		internal const string XMSExchangeSharedMailboxSentItemMessage = "X-MS-Exchange-SharedMailbox-SentItem-Message";

		internal const int XLoopMaximumLength = 1000;

		internal const int XLoopMaximumCount = 3;

		internal const int XLoopDatacenterMaximumCount = 1;

		internal const string XMsExchangeOrganizationAutoforwarded = "X-MS-Exchange-Organization-AutoForwarded";

		internal const string XMsExchOrganizationAntispamReport = "X-MS-Exchange-Organization-Antispam-Report";

		internal const string XMsExchOrganizationPrd = "X-MS-Exchange-Organization-PRD";

		internal const string XMsExchOrganizationScl = "X-MS-Exchange-Organization-SCL";

		internal const string XMsExchOrganizationPcl = "X-MS-Exchange-Organization-PCL";

		internal const string XMSExchOrganizationSenderIdResult = "X-MS-Exchange-Organization-SenderIdResult";

		internal const string XMsExchOrganizationAuthDomain = "X-MS-Exchange-Organization-AuthDomain";

		internal const string XMsExchOrganizationAuthMechanism = "X-MS-Exchange-Organization-AuthMechanism";

		internal const string XMsExchOrganizationAuthSource = "X-MS-Exchange-Organization-AuthSource";

		internal const string XMsExchOrganizationAuthAs = "X-MS-Exchange-Organization-AuthAs";

		internal const string XMsExchangeOrganizationRightsProtectMessage = "X-MS-Exchange-Organization-RightsProtectMessage";

		internal const string XMsExchangeOrganizationCrossPremiseDecrypted = "X-MS-Exchange-Organization-CrossPremiseDecrypted";

		internal const string XMSExchangeForestTransportDecryptionActionHeader = "X-MS-Exchange-Forest-TransportDecryption-Action";

		internal const string XMsExchOrganizationAVStampMailbox = "X-MS-Exchange-Organization-AVStamp-Mailbox";

		internal const string XMsExchOrganizationOriginalScl = "X-MS-Exchange-Organization-Original-SCL";

		internal const string XMSExchangeOrganizationDecisionMakers = "X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers";

		internal const string XMSExchangeOrganizationApprovalRequestor = "X-MS-Exchange-Organization-Approval-Requestor";

		internal const string XMSExchangeJournalingRemoteAccounts = "X-MS-Exchange-Organization-Journaling-Remote-Accounts";

		internal const string XMSExchangeOrganizationRecipientP2Type = "X-MS-Exchange-Organization-Recipient-P2-Type";

		internal const string XMsExchOrganizationOriginalReceivedTime = "X-MS-Exchange-Organization-Original-Received-Time";

		internal const string XMSExchangeOrganizationNetworkMessageId = "X-MS-Exchange-Organization-Network-Message-Id";

		internal const string XMsExchOrganizationSharingInstanceGuid = "X-MS-Exchange-Organization-Sharing-Instance-Guid";

		internal const string XMsExchOrganizationCloudId = "X-MS-Exchange-Organization-Cloud-Id";

		internal const string XMsExchOrganizationCloudVersion = "X-MS-Exchange-Organization-Cloud-Version";

		internal const string XMsExchangeUMPartnerAssignedID = "X-MS-Exchange-UM-PartnerAssignedID";

		internal const string XMsExchangeUMPartnerContent = "X-MS-Exchange-UM-PartnerContent";

		internal const string XMsExchangeUMPartnerContext = "X-MS-Exchange-UM-PartnerContext";

		internal const string XMsExchangeUMPartnerStatus = "X-MS-Exchange-UM-PartnerStatus";

		internal const string XMsExchangeUMDialPlanLanguage = "X-MS-Exchange-UM-DialPlanLanguage";

		internal const string XMsExchangeUMCallerInformedOfAnalysis = "X-MS-Exchange-UM-CallerInformedOfAnalysis";

		internal const string ReceivedSPF = "Received-SPF";

		private const string XMsExchOrganizationPrefix = "X-MS-Exchange-Organization-";

		private const string XMsExchForestPrefix = "X-MS-Exchange-Forest-";

		private const string XMsExchCrossPremisesPrefix = "X-MS-Exchange-CrossPremises-";

		internal const string XMSExchangeOutlookProtectionRuleVersion = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Addin-Version";

		internal const string XMSExchangeOutlookProtectionRuleConfigTimestamp = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Config-Timestamp";

		internal const string XMSExchangeOutlookProtectionRuleOverridden = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Overridden";

		internal const string XMsExchOrganizationDeliverAsRead = "X-MS-Exchange-Organization-DeliverAsRead";

		internal const string XMsExchOrganizationMailReplied = "X-MS-Exchange-Organization-MailReplied";

		internal const string XMsExchOrganizationMailForwarded = "X-MS-Exchange-Organization-MailForwarded";

		internal const string XMsExchOrganizationCategory = "X-MS-Exchange-Organization-Category";

		internal const string XMSExchangeOrganizationDirectionalityHeader = "X-MS-Exchange-Organization-MessageDirectionality";

		internal const string XMSExchangeCalendarOriginatorIdHeader = "X-MS-Exchange-Calendar-Originator-Id";

		internal const string XMSExchangeCalendarSeriesSequenceNumberHeader = "X-MS-Exchange-Calendar-Series-Sequence-Number";

		internal const string XMSExchangeCalendarSeriesIdHeader = "X-MS-Exchange-Calendar-Series-Id";

		internal const string XMSExchangeCalendarSeriesInstanceIdHeader = "X-MS-Exchange-Calendar-Series-Instance-Id";

		internal const string XMSExchangeCalendarSeriesMasterIdHeader = "X-MS-Exchange-Calendar-Series-Master-Id";

		internal const string XMSExchangeCalendarSeriesInstanceUnparkedHeader = "X-MS-Exchange-Calendar-Series-Instance-Unparked";

		internal const string XMSExchangeCalendarSeriesInstanceCalendarItemIdHeader = "X-MS-Exchange-Calendar-Series-Instance-Calendar-Item-Id";

		internal const string XMsExchImapAppendStamp = "X-MS-Exchange-ImapAppendStamp";

		internal const string XmsExchOrganizationDlpPrefix = "X-Ms-Exchange-Organization-Dlp-";

		internal const string XMsExchOrganizationDlpSenderOverride = "X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification";

		internal const string XMsExchOrganizationDlpFalsePositive = "X-Ms-Exchange-Organization-Dlp-FalsePositive";

		internal const string XMsExchOrganizationDlpDetectedClassifications = "X-Ms-Exchange-Organization-Dlp-DetectedClassifications";

		internal const string XMSExchangeOrganizationAVStampServiceName = "X-MS-Exchange-Organization-AVStamp-Service";

		internal const string XMSExchangeOrganizationAVStampEnterpriseName = "X-MS-Exchange-Organization-AVStamp-Enterprise";

		public const string XMmsMesageId = "X-MmsMessageId";

		internal const string XSimSlotNumber = "X-SimSlotNumber";

		public const string XSentTime = "X-SentTime";

		public const string XSentItem = "X-SentItem";

		internal const string RecipientP2TypeBcc = "Bcc";

		internal const string High = "high";

		internal const string Low = "low";

		internal const string Normal = "normal";

		internal const string XPriority5 = "5";

		internal const string XPriority3 = "3";

		internal const string XPriority1 = "1";

		internal const string Personal = "personal";

		internal const string Private = "private";

		internal const string CompanyConfidential = "company-confidential";

		internal const string Urgent = "urgent";

		internal const string NonUrgent = "non-urgent";

		internal const string Inline = "inline";

		internal const string Attachment = "attachment";

		internal const string CreationDate = "creation-date";

		internal const string ModificationDate = "modification-date";

		internal const string ReadDate = "read-date";

		internal const string Charset = "charset";

		internal const string Profile = "profile";

		internal const string VCard = "vCard";

		internal const string CharsetUtf8 = "utf-8";

		internal const string CharsetUSAscii = "us-ascii";

		internal const string Name = "name";

		internal const string Size = "size";

		internal const string Type = "type";

		internal const string Boundary = "boundary";

		internal const string ReportType = "report-type";

		internal const string Filename = "filename";

		internal const string DeliveryStatus = "delivery-status";

		internal const string DispositionNotification = "disposition-notification";

		internal const string BinhexEncoding = "binhex";

		internal const string Base64Encoding = "base64";

		internal const string QPEncoding = "quoted-printable";

		internal const string SevenBitEncoding = "7-bit";

		internal const string EmptyDateHeader = "<empty>";

		internal const string DefaultSmimeAttachmentName = "smime.p7m";

		internal const string HeaderTrue = "true";

		internal const string HeaderFalse = "false";

		internal const string MethodHeader = "method";

		internal const string AccessTypeParameter = "access-type";

		internal const string AccessTypeAnonFtp = "anon-ftp";

		internal const string SiteParameter = "site";

		internal const string DirectoryParameter = "directory";

		internal const string UrlExtension = ".url";

		internal const string FtpShortcutPrefix = "[InternetShortcut]\r\nURL=ftp://";

		internal const string FtpShortcutSuffix = "\r\n";

		internal const string ModeParameter = "mode";

		internal const string ModeAscii = "ascii";

		internal const string FtpShortcutTypeAscii = ";type=a";

		internal const string ModeImage = "image";

		internal const string FtpShortcutTypeImage = ";type=i";

		internal const string Yes = "yes";

		internal const string XMimeOleExchange = "Microsoft Exchange";

		internal const string SmimeType = "smime-type";

		internal const string SmimeTypeEncrypted = "enveloped-data";

		internal const string SmimeTypeSigned = "signed-data";

		internal const string SmimeTypeCerts = "certs-only";

		internal const int MaxReferencesHeaderLength = 65536;

		internal const string SmtpAddressType = "SMTP";

		internal const string ImceaAddressPrefix = "IMCEA";

		internal const string FileNameWinMailDat = "winmail.dat";

		internal const string UndisclosedRecipientsGroup = "undisclosed recipients";

		internal const string OleAttachmentDefaultFilenameExtension = "jpg";

		internal const string OleAttachmentDefaultFilename = "{0}.jpg";

		internal const string OleAttachmentComputedFilename = "{0} {1}.jpg";

		internal const string Rfc822 = "RFC822";

		internal const string Unknown = "unknown";

		internal const string ExtensionPrefix = "X-";

		internal const string DefaultMdnDispositionTypeAndModifier = "automatic-action/MDN-sent-automatically";

		internal const string ExtensionMsg = ".msg";

		internal const string ExtensionVcf = ".vcf";

		internal const string FailedJournalReportName = "corrupt.eml";

		internal const string ProtectedAttachmentFilename = "message.rpmsg";

		internal const string ProtectedAttachmentContentType = "application/x-microsoft-rpmsg-message";

		internal const uint TnefNamedPropertyTag = 2147483648U;

		internal const uint MultiValuedPropertyFlag = 4096U;

		private static string[] reservedHeaderPrefixes = new string[]
		{
			"X-MS-Exchange-Organization-",
			"X-MS-Exchange-Forest-",
			"X-MS-Exchange-CrossPremises-"
		};

		private static string[] reservedHeadersAllowedOnDelivery = new string[]
		{
			"X-MS-Exchange-Organization-Antispam-Report",
			"X-MS-Exchange-Organization-AVStamp-Mailbox",
			"X-MS-Exchange-Organization-Approval-Requestor",
			"X-MS-Exchange-Organization-AuthAs",
			"X-MS-Exchange-Organization-AuthDomain",
			"X-MS-Exchange-Organization-AuthMechanism",
			"X-MS-Exchange-Organization-AuthSource",
			"X-MS-Exchange-Organization-CrossPremiseDecrypted",
			"X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers",
			"X-MS-Exchange-Organization-Journaling-Remote-Accounts",
			"X-MS-Exchange-Organization-Original-SCL",
			"X-MS-Exchange-Organization-PCL",
			"X-MS-Exchange-Organization-PRD",
			"X-MS-Exchange-Organization-Recipient-P2-Type",
			"X-MS-Exchange-Organization-SCL",
			"X-MS-Exchange-Organization-SenderIdResult",
			"X-MS-Exchange-Organization-Sharing-Instance-Guid",
			"X-MS-Exchange-Forest-TransportDecryption-Action",
			"X-MS-Exchange-Organization-DeliverAsRead",
			"X-MS-Exchange-Organization-MailReplied",
			"X-MS-Exchange-Organization-MailForwarded",
			"X-MS-Exchange-Organization-Category",
			"X-MS-Exchange-Organization-Network-Message-Id",
			"X-MS-Exchange-Organization-AVStamp-Service",
			"X-MS-Exchange-Organization-AVStamp-Enterprise",
			"X-MS-Exchange-Organization-MessageDirectionality"
		};

		internal static readonly Guid IID_IStorage = new Guid(11, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);
	}
}
