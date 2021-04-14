using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("Quarantine")]
	[Serializable]
	public class QuarantineAction : TransportRuleAction, IEquatable<QuarantineAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(QuarantineAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionQuarantine;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "Quarantine")
			{
				return null;
			}
			return new QuarantineAction();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("Quarantine", Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
