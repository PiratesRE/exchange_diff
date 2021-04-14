using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ExceptionParameterName("ExceptIfAttachmentHasExecutableContent")]
	[ConditionParameterName("AttachmentHasExecutableContent")]
	[Serializable]
	public class AttachmentHasExecutableContentPredicate : TransportRulePredicate, IEquatable<AttachmentHasExecutableContentPredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentHasExecutableContentPredicate)));
		}

		public bool Equals(AttachmentHasExecutableContentPredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAttachmentHasExecutableContent;
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("is") || !predicateCondition.Property.Name.Equals("Message.AttachmentTypes") || predicateCondition.Value.RawValues.Count != 1 || !predicateCondition.Value.RawValues[0].Equals("executable"))
			{
				return null;
			}
			return new AttachmentHasExecutableContentPredicate();
		}

		internal override Condition ToInternalCondition()
		{
			return TransportRuleParser.Instance.CreatePredicate("is", TransportRuleParser.Instance.CreateProperty("Message.AttachmentTypes"), new ShortList<string>
			{
				"executable"
			});
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}
	}
}
