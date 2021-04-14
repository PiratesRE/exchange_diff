using System;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal static class MimeConstant
	{
		public const string XMSExchangeOrganizationMBTSubmissionProcessedMessage = "X-MS-Exchange-Organization-Processed-By-MBTSubmission";

		public const string XMSExchangeOrganizationJournalingProcessedMessage = "X-MS-Exchange-Organization-Processed-By-Journaling";

		public const string XMSExchangeOrganizationGccJournalingProcessedMessage = "X-MS-Exchange-Organization-Processed-By-Gcc-Journaling";

		public const string XMSExchangeOrganizationProcessedByUnjournal = "X-MS-Exchange-Organization-Unjournal-Processed";

		public const string XMSExchangeOrganizationSenderIsRecipient = "X-MS-Exchange-Organization-Unjournal-SenderIsRecipient";

		public const string XMSExchangeOrganizationSenderAddress = "X-MS-Exchange-Organization-Unjournal-SenderAddress";

		public const string XMSExchangeOrganizationProcessedByUnjournalForNdr = "X-MS-Exchange-Organization-Unjournal-ProcessedNdr";

		public const string XMSExchangeOrganizationMessageOriginalDate = "X-MS-Exchange-Organization-Unjournal-OriginalReceiveDate";

		public const string XMSExchangeOrganizationMessageExpiryDate = "X-MS-Exchange-Organization-Unjournal-OriginalExpiryDate";

		public const string XMSInternalJournalReport = "X-MS-InternalJournal";

		public const string XMSExchangeOrganizationGeneratedMessageSource = "X-MS-Exchange-Organization-Generated-Message-Source";

		public const string XMSExchangeGeneratedMessageSource = "X-MS-Exchange-Generated-Message-Source";

		public const string XMSExchangeParentMessageId = "X-MS-Exchange-Parent-Message-Id";

		public const string XMSExchangeMessageIsNdr = "X-MS-Exchange-Message-Is-Ndr";

		public const string XMSExchangeOrganizationFfoServiceTagHeader = "X-MS-Exchange-Organization-FFO-ServiceTag";

		public const string XMSExchangeOrganizationCrossPremiseEncrypted = "X-MS-Exchange-Organization-CrossPremiseEncrypted";

		public const string XMSExchangeOrganizationCrossPremiseDecrypted = "X-MS-Exchange-Organization-CrossPremiseDecrypted";

		public const string XMSExchangeOrganizationApaExecuted = "X-MS-Exchange-Organization-MessageProtectedByApa";

		public const string XMSExchangeOrganizationJournalNdrSkipTransportMailboxRules = "X-MS-Exchange-Organization-JournalNdr-Skip-TransportMailboxRules";

		public const string XMSExchangeOrganizationMalwareFilterPolicy = "X-MS-Exchange-Organization-MalwareFilterPolicy";

		public const string XHeaderDecryptCrossPremise = "X-Decrypt-CrossPremise";

		public const string TestMessageHeader = "X-MS-Exchange-Organization-Test-Message";

		public const string TestMessageLogForHeader = "X-MS-Exchange-Organization-Test-Message-Log-For";

		public const string TestMessageSupressValue = "Supress";

		public const string TestMessageDeliverValue = "Deliver";

		public const string TestMessageSendReportToHeader = "X-MS-Exchange-Organization-Test-Message-Send-Report-To";

		public const string TestMessageOptionsHeader = "X-MS-Exchange-Organization-Test-Message-Options";

		public const string XMSExchangeOrganizationDeliveryFolder = "X-MS-Exchange-Organization-DeliveryFolder";

		public const string XMSExchangeOrganizationStorageQuota = "X-MS-Exchange-Organization-StorageQuota";

		public const string EndOfInjectedXHeaders = "X-EndOfInjectedXHeaders";

		public const string XCreatedBy = "X-CreatedBy";

		public const string XSender = "X-Sender";

		public const string XReceiver = "X-Receiver";

		public const string MSExchange12 = "MSExchange12";

		public const string MicrosoftApprovalRequestRecallClass = "IPM.Note.Microsoft.Approval.Request.Recall";

		public const string XAutoResponseSuppress = "X-Auto-Response-Suppress";

		public const string XModerationData = "X-Moderation-Data";

		public const string XSendOutlookRecallReport = "X-MS-Exchange-Send-Outlook-Recall-Report";

		public const string XOrganizationHygienePolicy = "X-MS-Exchange-Organization-HygienePolicy";

		public const string XMSExchangeOrganizationHygieneReleasedFromQuarantine = "X-MS-Exchange-Organization-Hygiene-ReleasedFromQuarantine";

		public const string HeaderXMsReplyToMobile = "X-MS-Reply-To-Mobile";

		public const string XOriginatingIP = "X-Originating-IP";

		internal const string XMSTnefCorrelator = "X-MS-TNEF-Correlator";

		internal const string ThreadIndex = "Thread-Index";

		internal const string ThreadTopic = "Thread-Topic";

		internal const string TransportFlagMustDeliver = "MustDeliver";

		internal const string DeliveryPriority = "DeliveryPriority";

		internal const string AcceptLanguage = "Accept-Language";

		internal const string XMSExchangeOrganizationScl = "X-MS-Exchange-Organization-SCL";

		internal const string XMSExchangeOrganizationPcl = "X-MS-Exchange-Organization-PCL";

		internal const string XMSExchangeOrganizationSenderIdResult = "X-MS-Exchange-Organization-SenderIdResult";

		internal const string XMSExchangeOrganizationPrd = "X-MS-Exchange-Organization-PRD";

		internal const string XMSExchangeOrganizationAntiSpamReport = "X-MS-Exchange-Organization-Antispam-Report";

		internal const string XMSExchangeOrganizationDsnVersion = "X-MS-Exchange-Organization-Dsn-Version";

		internal const string XMSExchangeOrganizationOriginalSize = "X-MS-Exchange-Organization-OriginalSize";

		internal const string XMSExchangeOrganizationContentConversionOptions = "X-MS-Exchange-Organization-ContentConversionOptions";

		internal const string XMSExchangeOrganizationMessageSource = "X-MS-Exchange-Organization-MessageSource";

		internal const string XMSExchangeOrganizationPrioritization = "X-MS-Exchange-Organization-Prioritization";

		internal const string XMSExchangeOrganizationSpamFilterEnumeratedRisk = "X-MS-Exchange-Organization-Spam-Filter-Enumerated-Risk";

		internal const string XMSExchangeOrganizationAuthAs = "X-MS-Exchange-Organization-AuthAs";

		internal const string XMSExchangeOrganizationAuthDomain = "X-MS-Exchange-Organization-AuthDomain";

		internal const string XMSExchangeOrganizationAuthMechanism = "X-MS-Exchange-Organization-AuthMechanism";

		internal const string XMSExchangeOrganizationAuthSource = "X-MS-Exchange-Organization-AuthSource";

		internal const string XMSExchangeOrganizationAntispamIPv6Check = "X-MS-Exchange-Organization-Antispam-IPv6Check";

		internal const string XMSExchangeOrganizationOutboundIPPartition = "X-MS-Exchange-Organization-Antispam-OutboundIPPartition";

		public const string XMSExchangeAntispamAsyncContext = "X-MS-Exchange-Organization-Antispam-AsyncContext";

		public const string XMSExchangeAntispamContentFilterScanContext = "X-MS-Exchange-Organization-Antispam-ContentFilter-ScanContext";

		public const string XMSExchangeAntispamScanContext = "X-MS-Exchange-Organization-Antispam-ScanContext";

		public const string AntispamScanContextXPremTagName = "XPREM";

		public const char AntispamScanContextNameValuePairSeparator = ';';

		public const char AntispamScanContextNameValueSeparator = ':';

		internal const string XMSExchangeOrganizationRecipientP2Type = "X-MS-Exchange-Organization-Recipient-P2-Type";

		internal const string XMSExchangeOrganizationNetworkMessageId = "X-MS-Exchange-Organization-Network-Message-Id";

		internal const string XMSExchangeOrganizationHistory = "X-MS-Exchange-Organization-History";

		internal const string XMSExchangeOrganizationDisclaimerHash = "X-MS-Exchange-Organization-Disclaimer-Hash";

		internal const string XMSExchangeDisclaimerWrapperHeader = "X-MS-Exchange-Organization-Disclaimer-Wrapper";

		internal const string XMSExchangeOrganizationRulesExecutionHistoryHeader = "X-MS-Exchange-Organization-Rules-Execution-History";

		internal const string XMSExchangeForestSkipRuleExecution = "X-MS-Exchange-Forest-RulesExecuted";

		internal const string XMSExchangeTransportRulesLoop = "X-MS-Exchange-Transport-Rules-Loop";

		internal const string XMSExchangeTransportRulesDeferCount = "X-MS-Exchange-Transport-Rules-Defer-Count";

		internal const string XMSExchangeOrganizationTransportRulesFipsResult = "X-MS-Exchange-Organization-Transport-Rules-Fips-Result";

		internal const string XMSExchangeTransportRulesIncidentReport = "X-MS-Exchange-Transport-Rules-IncidentReport";

		internal const string XMSExchangeTransportRulesNotification = "X-MS-Exchange-Transport-Rules-Notification";

		internal const string XMSExchangeModerationLoop = "X-MS-Exchange-Moderation-Loop";

		internal const string XMSExchangeJournalReportHeader = "X-MS-Exchange-Organization-Journal-Report";

		internal const string XMSExchangeOrganizationBcc = "X-MS-Exchange-Organization-BCC";

		internal const string XMSExchangeJournalReportDecryptionProcessedHeader = "X-MS-Exchange-Organization-JournalReportDecryption-Processed";

		internal const string XMSExchangeJournaledToRecipients = "X-MS-Exchange-Organization-Journaled-To-Recipients";

		internal const string XMSExchangeJournalingRemoteAccounts = "X-MS-Exchange-Organization-Journaling-Remote-Accounts";

		internal const string XMSExchangeDoNotJournal = "X-MS-Exchange-Organization-Do-Not-Journal";

		internal const string XMSExchangeMapiAdminSubmission = "X-MS-Exchange-Organization-Mapi-Admin-Submission";

		internal const string XMSExchangeTransportPropertiesHeader = "X-MS-Exchange-Organization-Transport-Properties";

		internal const string XMSExchangeOrganizationId = "X-MS-Exchange-Organization-Id";

		internal const string XMSExchangeMatchingConnectorTenantId = "X-MS-Exchange-Organization-MatchingConnector-TenantId";

		internal const string XMSExchangeOrganizationDirectionalityHeader = "X-MS-Exchange-Organization-MessageDirectionality";

		internal const string XMSExchangeOrganizationInboundConnectorTypeHeader = "X-MS-Exchange-Organization-MessageInboundConnectorType";

		internal const string XMSExchangeOrganizationConnectorInfo = "X-MS-Exchange-Organization-ConnectorInfo";

		internal const string XMSExchangeOrganizationScopeHeader = "X-MS-Exchange-Organization-MessageScope";

		internal const string XMSExchangeForestScopeHeader = "X-MS-Exchange-Forest-MessageScope";

		internal const string XMSExchangeForestTransportDecryptionActionHeader = "X-MS-Exchange-Forest-TransportDecryption-Action";

		internal const string XMSExchangeOrganizationOriginalClientIPAddress = "X-MS-Exchange-Organization-OriginalClientIPAddress";

		internal const string XMSExchangeOrganizationOriginalServerIPAddress = "X-MS-Exchange-Organization-OriginalServerIPAddress";

		internal const string XMSQuarantineOriginalSenderHeader = "X-MS-Exchange-Organization-Original-Sender";

		internal const string XMSJournalReportHeader = "X-MS-Journal-Report";

		internal const string XMSGccJournalReportHeader = "X-MS-Gcc-Journal-Report";

		internal const string XMSExchangeQuarantineMessageMarkerHeader = "X-MS-Exchange-Organization-Quarantine";

		internal const string XMSExchangeOrganizationOriginalScl = "X-MS-Exchange-Organization-Original-SCL";

		internal const string XMSExchangeOrganizationDlExpansionProhibited = "X-MS-Exchange-Organization-DL-Expansion-Prohibited";

		internal const string XMSExchangeOrganizationAltRecipientProhibited = "X-MS-Exchange-Organization-Alt-Recipient-Prohibited";

		internal const string XMSExchangeOrganizationJournalRecipientList = "X-MS-Exchange-Organization-JournalRecipientList";

		internal const string TransportLabelHeader = "X-MS-Exchange-Organization-Classification";

		internal const string ClassifiedHeader = "x-microsoft-classified";

		internal const string ClassificationHeader = "x-microsoft-classification";

		internal const string ClassDescHeader = "x-microsoft-classDesc";

		internal const string ClassIDHeader = "x-microsoft-classID";

		internal const string ClassificationKeep = "X-microsoft-classKeep";

		internal const string StoreDriver = "StoreDriver";

		internal const string XMSExchangeOrganizationDecisionMakersHeader = "X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers";

		internal const string XMSExchangeOrganizationAllowedActionsHeader = "X-MS-Exchange-Organization-Approval-Allowed-Actions";

		internal const string XMSExchangeOrganizationApprovalRequestorHeader = "X-MS-Exchange-Organization-Approval-Requestor";

		internal const string XMSExchangeOrganizationApprovalInitiatorHeader = "X-MS-Exchange-Organization-Approval-Initiator";

		internal const string XMSExchangeOrganizationApprovalAttachToApprovalRequestHeader = "X-MS-Exchange-Organization-Approval-AttachToApprovalRequest";

		internal const string XMSExchangeOrganizationApprovalApprovedHeader = "X-MS-Exchange-Organization-Approval-Approved";

		internal const string XMSExchangeOrganizationBypassChildModerationHeader = "X-MS-Exchange-Organization-Bypass-Child-Moderation";

		internal const string XMSExchangeOrganizationModerationSavedArrivalTimeHeader = "X-MS-Exchange-Organization-Moderation-SavedArrivalTime";

		internal const string XMSExchangeOrganizationModerationData = "X-MS-Exchange-Organization-Moderation-Data";

		internal const char ModerationSuppressNotification = '0';

		internal const char ModerationEnableNotification = '1';

		internal const string XMSExchangeOrganizationRightsProtectMessage = "X-MS-Exchange-Organization-RightsProtectMessage";

		internal const string XMSExchangeOrganizationE4eEncryptMessage = "X-MS-Exchange-Organization-E4eEncryptMessage";

		internal const string XMSExchangeOrganizationE4eDecryptMessage = "X-MS-Exchange-Organization-E4eDecryptMessage";

		internal const string XMSExchangeOrganizationE4eMessageOriginalSender = "X-MS-Exchange-Organization-E4eMessageOriginalSender";

		internal const string XMSExchangeOrganizationE4eMessageOriginalSenderOrgId = "X-MS-Exchange-Organization-E4eMessageOriginalSenderOrgId";

		internal const string XMSExchangeOrganizationE4eMessageDecrypted = "X-MS-Exchange-Organization-E4eMessageDecrypted";

		internal const string XMSExchangeOrganizationE4eMessageEncrypted = "X-MS-Exchange-Organization-E4eMessageEncrypted";

		internal const string XMSExchangeOrganizationE4eHtmlFileGenerated = "X-MS-Exchange-Organization-E4eHtmlFileGenerated";

		internal const string XMSExchangeOrganizationE4ePortal = "X-MS-Exchange-Organization-E4ePortal";

		internal const string XMSExchangeOrganizationE4eReEncryptMessage = "X-MS-Exchange-Organization-E4eReEncryptMessage";

		internal const string XMSExchangeOMEMessageEncrypted = "X-MS-Exchange-OMEMessageEncrypted";

		internal const string XMSExchangeOutlookProtectionRuleVersion = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Addin-Version";

		internal const string XMSExchangeOutlookProtectionRuleConfigTimestamp = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Config-Timestamp";

		internal const string XMSExchangeOutlookProtectionRuleOverridden = "X-MS-Exchange-Organization-Outlook-Protection-Rule-Overridden";

		internal const string XMSExchangeOrganizationContentConvertInternalMessage = "X-MS-Exchange-Organization-ContentConvertInternalMessage";

		internal const string XMSExchangeForestArrivalHubServer = "X-MS-Exchange-Forest-ArrivalHubServer";

		internal const string XMSExchangeOrganizationOriginatorOrganization = "X-MS-Exchange-Organization-OriginatorOrganization";

		internal const string XMSExchangeForestRoutedForHighAvailability = "X-MS-Exchange-Forest-RoutedForHighAvailability";

		internal const string XMSExchangeDeliveryDatabase = "X-MS-Exchange-Delivery-Database";

		internal const string XMSExchangeItemDeliveryDatabaseName = "X-MS-Exchange-Delivery-Database-Name";

		internal const string XMSExchangeOrganizationAVStampServiceName = "X-MS-Exchange-Organization-AVStamp-Service";

		internal const string XMSExchangeOrganizationAVStampEnterpriseName = "X-MS-Exchange-Organization-AVStamp-Enterprise";

		internal const string XMSExchangeOrganizationAVScannedByV2Stamp = "X-MS-Exchange-Organization-AVScannedByV2";

		internal const string XMSExchangeOrganizationMatchedInterceptorRule = "X-MS-Exchange-Organization-Matched-Interceptor-Rule";

		internal const string EncryptedSystemProbeGuidHeader = "X-FFOSystemProbe";

		internal const string UnencryptedSystemProbeGuidHeader = "X-LAMNotificationId";

		internal const string XMSExchangeOrganizationOutboundConnector = "X-MS-Exchange-Organization-OutboundConnector";

		internal const string XMSExchangeOrganizationCatchAllOriginalRecipients = "X-MS-Exchange-Organization-CatchAll-OriginalRecipients";

		internal const string XMSExchangeActiveMonitoringProbeName = "X-MS-Exchange-ActiveMonitoringProbeName";

		internal const string ProbeMessageDropHeader = "X-Exchange-Probe-Drop-Message";

		internal const string SystemProbeDropHeader = "X-Exchange-System-Probe-Drop";

		internal const string ProbeMessageDropHeaderValueEOH = "FrontEnd-EOH-250";

		internal const string ProbeMessageDropHeaderValueCAT = "FrontEnd-CAT-250";

		internal const string ProbeMessageDropHeaderValueOnCategorizedMessage = "OnCategorizedMessage";

		internal const string ProbeMessageDropHeaderValueOnEndOfHeaders = "OnEndOfHeaders";

		internal const string ProbeMessageDropHeaderValueSDD = "MailboxTransportDelivery-SDD-250";

		internal const string PersistProbeTraceHeader = "X-Exchange-Persist-Probe-Trace";

		internal const string XMSExchangeForestGalScope = "X-MS-Exchange-Forest-GAL-Scope";

		internal const string XMSExchangeAddressBookPolicy = "X-MS-Exchange-ABP-GUID";

		public static readonly string AntispamScanContextXPremTagNameWithSeparator = "XPREM" + ':';

		internal static readonly string[] XOriginatorOrganization = new string[]
		{
			"X-OriginatorOrg",
			"X-OriginatorOrganization"
		};

		internal static readonly string PreferredXOriginatorOrganization = MimeConstant.XOriginatorOrganization[0];

		public enum ApprovalAllowedAction
		{
			ApproveReject,
			ApproveRejectComments,
			ApproveRejectCommentOnApprove,
			ApproveRejectCommentOnReject
		}

		public class MultiLevelAuthHeaderValue
		{
			public const string Anonymous = "Anonymous";
		}

		public class ApprovalAttachHeaderValue
		{
			public const string Never = "Never";

			public const string AsIs = "AsIs";

			public const string AsMessage = "AsMessage";
		}
	}
}
