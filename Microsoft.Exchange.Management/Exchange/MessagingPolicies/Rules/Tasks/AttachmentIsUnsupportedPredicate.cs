using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ConditionParameterName("AttachmentIsUnsupported")]
	[ExceptionParameterName("ExceptIfAttachmentIsUnsupported")]
	[Serializable]
	public class AttachmentIsUnsupportedPredicate : TransportRulePredicate, IEquatable<AttachmentIsUnsupportedPredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentIsUnsupportedPredicate)));
		}

		public bool Equals(AttachmentIsUnsupportedPredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAttachmentIsUnsupported;
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("attachmentIsUnsupported"))
			{
				return null;
			}
			return new AttachmentIsUnsupportedPredicate();
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>();
			return TransportRuleParser.Instance.CreatePredicate("attachmentIsUnsupported", null, valueEntries);
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}
	}
}
