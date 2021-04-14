using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ActionParameterName("RouteMessageOutboundRequireTls")]
	[Serializable]
	public class RouteMessageOutboundRequireTlsAction : TransportRuleAction, IEquatable<RouteMessageOutboundRequireTlsAction>
	{
		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || !(base.GetType() != right.GetType()));
		}

		public bool Equals(RouteMessageOutboundRequireTlsAction other)
		{
			return this.Equals(other);
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRouteMessageOutboundRequireTls;
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RouteMessageOutboundRequireTls")
			{
				return null;
			}
			return new RouteMessageOutboundRequireTlsAction();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("RouteMessageOutboundRequireTls", new ShortList<Argument>(), Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return "$true";
		}
	}
}
