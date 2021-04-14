using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ConditionParameterName("HasSenderOverride")]
	[ExceptionParameterName("ExceptIfHasSenderOverride")]
	[Serializable]
	public class HasSenderOverridePredicate : TransportRulePredicate, IEquatable<HasSenderOverridePredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as HasSenderOverridePredicate)));
		}

		public bool Equals(HasSenderOverridePredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionHasSenderOverride;
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public override IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None
				};
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("hasSenderOverride") || !predicateCondition.Property.Name.Equals("X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification"))
			{
				return null;
			}
			return new HasSenderOverridePredicate();
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>();
			TransportRuleParser instance = TransportRuleParser.Instance;
			return instance.CreatePredicate("hasSenderOverride", instance.CreateProperty("Message.Headers:X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification"), valueEntries);
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}

		private const string PropertyBaseName = "X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification";

		private const string PropertyName = "Message.Headers:X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification";
	}
}
