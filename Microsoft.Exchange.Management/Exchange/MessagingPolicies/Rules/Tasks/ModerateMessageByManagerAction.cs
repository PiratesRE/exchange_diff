using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("ModerateMessageByManager")]
	[Serializable]
	public class ModerateMessageByManagerAction : TransportRuleAction, IEquatable<ModerateMessageByManagerAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(ModerateMessageByManagerAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionModerateMessageByManager;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "ModerateMessageByManager")
			{
				return null;
			}
			return new ModerateMessageByManagerAction();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("ModerateMessageByManager", Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
