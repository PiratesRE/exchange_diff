using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ExceptionParameterName("ExceptIfAttachmentIsPasswordProtected")]
	[ConditionParameterName("AttachmentIsPasswordProtected")]
	[Serializable]
	public class AttachmentIsPasswordProtectedPredicate : TransportRulePredicate, IEquatable<AttachmentIsPasswordProtectedPredicate>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentIsPasswordProtectedPredicate)));
		}

		public bool Equals(AttachmentIsPasswordProtectedPredicate other)
		{
			return true;
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAttachmentIsPasswordProtected;
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("attachmentIsPasswordProtected"))
			{
				return null;
			}
			return new AttachmentIsPasswordProtectedPredicate();
		}

		internal override Condition ToInternalCondition()
		{
			return TransportRuleParser.Instance.CreatePredicate("attachmentIsPasswordProtected", null, new ShortList<string>());
		}

		internal override string GetPredicateParameters()
		{
			return "$true";
		}
	}
}
