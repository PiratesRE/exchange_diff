using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ConditionParameterName("AttachmentProcessingLimitExceeded")]
	[ExceptionParameterName("ExceptIfAttachmentProcessingLimitExceeded")]
	[Serializable]
	public class AttachmentProcessingLimitExceededPredicate : TransportRulePredicate, IEquatable<AttachmentProcessingLimitExceededPredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentProcessingLimitExceededPredicate)));
		}

		public bool Equals(AttachmentProcessingLimitExceededPredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAttachmentProcessingLimitExceeded;
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("attachmentProcessingLimitExceeded"))
			{
				return null;
			}
			return new AttachmentProcessingLimitExceededPredicate();
		}

		internal override Condition ToInternalCondition()
		{
			return TransportRuleParser.Instance.CreatePredicate("attachmentProcessingLimitExceeded", null, new ShortList<string>());
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}
	}
}
