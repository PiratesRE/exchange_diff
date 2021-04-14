using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("DeleteMessage")]
	[Serializable]
	public class DeleteMessageAction : TransportRuleAction, IEquatable<DeleteMessageAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(DeleteMessageAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionDeleteMessage;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "DeleteMessage")
			{
				return null;
			}
			return new DeleteMessageAction();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("DeleteMessage", Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
