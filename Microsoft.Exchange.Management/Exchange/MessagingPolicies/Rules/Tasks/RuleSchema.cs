using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class RuleSchema : RulePresentationObjectBaseSchema
	{
		public static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2003, typeof(int), "priority", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DlpPolicy = new ADPropertyDefinition("DlpPolicy", ExchangeObjectVersion.Exchange2012, typeof(string), "dlpPolicy", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 64)
		}, null, null);

		public static readonly ADPropertyDefinition Comments = new ADPropertyDefinition("Comments", ExchangeObjectVersion.Exchange2003, typeof(string), "comments", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition Conditions = new ADPropertyDefinition("Conditions", ExchangeObjectVersion.Exchange2003, typeof(TransportRulePredicate[]), "conditions", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Actions = new ADPropertyDefinition("Actions", ExchangeObjectVersion.Exchange2003, typeof(TransportRuleAction[]), "actions", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Exceptions = new ADPropertyDefinition("Exceptions", ExchangeObjectVersion.Exchange2003, typeof(TransportRulePredicate[]), "exceptions", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition State = new ADPropertyDefinition("State", ExchangeObjectVersion.Exchange2003, typeof(RuleState), "state", ADPropertyDefinitionFlags.None, RuleState.Enabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Mode = new ADPropertyDefinition("Mode", ExchangeObjectVersion.Exchange2012, typeof(RuleMode), "mode", ADPropertyDefinitionFlags.None, RuleMode.Enforce, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RuleSubType = new ADPropertyDefinition("RuleSubType", ExchangeObjectVersion.Exchange2012, typeof(RuleSubType), "ruleSubType", ADPropertyDefinitionFlags.None, Microsoft.Exchange.MessagingPolicies.Rules.RuleSubType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RuleErrorAction = new ADPropertyDefinition("RuleErrorAction", ExchangeObjectVersion.Exchange2012, typeof(RuleErrorAction), "ruleErrorAction", ADPropertyDefinitionFlags.None, Microsoft.Exchange.MessagingPolicies.Rules.RuleErrorAction.Ignore, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderAddressLocation = new ADPropertyDefinition("SenderAddressLocation", ExchangeObjectVersion.Exchange2012, typeof(SenderAddressLocation), "senderAddressLocation", ADPropertyDefinitionFlags.None, Microsoft.Exchange.MessagingPolicies.Rules.SenderAddressLocation.Header, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseLegacyRegex = new ADPropertyDefinition("UseLegacyRegex", ExchangeObjectVersion.Exchange2012, typeof(bool), "useLegacyRegex", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition From = new ADPropertyDefinition("From", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "From".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FromMemberOf = new ADPropertyDefinition("FromMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "FromMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FromScope = new ADPropertyDefinition("FromScope", ExchangeObjectVersion.Exchange2003, typeof(FromUserScope?), "FromScope".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SentTo = new ADPropertyDefinition("SentTo", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "SentTo".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SentToMemberOf = new ADPropertyDefinition("SentToMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "SentToMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SentToScope = new ADPropertyDefinition("SentToScope", ExchangeObjectVersion.Exchange2003, typeof(ToUserScope?), "SentToScope".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BetweenMemberOf1 = new ADPropertyDefinition("BetweenMemberOf1", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "BetweenMemberOf1".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BetweenMemberOf2 = new ADPropertyDefinition("BetweenMemberOf2", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "BetweenMemberOf2".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ManagerAddresses = new ADPropertyDefinition("ManagerAddresses", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ManagerAddresses".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ManagerForEvaluatedUser = new ADPropertyDefinition("ManagerForEvaluatedUser", ExchangeObjectVersion.Exchange2003, typeof(EvaluatedUser?), "ManagerForEvaluatedUser".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderManagementRelationship = new ADPropertyDefinition("SenderManagementRelationship", ExchangeObjectVersion.Exchange2003, typeof(ManagementRelationship?), "SenderManagementRelationship".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADComparisonAttribute = new ADPropertyDefinition("ADComparisonAttribute", ExchangeObjectVersion.Exchange2003, typeof(ADAttribute?), "ADComparisonAttribute".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADComparisonOperator = new ADPropertyDefinition("ADComparisonOperator", ExchangeObjectVersion.Exchange2003, typeof(Evaluation?), "ADComparisonOperator".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderADAttributeContainsWords = new ADPropertyDefinition("SenderADAttributeContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "SenderADAttributeContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderADAttributeMatchesPatterns = new ADPropertyDefinition("SenderADAttributeMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "SenderADAttributeMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientADAttributeContainsWords = new ADPropertyDefinition("RecipientADAttributeContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "RecipientADAttributeContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientADAttributeMatchesPatterns = new ADPropertyDefinition("RecipientADAttributeMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "RecipientADAttributeMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfToHeader = new ADPropertyDefinition("AnyOfToHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfToHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfToHeaderMemberOf = new ADPropertyDefinition("AnyOfToHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfToHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfCcHeader = new ADPropertyDefinition("AnyOfCcHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfCcHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfCcHeaderMemberOf = new ADPropertyDefinition("AnyOfCcHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfCcHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfToCcHeader = new ADPropertyDefinition("AnyOfToCcHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfToCcHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfToCcHeaderMemberOf = new ADPropertyDefinition("AnyOfToCcHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AnyOfToCcHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HasClassification = new ADPropertyDefinition("HasClassification", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "HasClassification".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HasNoClassification = new ADPropertyDefinition("HasNoClassification", ExchangeObjectVersion.Exchange2003, typeof(bool), "HasNoClassification".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SubjectContainsWords = new ADPropertyDefinition("SubjectContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "SubjectContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SubjectOrBodyContainsWords = new ADPropertyDefinition("SubjectOrBodyContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "SubjectOrBodyContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HeaderContainsMessageHeader = new ADPropertyDefinition("HeaderContainsMessageHeader", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "HeaderContainsMessageHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HeaderContainsWords = new ADPropertyDefinition("HeaderContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "HeaderContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FromAddressContainsWords = new ADPropertyDefinition("FromAddressContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "FromAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderDomainIs = new ADPropertyDefinition("SenderDomainIs", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "SenderDomainIs".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientDomainIs = new ADPropertyDefinition("RecipientDomainIs", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "RecipientDomainIs".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SubjectMatchesPatterns = new ADPropertyDefinition("SubjectMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "SubjectMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SubjectOrBodyMatchesPatterns = new ADPropertyDefinition("SubjectOrBodyMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "SubjectOrBodyMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HeaderMatchesMessageHeader = new ADPropertyDefinition("HeaderMatchesMessageHeader", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "HeaderMatchesMessageHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HeaderMatchesPatterns = new ADPropertyDefinition("HeaderMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "HeaderMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FromAddressMatchesPatterns = new ADPropertyDefinition("FromAddressMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "FromAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentNameMatchesPatterns = new ADPropertyDefinition("AttachmentNameMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "AttachmentNameMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentExtensionMatchesWords = new ADPropertyDefinition("AttachmentExtensionMatchesWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "AttachmentExtensionMatchesWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentPropertyContainsWords = new ADPropertyDefinition("AttachmentPropertyContainsWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "AttachmentPropertyContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContentCharacterSetContainsWords = new ADPropertyDefinition("ContentCharacterSetContainsWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ContentCharacterSetContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLOver = new ADPropertyDefinition("SCLOver", ExchangeObjectVersion.Exchange2003, typeof(SclValue?), "SCLOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentSizeOver = new ADPropertyDefinition("AttachmentSizeOver", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), "AttachmentSizeOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageSizeOver = new ADPropertyDefinition("MessageSizeOver", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), "MessageSizeOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WithImportance = new ADPropertyDefinition("WithImportance", ExchangeObjectVersion.Exchange2003, typeof(Importance?), "WithImportance".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageTypeMatches = new ADPropertyDefinition("MessageTypeMatches", ExchangeObjectVersion.Exchange2003, typeof(MessageType?), "MessageTypeMatches".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientAddressContainsWords = new ADPropertyDefinition("RecipientAddressContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "RecipientAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderInRecipientList = new ADPropertyDefinition("SenderInRecipientList", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "SenderInRecipientList".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfRecipientAddressContainsWords = new ADPropertyDefinition("AnyOfRecipientAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(Word[]), "AnyOfRecipientAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnyOfRecipientAddressMatchesPatterns = new ADPropertyDefinition("AnyOfRecipientAddressMatchesPatterns", ExchangeObjectVersion.Exchange2010, typeof(Pattern[]), "AnyOfRecipientAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientInSenderList = new ADPropertyDefinition("RecipientInSenderList", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "RecipientInSenderList".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientAddressMatchesPatterns = new ADPropertyDefinition("RecipientAddressMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "RecipientAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentContainsWords = new ADPropertyDefinition("AttachmentContainsWords", ExchangeObjectVersion.Exchange2010, typeof(Word[]), "AttachmentContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentMatchesPatterns = new ADPropertyDefinition("AttachmentMatchesPatterns", ExchangeObjectVersion.Exchange2010, typeof(Pattern[]), "AttachmentMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentIsUnsupported = new ADPropertyDefinition("AttachmentIsUnsupported", ExchangeObjectVersion.Exchange2010, typeof(bool), "AttachmentIsUnsupported".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentProcessingLimitExceeded = new ADPropertyDefinition("AttachmentProcessingLimitExceeded", ExchangeObjectVersion.Exchange2010, typeof(bool), "AttachmentProcessingLimitExceeded".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentHasExecutableContent = new ADPropertyDefinition("AttachmentHasExecutableContent", ExchangeObjectVersion.Exchange2010, typeof(bool), "AttachmentHasExecutableContent".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentIsPasswordProtected = new ADPropertyDefinition("AttachmentIsPasswordProtected", ExchangeObjectVersion.Exchange2010, typeof(bool), "AttachmentIsPasswordProtected".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HasSenderOverride = new ADPropertyDefinition("HasSenderOverride", ExchangeObjectVersion.Exchange2012, typeof(bool), "HasSenderOverride".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageContainsDataClassifications = new ADPropertyDefinition("MessageContainsDataClassifications", ExchangeObjectVersion.Exchange2010, typeof(string[]), "MessageContainsDataClassifications".ToLower(), ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderIpRanges = new ADPropertyDefinition("SenderIpRanges", ExchangeObjectVersion.Exchange2010, typeof(IPRange), "SenderIpRanges".ToLower(), ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfFrom = new ADPropertyDefinition("ExceptIfFrom", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfFrom".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfFromMemberOf = new ADPropertyDefinition("ExceptIfFromMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfFromMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfFromScope = new ADPropertyDefinition("ExceptIfFromScope", ExchangeObjectVersion.Exchange2003, typeof(FromUserScope?), "ExceptIfFromScope".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSentTo = new ADPropertyDefinition("ExceptIfSentTo", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfSentTo".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSentToMemberOf = new ADPropertyDefinition("ExceptIfSentToMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfSentToMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSentToScope = new ADPropertyDefinition("ExceptIfSentToScope", ExchangeObjectVersion.Exchange2003, typeof(ToUserScope?), "ExceptIfSentToScope".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfBetweenMemberOf1 = new ADPropertyDefinition("ExceptIfBetweenMemberOf1", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfBetweenMemberOf1".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfBetweenMemberOf2 = new ADPropertyDefinition("ExceptIfBetweenMemberOf2", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfBetweenMemberOf2".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfManagerAddresses = new ADPropertyDefinition("ExceptIfManagerAddresses", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfManagerAddresses".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfManagerForEvaluatedUser = new ADPropertyDefinition("ExceptIfManagerForEvaluatedUser", ExchangeObjectVersion.Exchange2003, typeof(EvaluatedUser?), "ExceptIfManagerForEvaluatedUser".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderManagementRelationship = new ADPropertyDefinition("ExceptIfSenderManagementRelationship", ExchangeObjectVersion.Exchange2003, typeof(ManagementRelationship?), "ExceptIfSenderManagementRelationship".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfADComparisonAttribute = new ADPropertyDefinition("ExceptIfADComparisonAttribute", ExchangeObjectVersion.Exchange2003, typeof(ADAttribute?), "ExceptIfADComparisonAttribute".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfADComparisonOperator = new ADPropertyDefinition("ExceptIfADComparisonOperator", ExchangeObjectVersion.Exchange2003, typeof(Evaluation?), "ExceptIfADComparisonOperator".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderADAttributeContainsWords = new ADPropertyDefinition("ExceptIfSenderADAttributeContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfSenderADAttributeContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderADAttributeMatchesPatterns = new ADPropertyDefinition("ExceptIfSenderADAttributeMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfSenderADAttributeMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientADAttributeContainsWords = new ADPropertyDefinition("ExceptIfRecipientADAttributeContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfRecipientADAttributeContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientADAttributeMatchesPatterns = new ADPropertyDefinition("ExceptIfRecipientADAttributeMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfRecipientADAttributeMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfToHeader = new ADPropertyDefinition("ExceptIfAnyOfToHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfToHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfToHeaderMemberOf = new ADPropertyDefinition("ExceptIfAnyOfToHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfToHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfCcHeader = new ADPropertyDefinition("ExceptIfAnyOfCcHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfCcHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfCcHeaderMemberOf = new ADPropertyDefinition("ExceptIfAnyOfCcHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfCcHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfToCcHeader = new ADPropertyDefinition("ExceptIfAnyOfToCcHeader", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfToCcHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfToCcHeaderMemberOf = new ADPropertyDefinition("ExceptIfAnyOfToCcHeaderMemberOf", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ExceptIfAnyOfToCcHeaderMemberOf".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHasClassification = new ADPropertyDefinition("ExceptIfHasClassification", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "ExceptIfHasClassification".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHasNoClassification = new ADPropertyDefinition("ExceptIfHasNoClassification", ExchangeObjectVersion.Exchange2003, typeof(bool), "ExceptIfHasNoClassification".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSubjectContainsWords = new ADPropertyDefinition("ExceptIfSubjectContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfSubjectContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSubjectOrBodyContainsWords = new ADPropertyDefinition("ExceptIfSubjectOrBodyContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfSubjectOrBodyContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHeaderContainsMessageHeader = new ADPropertyDefinition("ExceptIfHeaderContainsMessageHeader", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "ExceptIfHeaderContainsMessageHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHeaderContainsWords = new ADPropertyDefinition("ExceptIfHeaderContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfHeaderContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfFromAddressContainsWords = new ADPropertyDefinition("ExceptIfFromAddressContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfFromAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderDomainIs = new ADPropertyDefinition("ExceptIfSenderDomainIs", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ExceptIfSenderDomainIs".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientDomainIs = new ADPropertyDefinition("ExceptIfRecipientDomainIs", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ExceptIfRecipientDomainIs".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSubjectMatchesPatterns = new ADPropertyDefinition("ExceptIfSubjectMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfSubjectMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSubjectOrBodyMatchesPatterns = new ADPropertyDefinition("ExceptIfSubjectOrBodyMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfSubjectOrBodyMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHeaderMatchesMessageHeader = new ADPropertyDefinition("ExceptIfHeaderMatchesMessageHeader", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "ExceptIfHeaderMatchesMessageHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHeaderMatchesPatterns = new ADPropertyDefinition("ExceptIfHeaderMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfHeaderMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfFromAddressMatchesPatterns = new ADPropertyDefinition("ExceptIfFromAddressMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfFromAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentNameMatchesPatterns = new ADPropertyDefinition("ExceptIfAttachmentNameMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfAttachmentNameMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentExtensionMatchesWords = new ADPropertyDefinition("ExceptIfAttachmentExtensionMatchesWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ExceptIfAttachmentExtensionMatchesWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentPropertyContainsWords = new ADPropertyDefinition("ExceptIfAttachmentPropertyContainsWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ExceptIfAttachmentPropertyContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfContentCharacterSetContainsWords = new ADPropertyDefinition("ExceptIfContentCharacterSetContainsWords", ExchangeObjectVersion.Exchange2012, typeof(Word[]), "ExceptIfContentCharacterSetContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSCLOver = new ADPropertyDefinition("ExceptIfSCLOver", ExchangeObjectVersion.Exchange2003, typeof(SclValue?), "ExceptIfSCLOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentSizeOver = new ADPropertyDefinition("ExceptIfAttachmentSizeOver", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), "ExceptIfAttachmentSizeOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfMessageSizeOver = new ADPropertyDefinition("ExceptIfMessageSizeOver", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), "ExceptIfMessageSizeOver".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfWithImportance = new ADPropertyDefinition("ExceptIfWithImportance", ExchangeObjectVersion.Exchange2003, typeof(Importance?), "ExceptIfWithImportance".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfMessageTypeMatches = new ADPropertyDefinition("ExceptIfMessageTypeMatches", ExchangeObjectVersion.Exchange2003, typeof(MessageType?), "ExceptIfMessageTypeMatches".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientAddressContainsWords = new ADPropertyDefinition("ExceptIfRecipientAddressContainsWords", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfRecipientAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderInRecipientList = new ADPropertyDefinition("ExceptIfSenderInRecipientList", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfSenderInRecipientList".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientInSenderList = new ADPropertyDefinition("ExceptIfRecipientInSenderList", ExchangeObjectVersion.Exchange2003, typeof(Word[]), "ExceptIfRecipientInSenderList".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfRecipientAddressContainsWords = new ADPropertyDefinition("ExceptIfAnyOfRecipientAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(Word[]), "ExceptIfAnyOfRecipientAddressContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAnyOfRecipientAddressMatchesPatterns = new ADPropertyDefinition("ExceptIfAnyOfRecipientAddressMatchesPatterns", ExchangeObjectVersion.Exchange2010, typeof(Pattern[]), "ExceptIfAnyOfRecipientAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfRecipientAddressMatchesPatterns = new ADPropertyDefinition("ExceptIfRecipientAddressMatchesPatterns", ExchangeObjectVersion.Exchange2003, typeof(Pattern[]), "ExceptIfRecipientAddressMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentContainsWords = new ADPropertyDefinition("ExceptIfAttachmentContainsWords", ExchangeObjectVersion.Exchange2010, typeof(Word[]), "ExceptIfAttachmentContainsWords".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentMatchesPatterns = new ADPropertyDefinition("ExceptIfAttachmentMatchesPatterns", ExchangeObjectVersion.Exchange2010, typeof(Pattern[]), "ExceptIfAttachmentMatchesPatterns".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentIsUnsupported = new ADPropertyDefinition("ExceptIfAttachmentIsUnsupported", ExchangeObjectVersion.Exchange2010, typeof(bool), "ExceptIfAttachmentIsUnsupported".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentProcessingLimitExceeded = new ADPropertyDefinition("ExceptIfAttachmentProcessingLimitExceeded", ExchangeObjectVersion.Exchange2010, typeof(bool), "ExceptIfAttachmentProcessingLimitExceeded".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentHasExecutableContent = new ADPropertyDefinition("ExceptIfAttachmentHasExecutableContent", ExchangeObjectVersion.Exchange2010, typeof(bool), "ExceptIfAttachmentHasExecutableContent".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfAttachmentIsPasswordProtected = new ADPropertyDefinition("ExceptIfAttachmentIsPasswordProtected", ExchangeObjectVersion.Exchange2010, typeof(bool), "ExceptIfAttachmentIsPasswordProtected".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfHasSenderOverride = new ADPropertyDefinition("ExceptIfHasSenderOverride", ExchangeObjectVersion.Exchange2012, typeof(bool), "ExceptIfHasSenderOverride".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfMessageContainsDataClassifications = new ADPropertyDefinition("ExceptIfMessageContainsDataClassifications", ExchangeObjectVersion.Exchange2010, typeof(string[]), "ExceptIfMessageContainsDataClassifications".ToLower(), ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptIfSenderIpRanges = new ADPropertyDefinition("ExceptIfSenderIpRanges", ExchangeObjectVersion.Exchange2010, typeof(IPRange), "ExceptIfSenderIpRanges".ToLower(), ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PrependSubject = new ADPropertyDefinition("PrependSubject", ExchangeObjectVersion.Exchange2003, typeof(string), "PrependSubject".ToLower(), ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SetAuditSeverityLevel = new ADPropertyDefinition("SetAuditSeverity", ExchangeObjectVersion.Exchange2012, typeof(string), "SetAuditSeverity".ToLower(), ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplyClassification = new ADPropertyDefinition("ApplyClassification", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "ApplyClassification".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplyHtmlDisclaimerLocation = new ADPropertyDefinition("ApplyHtmlDisclaimerLocation", ExchangeObjectVersion.Exchange2003, typeof(DisclaimerLocation?), "ApplyHtmlDisclaimerLocation".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplyHtmlDisclaimerText = new ADPropertyDefinition("ApplyHtmlDisclaimerText", ExchangeObjectVersion.Exchange2003, typeof(DisclaimerText?), "ApplyHtmlDisclaimerText".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplyHtmlDisclaimerFallbackAction = new ADPropertyDefinition("ApplyHtmlDisclaimerFallbackAction", ExchangeObjectVersion.Exchange2003, typeof(DisclaimerFallbackAction?), "ApplyHtmlDisclaimerFallbackAction".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplyRightsProtectionTemplate = new ADPropertyDefinition("ApplyRightsProtectionTemplate", ExchangeObjectVersion.Exchange2010, typeof(RmsTemplateIdentity), "ApplyRightsProtectionTemplate".ToLower(), ADPropertyDefinitionFlags.None, new RmsTemplateIdentity(), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SetSCL = new ADPropertyDefinition("SetSCL", ExchangeObjectVersion.Exchange2003, typeof(SclValue?), "SetSCL".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SetHeaderName = new ADPropertyDefinition("SetHeaderName", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "SetHeaderName".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SetHeaderValue = new ADPropertyDefinition("SetHeaderValue", ExchangeObjectVersion.Exchange2003, typeof(HeaderValue?), "SetHeaderValue".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoveHeader = new ADPropertyDefinition("RemoveHeader", ExchangeObjectVersion.Exchange2003, typeof(HeaderName?), "RemoveHeader".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AddToRecipients = new ADPropertyDefinition("AddToRecipients", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "AddToRecipients".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CopyTo = new ADPropertyDefinition("CopyTo", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "CopyTo".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BlindCopyTo = new ADPropertyDefinition("BlindCopyTo", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "BlindCopyTo".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AddManagerAsRecipientType = new ADPropertyDefinition("AddManagerAsRecipientType", ExchangeObjectVersion.Exchange2003, typeof(AddedRecipientType?), "AddManagerAsRecipientType".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ModerateMessageByUser = new ADPropertyDefinition("ModerateMessageByUser", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "ModerateMessageByUser".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ModerateMessageByManager = new ADPropertyDefinition("ModerateMessageByManager", ExchangeObjectVersion.Exchange2003, typeof(bool), "ModerateMessageByManager".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RedirectMessageTo = new ADPropertyDefinition("RedirectMessageTo", ExchangeObjectVersion.Exchange2003, typeof(RecipientIdParameter[]), "RedirectMessageTo".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RejectMessageEnhancedStatusCode = new ADPropertyDefinition("RejectMessageEnhancedStatusCode", ExchangeObjectVersion.Exchange2003, typeof(RejectEnhancedStatus?), "RejectMessageEnhancedStatusCode".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RejectMessageReasonText = new ADPropertyDefinition("RejectMessageReasonText", ExchangeObjectVersion.Exchange2003, typeof(DsnText?), "RejectMessageReasonText".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeleteMessage = new ADPropertyDefinition("DeleteMessage", ExchangeObjectVersion.Exchange2003, typeof(bool), "DeleteMessage".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Disconnect = new ADPropertyDefinition("Disconnect", ExchangeObjectVersion.Exchange2010, typeof(bool), "Disconnect".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Quarantine = new ADPropertyDefinition("Quarantine", ExchangeObjectVersion.Exchange2010, typeof(bool), "Quarantine".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmtpRejectMessageRejectText = new ADPropertyDefinition("SmtpRejectMessageRejectText", ExchangeObjectVersion.Exchange2010, typeof(RejectText?), "SmtpRejectMessageRejectText".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmtpRejectMessageRejectStatusCode = new ADPropertyDefinition("SmtpRejectMessageRejectStatusCode", ExchangeObjectVersion.Exchange2010, typeof(RejectStatusCode?), "SmtpRejectMessageRejectStatusCode".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogEventText = new ADPropertyDefinition("LogEventText", ExchangeObjectVersion.Exchange2010, typeof(EventLogText?), "LogEventText".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2003, typeof(string), "Description".ToLower(), ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition ManuallyModified = new ADPropertyDefinition("manuallyModified", ExchangeObjectVersion.Exchange2003, typeof(bool), "manuallyModified", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition ActivationDate = new ADPropertyDefinition("ActivationDate", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), "ActivationDate".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition ExpiryDate = new ADPropertyDefinition("ExpiryDate", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), "ExpiryDate".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StopRuleProcessing = new ADPropertyDefinition("StopRuleProcessing", ExchangeObjectVersion.Exchange2012, typeof(bool), "StopRuleProcessing".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderNotificationType = new ADPropertyDefinition("NotifySender", ExchangeObjectVersion.Exchange2012, typeof(NotifySenderType?), "NotifySender".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GenerateIncidentReport = new ADPropertyDefinition("GenerateIncidentReport", ExchangeObjectVersion.Exchange2012, typeof(RecipientIdParameter), "GenerateIncidentReport".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IncidentReportOriginalMail = new ADPropertyDefinition("IncidentReportOriginalMail", ExchangeObjectVersion.Exchange2012, typeof(IncidentReportOriginalMail?), "IncidentReportOriginalMail".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IncidentReportContent = new ADPropertyDefinition("IncidentReportContent", ExchangeObjectVersion.Exchange2012, typeof(IncidentReportContent), "IncidentReportContent".ToLower(), ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GenerateNotification = new ADPropertyDefinition("GenerateNotification", ExchangeObjectVersion.Exchange2012, typeof(DisclaimerText?), "GenerateNotification".ToLower(), ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RouteMessageOutboundConnector = new ADPropertyDefinition("RouteMessageOutboundConnector", ExchangeObjectVersion.Exchange2012, typeof(string), "RouteMessageOutboundConnector".ToLower(), ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RouteMessageOutboundRequireTls = new ADPropertyDefinition("RouteMessageOutboundRequireTls", ExchangeObjectVersion.Exchange2012, typeof(bool), "RouteMessageOutboundRequireTls".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EncryptMessage = new ADPropertyDefinition("ApplyOME", ExchangeObjectVersion.Exchange2012, typeof(bool), "ApplyOME".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DecryptMessage = new ADPropertyDefinition("RemoveOME", ExchangeObjectVersion.Exchange2012, typeof(bool), "RemoveOME".ToLower(), ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
