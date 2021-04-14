using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class HygieneFilterRuleSchema : RulePresentationObjectBaseSchema
	{
		public static readonly ADPropertyDefinition Priority = RuleSchema.Priority;

		public static readonly ADPropertyDefinition SentTo = RuleSchema.SentTo;

		public static readonly ADPropertyDefinition SentToMemberOf = RuleSchema.SentToMemberOf;

		public static readonly ADPropertyDefinition RecipientDomainIs = RuleSchema.RecipientDomainIs;

		public static readonly ADPropertyDefinition ExceptIfSentTo = RuleSchema.ExceptIfSentTo;

		public static readonly ADPropertyDefinition ExceptIfSentToMemberOf = RuleSchema.ExceptIfSentToMemberOf;

		public static readonly ADPropertyDefinition ExceptIfRecipientDomainIs = RuleSchema.ExceptIfRecipientDomainIs;

		public static readonly ADPropertyDefinition Conditions = RuleSchema.Conditions;

		public static readonly ADPropertyDefinition Exceptions = RuleSchema.Exceptions;
	}
}
