using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRuleCollection : RuleCollection
	{
		internal ClientAccessRuleCollection(string name) : base(name)
		{
		}

		public ClientAccessRulesExecutionStatus Run(ClientAccessRulesEvaluationContext context)
		{
			RulesEvaluator rulesEvaluator = new RulesEvaluator(context);
			rulesEvaluator.Run();
			return ClientAccessRulesExecutionStatus.Success;
		}

		internal void AddClientAccessRuleCollection(ClientAccessRuleCollection rules)
		{
			foreach (Rule rule in rules)
			{
				ClientAccessRule rule2 = (ClientAccessRule)rule;
				base.AddWithoutNameCheck(rule2);
			}
		}
	}
}
