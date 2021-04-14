using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class SupportedConditionNames
	{
		public const string From = "From";

		public const string SentTo = "SentTo";

		public const string FromScope = "FromScope";

		public const string SentToScope = "SentToScope";

		public const string FromMemberOf = "FromMemberOf";

		public const string SentToMemberOf = "SentToMemberOf";

		public const string SubjectOrBodyContains = "SubjectOrBodyContains";

		public const string FromAddressContains = "FromAddressContains";

		public const string RecipientAddressContains = "RecipientAddressContains";

		public const string AttachmentContainsWords = "AttachmentContainsWords";

		public const string AttachmentMatchesPatterns = "AttachmentMatchesPatterns";

		public const string AttachmentIsUnsupported = "AttachmentIsUnsupported";

		public const string SubjectOrBodyMatchesPatterns = "SubjectOrBodyMatchesPatterns";

		public const string FromAddressMatchesPatterns = "FromAddressMatchesPatterns";

		public const string RecipientAddressMatchesPatterns = "RecipientAddressMatchesPatterns";

		public const string AttachmentNameMatchesPatterns = "AttachmentNameMatchesPatterns";

		public const string MessageTypeMatches = "MessageTypeMatches";

		public const string HasClassification = "HasClassification";

		public const string HasNoClassification = "HasNoClassification";

		public const string SubjectContainsWords = "SubjectContainsWords";

		public const string SubjectMatchesPatterns = "SubjectMatchesPatterns";

		public const string AnyOfToHeader = "AnyOfToHeader";

		public const string AnyOfToHeaderMemberOf = "AnyOfToHeaderMemberOf";

		public const string AnyOfCcHeader = "AnyOfCcHeader";

		public const string AnyOfCcHeaderMemberOf = "AnyOfCcHeaderMemberOf";

		public const string AnyOfToCcHeader = "AnyOfToCcHeader";

		public const string AnyOfToCcHeaderMemberOf = "AnyOfToCcHeaderMemberOf";

		public const string SenderManagementRelationship = "SenderManagementRelationship";

		public const string SCLOver = "SCLOver";

		public const string WithImportance = "WithImportance";

		public const string SenderInRecipientList = "SenderInRecipientList";

		public const string RecipientInSenderList = "RecipientInSenderList";

		public const string HeaderContains = "HeaderContains";

		public const string BetweenMemberOf = "BetweenMemberOf";

		public const string HeaderMatches = "HeaderMatches";

		public const string ManagerForEvaluatedUser = "ManagerForEvaluatedUser";

		public const string ADComparisonAttribute = "ADComparisonAttribute";

		public const string AttachmentSizeOver = "AttachmentSizeOver";

		public const string AttachmentProcessingLimitsExceeded = "AttachmentProcessingLimitExceeded";

		public const string SenderADAttributeContainsWords = "SenderADAttributeContainsWords";

		public const string SenderADAttributeMatchesPatterns = "SenderADAttributeMatchesPatterns";

		public const string RecipientADAttributeContainsWords = "RecipientADAttributeContainsWords";

		public const string RecipientADAttributeMatchesPatterns = "RecipientADAttributeMatchesPatterns";

		public const string MessageContainsDataClassifications = "MessageContainsDataClassifications";

		public const string AttachmentExtensionMatchesWords = "AttachmentExtensionMatchesWords";

		public const string MessageSizeOver = "MessageSizeOver";

		public const string HasSenderOverride = "HasSenderOverride";

		public const string AttachmentHasExecutableContent = "AttachmentHasExecutableContent";

		public const string SenderIpRange = "SenderIpRange";

		public const string SenderDomainIs = "SenderDomainIs";

		public const string RecipientDomainIs = "RecipientDomainIs";

		public const string ContentCharacterSetContainsWords = "ContentCharacterSetContainsWords";

		public const string AttachmentIsPasswordProtected = "AttachmentIsPasswordProtected";

		public const string AnyOfRecipientAddressContainsWords = "AnyOfRecipientAddressContainsWords";

		public const string AnyOfRecipientAddressMatchesPatterns = "AnyOfRecipientAddressMatchesPatterns";
	}
}
