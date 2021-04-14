using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class PredicatesAndActionsWrapper
	{
		internal TransportRulePredicate[] Conditions { get; set; }

		internal TransportRulePredicate[] Exceptions { get; set; }

		internal TransportRuleAction[] Actions { get; set; }

		public PredicatesAndActionsWrapper(TransportRulePredicate[] conditions, TransportRulePredicate[] exceptions, TransportRuleAction[] actions)
		{
			this.Conditions = conditions;
			this.Exceptions = exceptions;
			this.Actions = actions;
		}
	}
}
