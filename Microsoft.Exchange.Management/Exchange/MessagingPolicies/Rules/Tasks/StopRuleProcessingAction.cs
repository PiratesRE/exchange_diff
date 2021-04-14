using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("StopRuleProcessing")]
	[Serializable]
	public class StopRuleProcessingAction : TransportRuleAction, IEquatable<StopRuleProcessingAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(StopRuleProcessingAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionStopRuleProcessing;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "Halt")
			{
				return null;
			}
			return new StopRuleProcessingAction();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("Halt", null);
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
