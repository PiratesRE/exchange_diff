using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ConditionParameterName("HasNoClassification")]
	[ExceptionParameterName("ExceptIfHasNoClassification")]
	[Serializable]
	public class HasNoClassificationPredicate : TransportRulePredicate, IEquatable<HasNoClassificationPredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as HasNoClassificationPredicate)));
		}

		public bool Equals(HasNoClassificationPredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionHasNoClassification;
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!(predicateCondition.Property is HeaderProperty))
			{
				return null;
			}
			if (!predicateCondition.Name.Equals("notExists") || !predicateCondition.Property.Name.Equals("X-MS-Exchange-Organization-Classification"))
			{
				return null;
			}
			return new HasNoClassificationPredicate();
		}

		internal override Condition ToInternalCondition()
		{
			Property property = TransportRuleParser.Instance.CreateProperty("Message.Headers:X-MS-Exchange-Organization-Classification");
			ShortList<string> valueEntries = new ShortList<string>();
			return TransportRuleParser.Instance.CreatePredicate("notExists", property, valueEntries);
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}
	}
}
