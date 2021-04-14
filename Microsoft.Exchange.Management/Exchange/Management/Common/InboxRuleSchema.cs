using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Management.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InboxRuleSchema : XsoMailboxConfigurationObjectSchema
	{
		public const string DescriptionTimeZoneParameterName = "DescriptionTimeZone";

		public const string DescriptionTimeFormatParameterName = "DescriptionTimeFormat";

		public const string EnabledParameterName = "Enabled";

		public const string IdentityParameterName = "Identity";

		public const string InErrorParameterName = "InError";

		public const string MailboxParameterName = "Mailbox";

		public const string NameParameterName = "Name";

		public const string PriorityParameterName = "Priority";

		public const string RuleIdParameterName = "RuleId";

		public const string RuleIdentityParameterName = "RuleIdentity";

		public const string SupportedByTaskParameterName = "SupportedByTask";

		public const string ExceptIf = "ExceptIf";

		public const string BodyContainsWordsParameterName = "BodyContainsWords";

		public const string FlaggedForActionParameterName = "FlaggedForAction";

		public const string FromParameterName = "From";

		public const string FromAddressContainsWordsParameterName = "FromAddressContainsWords";

		public const string HasAttachmentParameterName = "HasAttachment";

		public const string HasClassificationParameterName = "HasClassification";

		public const string HeaderContainsWordsParameterName = "HeaderContainsWords";

		public const string FromSubscriptionParameterName = "FromSubscription";

		public const string MessageTypeMatchesParameterName = "MessageTypeMatches";

		public const string MyNameInCcBoxParameterName = "MyNameInCcBox";

		public const string MyNameInToBoxParameterName = "MyNameInToBox";

		public const string MyNameInToOrCcBoxParameterName = "MyNameInToOrCcBox";

		public const string MyNameNotInToBoxParameterName = "MyNameNotInToBox";

		public const string ReceivedAfterDateParameterName = "ReceivedAfterDate";

		public const string ReceivedBeforeDateParameterName = "ReceivedBeforeDate";

		public const string RecipientAddressContainsWordsParameterName = "RecipientAddressContainsWords";

		public const string SentOnlyToMeParameterName = "SentOnlyToMe";

		public const string SentToParameterName = "SentTo";

		public const string SubjectContainsWordsParameterName = "SubjectContainsWords";

		public const string SubjectOrBodyContainsWordsParameterName = "SubjectOrBodyContainsWords";

		public const string WithImportanceParameterName = "WithImportance";

		public const string WithinSizeRangeMaximumParameterName = "WithinSizeRangeMaximum";

		public const string WithinSizeRangeMinimumParameterName = "WithinSizeRangeMinimum";

		public const string WithSensitivityParameterName = "WithSensitivity";

		public const string ApplyCategoryParameterName = "ApplyCategory";

		public const string CopyToFolderParameterName = "CopyToFolder";

		public const string DeleteMessageParameterName = "DeleteMessage";

		public const string ForwardAsAttachmentToParameterName = "ForwardAsAttachmentTo";

		public const string ForwardToParameterName = "ForwardTo";

		public const string MarkAsReadParameterName = "MarkAsRead";

		public const string MarkImportanceParameterName = "MarkImportance";

		public const string MoveToFolderParameterName = "MoveToFolder";

		public const string RedirectToParameterName = "RedirectTo";

		public const string SendTextMessageNotificationToParameterName = "SendTextMessageNotificationTo";

		public const string StopProcessingRulesParameterName = "StopProcessingRules";

		public const int MinimumSizeConstraint = 0;

		public const int MaximumSizeConstraint = 999999;

		public const int MaximumPriority = 2147483637;

		public const int MaximumWordLengthConstraint = 255;

		public static readonly SimpleProviderPropertyDefinition BodyContainsWords = new SimpleProviderPropertyDefinition("BodyContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfBodyContainsWords = new SimpleProviderPropertyDefinition("ExceptIfBodyContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition FlaggedForAction = new SimpleProviderPropertyDefinition("FlaggedForAction", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfFlaggedForAction = new SimpleProviderPropertyDefinition("ExceptIfFlaggedForAction", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition From = new SimpleProviderPropertyDefinition("From", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfFrom = new SimpleProviderPropertyDefinition("ExceptIfFrom", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FromAddressContainsWords = new SimpleProviderPropertyDefinition("FromAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfFromAddressContainsWords = new SimpleProviderPropertyDefinition("ExceptIfFromAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition HasAttachment = new SimpleProviderPropertyDefinition("HasAttachment", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfHasAttachment = new SimpleProviderPropertyDefinition("ExceptIfHasAttachment", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition HasClassification = new SimpleProviderPropertyDefinition("HasClassification", ExchangeObjectVersion.Exchange2010, typeof(MessageClassification[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfHasClassification = new SimpleProviderPropertyDefinition("ExceptIfHasClassification", ExchangeObjectVersion.Exchange2010, typeof(MessageClassification[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition HeaderContainsWords = new SimpleProviderPropertyDefinition("HeaderContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfHeaderContainsWords = new SimpleProviderPropertyDefinition("ExceptIfHeaderContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition FromSubscription = new SimpleProviderPropertyDefinition("FromSubscription", ExchangeObjectVersion.Exchange2010, typeof(AggregationSubscriptionIdentity[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfFromSubscription = new SimpleProviderPropertyDefinition("ExceptIfFromSubscription", ExchangeObjectVersion.Exchange2010, typeof(AggregationSubscriptionIdentity[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MessageTypeMatches = new SimpleProviderPropertyDefinition("MessageTypeMatches", ExchangeObjectVersion.Exchange2010, typeof(InboxRuleMessageType?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfMessageTypeMatches = new SimpleProviderPropertyDefinition("ExceptIfMessageTypeMatches", ExchangeObjectVersion.Exchange2010, typeof(InboxRuleMessageType?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MyNameInCcBox = new SimpleProviderPropertyDefinition("MyNameInCcBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfMyNameInCcBox = new SimpleProviderPropertyDefinition("ExceptIfMyNameInCcBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MyNameInToBox = new SimpleProviderPropertyDefinition("MyNameInToBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfMyNameInToBox = new SimpleProviderPropertyDefinition("ExceptIfMyNameInToBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MyNameInToOrCcBox = new SimpleProviderPropertyDefinition("MyNameInToOrCcBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfMyNameInToOrCcBox = new SimpleProviderPropertyDefinition("ExceptIfMyNameInToOrCcBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MyNameNotInToBox = new SimpleProviderPropertyDefinition("MyNameNotInToBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfMyNameNotInToBox = new SimpleProviderPropertyDefinition("ExceptIfMyNameNotInToBox", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ReceivedAfterDate = new SimpleProviderPropertyDefinition("ReceivedAfterDate", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfReceivedAfterDate = new SimpleProviderPropertyDefinition("ExceptIfReceivedAfterDate", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ReceivedBeforeDate = new SimpleProviderPropertyDefinition("ReceivedBeforeDate", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfReceivedBeforeDate = new SimpleProviderPropertyDefinition("ExceptIfReceivedBeforeDate", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientAddressContainsWords = new SimpleProviderPropertyDefinition("RecipientAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfRecipientAddressContainsWords = new SimpleProviderPropertyDefinition("ExceptIfRecipientAddressContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition SentOnlyToMe = new SimpleProviderPropertyDefinition("SentOnlyToMe", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfSentOnlyToMe = new SimpleProviderPropertyDefinition("ExceptIfSentOnlyToMe", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SentTo = new SimpleProviderPropertyDefinition("SentTo", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfSentTo = new SimpleProviderPropertyDefinition("ExceptIfSentTo", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SubjectContainsWords = new SimpleProviderPropertyDefinition("SubjectContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfSubjectContainsWords = new SimpleProviderPropertyDefinition("ExceptIfSubjectContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition SubjectOrBodyContainsWords = new SimpleProviderPropertyDefinition("SubjectOrBodyContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfSubjectOrBodyContainsWords = new SimpleProviderPropertyDefinition("ExceptIfSubjectOrBodyContainsWords", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition WithImportance = new SimpleProviderPropertyDefinition("WithImportance", ExchangeObjectVersion.Exchange2010, typeof(Importance?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfWithImportance = new SimpleProviderPropertyDefinition("ExceptIfWithImportance", ExchangeObjectVersion.Exchange2010, typeof(Importance?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WithinSizeRangeMaximum = new SimpleProviderPropertyDefinition("WithinSizeRangeMaximum", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfWithinSizeRangeMaximum = new SimpleProviderPropertyDefinition("ExceptIfWithinSizeRangeMaximum", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		});

		public static readonly SimpleProviderPropertyDefinition WithinSizeRangeMinimum = new SimpleProviderPropertyDefinition("WithinSizeRangeMinimum", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		});

		public static readonly SimpleProviderPropertyDefinition ExceptIfWithinSizeRangeMinimum = new SimpleProviderPropertyDefinition("ExceptIfWithinSizeRangeMinimum", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		});

		public static readonly SimpleProviderPropertyDefinition WithSensitivity = new SimpleProviderPropertyDefinition("WithSensitivity", ExchangeObjectVersion.Exchange2010, typeof(Sensitivity?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExceptIfWithSensitivity = new SimpleProviderPropertyDefinition("ExceptIfWithSensitivity", ExchangeObjectVersion.Exchange2010, typeof(Sensitivity?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ApplyCategory = new SimpleProviderPropertyDefinition("ApplyCategory", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255)
		});

		public static readonly SimpleProviderPropertyDefinition CopyToFolder = new SimpleProviderPropertyDefinition("CopyToFolder", ExchangeObjectVersion.Exchange2010, typeof(MailboxFolder), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeleteMessage = new SimpleProviderPropertyDefinition("DeleteMessage", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ForwardAsAttachmentTo = new SimpleProviderPropertyDefinition("ForwardAsAttachmentTo", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ForwardTo = new SimpleProviderPropertyDefinition("ForwardTo", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MarkAsRead = new SimpleProviderPropertyDefinition("MarkAsRead", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MarkImportance = new SimpleProviderPropertyDefinition("MarkImportance", ExchangeObjectVersion.Exchange2010, typeof(Importance?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MoveToFolder = new SimpleProviderPropertyDefinition("MoveToFolder", ExchangeObjectVersion.Exchange2010, typeof(MailboxFolder), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RedirectTo = new SimpleProviderPropertyDefinition("RedirectTo", ExchangeObjectVersion.Exchange2010, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SendTextMessageNotificationTo = new SimpleProviderPropertyDefinition("SendTextMessageNotificationTo", ExchangeObjectVersion.Exchange2010, typeof(E164Number), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition StopProcessingRules = new SimpleProviderPropertyDefinition("StopProcessingRules", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Enabled = new SimpleProviderPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition InError = new SimpleProviderPropertyDefinition("InError", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 512)
		});

		public static readonly SimpleProviderPropertyDefinition Priority = new SimpleProviderPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 2147483637)
		});

		public static readonly SimpleProviderPropertyDefinition RuleId = new SimpleProviderPropertyDefinition("RuleId", ExchangeObjectVersion.Exchange2010, typeof(RuleId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SupportedByTask = new SimpleProviderPropertyDefinition("SupportedByTask", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(InboxRuleId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			InboxRuleSchema.Name,
			InboxRuleSchema.RuleId,
			XsoMailboxConfigurationObjectSchema.MailboxOwnerId
		}, null, new GetterDelegate(InboxRule.IdentityGetter), null);

		public static readonly SimpleProviderPropertyDefinition RuleIdentity = new SimpleProviderPropertyDefinition("RuleIdentity", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			InboxRuleSchema.RuleId
		}, null, new GetterDelegate(InboxRule.RuleIdentityGetter), null);

		public static readonly SimpleProviderPropertyDefinition MachineToPersonTextMessagingDisabled = new SimpleProviderPropertyDefinition("MachineToPersonTextMessagingDisabled", ExchangeObjectVersion.Exchange2010, typeof(object), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition StoreObjectInError = new SimpleProviderPropertyDefinition("StoreObjectInError", ExchangeObjectVersion.Exchange2010, typeof(object), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
