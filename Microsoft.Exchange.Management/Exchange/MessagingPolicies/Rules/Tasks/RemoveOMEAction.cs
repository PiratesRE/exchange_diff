using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("RemoveOME")]
	[Serializable]
	public class RemoveOMEAction : TransportRuleAction, IEquatable<RemoveOMEAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(RemoveOMEAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRemoveOME;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RemoveOME")
			{
				return null;
			}
			return new RemoveOMEAction();
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value("X-MS-Exchange-Organization-E4eDecryptMessage"),
				new Value("true")
			};
			return TransportRuleParser.Instance.CreateAction("RemoveOME", arguments, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
