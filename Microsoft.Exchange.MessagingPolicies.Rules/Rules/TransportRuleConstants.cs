using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class TransportRuleConstants
	{
		public const string EtrAgentName = "Transport Rule";

		public const string SclHeader = "X-MS-Exchange-Organization-SCL";

		public const string DlpPolicyIdRuleTagName = "CP";

		public const string DlpPolicyNameRuleTagName = "CPN";

		internal const string TagIsSameUser = "isSameUser";

		internal const string TagIsMemberOf = "isMemberOf";

		internal const string TagIsPartner = "isPartner";

		internal const string TagIsInternal = "isInternal";

		internal const string TagIsExternalPartner = "isExternalPartner";

		internal const string TagIsMessageType = "isMessageType";

		internal const string TagSenderAttributeContains = "senderAttributeContains";

		internal const string TagSenderAttributeMatches = "senderAttributeMatches";

		internal const string TagSenderAttributeMatchesRegex = "senderAttributeMatchesRegex";

		internal const string TagRecipientAttributeContains = "recipientAttributeContains";

		internal const string TagRecipientAttributeMatches = "recipientAttributeMatches";

		internal const string TagRecipientAttributeMatchesRegex = "recipientAttributeMatchesRegex";

		internal const string TagAttachmentContainsWords = "attachmentContainsWords";

		internal const string TagAttachmentMatchesPatterns = "attachmentMatchesPatterns";

		internal const string TagAttachmentMatchesRegexPatterns = "attachmentMatchesRegexPatterns";

		internal const string TagAttachmentIsUnsupported = "attachmentIsUnsupported";

		internal const string TagAttachmentIsPasswordProtected = "attachmentIsPasswordProtected";

		internal const string TagAttachmentPropertyContains = "attachmentPropertyContains";

		internal const string TagSenderInRecipientList = "senderInRecipientList";

		internal const string TagRecipientInSenderList = "recipientInSenderList";

		internal const string TagFork = "fork";

		internal const string TagRecipient = "recipient";

		internal const string TagFromRecipient = "fromRecipient";

		internal const string TagFromList = "fromList";

		internal const string TagManager = "manager";

		internal const string TagIsSenderEvaluation = "isSenderEvaluation";

		internal const string TagCheckADAttributeEquality = "checkADAttributeEquality";

		internal const string TagManagementRelationship = "managementRelationship";

		internal const string TagADAttribute = "adAttribute";

		internal const string TagADAttributeForTextMatch = "adAttributeForTextMatch";

		internal const string TagADAttributeValueForTextMatch = "adAttributeValueForTextMatch";

		internal const string TagPartner = "partner";

		internal const string TagExternal = "external";

		internal const string TagInternal = "internal";

		internal const string TagExternalPartner = "externalPartner";

		internal const string TagExternalNonPartner = "externalNonPartner";

		internal const string TagRecipientContainsWords = "recipientContainsWords";

		internal const string TagRecipientDomainIs = "recipientDomainIs";

		internal const string TagRecipientMatchesPatterns = "recipientMatchesPatterns";

		internal const string TagRecipientMatchesRegexPatterns = "recipientMatchesRegexPatterns";

		internal const string TagContainsDataClassification = "containsDataClassification";

		internal const string TagHasSenderOverride = "hasSenderOverride";

		internal const string TagIpMatch = "ipMatch";

		internal const string TagAttachmentProcessingLimitExceeded = "attachmentProcessingLimitExceeded";

		internal const string TagDomainIs = "domainIs";

		internal const string AttributeDomain = "domain";

		internal const string AttributeAddress = "address";

		internal const string AttributeValue = "value";

		internal const string True = "true";

		internal const string False = "false";

		internal const string Manager = "Manager";

		internal const string DirectReport = "DirectReport";

		internal const string SenderAddressLocation = "senderAddressLocation";

		internal const SenderAddressLocation DefaultSenderAddressLocation = Microsoft.Exchange.MessagingPolicies.Rules.SenderAddressLocation.Header;

		internal const string FromScopeExternalAuth = "<>";

		internal const string FromScopeInternalAuth = "FromInternal";

		internal const string DeleteMessage = "DeleteMessage";

		internal const string SetPriority = "SetPriority";

		internal const string SetExtendedPropertyString = "SetExtendedPropertyString";

		internal const string AddToRecipient = "AddToRecipient";

		internal const string AddCcRecipient = "AddCcRecipient";

		internal const string AddToRecipientSmtpOnly = "AddToRecipientSmtpOnly";

		internal const string AddCcRecipientSmtpOnly = "AddCcRecipientSmtpOnly";

		internal const string AddEnvelopeRecipient = "AddEnvelopeRecipient";

		internal const string AddManagerAsRecipientType = "AddManagerAsRecipientType";

		internal const string ModerateMessageByUser = "ModerateMessageByUser";

		internal const string ModerateMessageByManager = "ModerateMessageByManager";

		internal const string RedirectMessage = "RedirectMessage";

		internal const string AddHeader = "AddHeader";

		internal const string RemoveHeader = "RemoveHeader";

		internal const string SetSubject = "SetSubject";

		internal const string PrependSubject = "PrependSubject";

		internal const string Halt = "Halt";

		internal const string SetHeader = "SetHeader";

		internal const string SetHeaderUniqueValue = "SetHeaderUniqueValue";

		internal const string ApplyHtmlDisclaimer = "ApplyHtmlDisclaimer";

		internal const string ApplyDisclaimerWithSeparator = "ApplyDisclaimerWithSeparator";

		internal const string ApplyDisclaimer = "ApplyDisclaimer";

		internal const string ApplyDisclaimerWithSeparatorAndReadingOrder = "ApplyDisclaimerWithSeparatorAndReadingOrder";

		internal const string Quarantine = "Quarantine";

		internal const string Disconnect = "Disconnect";

		internal const string RejectMessage = "RejectMessage";

		internal const string LogEvent = "LogEvent";

		internal const string RightsProtectMessage = "RightsProtectMessage";

		internal const string RouteMessageOutboundRequireTls = "RouteMessageOutboundRequireTls";

		internal const string ApplyOME = "ApplyOME";

		internal const string RemoveOME = "RemoveOME";

		internal const string RouteMessageOutboundConnector = "RouteMessageOutboundConnector";

		internal const string MessageTypeOof = "OOF";

		internal const string MessageTypeAutoForward = "AutoForward";

		internal const string MessageTypeEncrypted = "Encrypted";

		internal const string MessageTypeCalendaring = "Calendaring";

		internal const string MessageTypePermissionControlled = "PermissionControlled";

		internal const string MessageTypeVoicemail = "Voicemail";

		internal const string MessageTypeSigned = "Signed";

		internal const string MessageTypeApprovalRequest = "ApprovalRequest";

		internal const string MessageTypeReadReceipt = "ReadReceipt";

		internal const string AuditSeverityLevel = "AuditSeverityLevel";

		internal const string SenderNotify = "SenderNotify";

		internal const string GenerateIncidentReport = "GenerateIncidentReport";

		internal const string GenerateNotification = "GenerateNotification";

		internal const string ClassificationGuidHeader = "X-MS-Exchange-Organization-Classification";

		internal const string XMSExchangeFfoAttributedTenantId = "X-MS-Exchange-Organization-Id";

		internal static readonly Version VersionedContainerBaseVersion = new Version("14.00.0000.00");

		public static class FileTypeNames
		{
			public const string FileTypeExecutable = "executable";

			public const string FileTypeUnknown = "unknown";
		}

		public static class PropertyNames
		{
			public const string AttachmentNames = "Message.AttachmentNames";

			public const string AttachmentExtensions = "Message.AttachmentExtensions";

			public const string AttachmentTypes = "Message.AttachmentTypes";

			public const string MaxAttachmentSize = "Message.MaxAttachmentSize";

			public const string ContentCharacterSets = "Message.ContentCharacterSets";

			public const string SenderDomain = "Message.SenderDomain";
		}

		public static class AuditingValues
		{
			public const string RuleId = "ruleId";

			public const string DlpProgramId = "dlpId";

			public const string MatchingDataClassificationId = "dcId";

			public const string WhenChangedUTC = "st";

			public const string Action = "action";

			public const string SenderOverridden = "sndOverride";

			public const string SenderOverriddenValue = "or";

			public const string SenderOverriddenJustification = "just";

			public const string SenderOverriddenFpValue = "fp";

			public const string Severity = "sev";

			public const string DataClassificationId = "dcid";

			public const string DataClassificationCount = "count";

			public const string DataClassificationConfidence = "conf";

			public const string RuleMode = "mode";

			public const string LoadW = "LoadW";

			public const string LoadC = "Loadc";

			public const string ExecW = "ExecW";

			public const string ExecC = "ExecC";
		}
	}
}
