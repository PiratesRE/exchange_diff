using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PolicyTipRulesPerTenantSettings : TransportRulesPerTenantSettings
	{
		protected override RuleParser Parser
		{
			get
			{
				return PolicyTipRuleParser.Instance;
			}
		}

		protected override RuleCollection ParseRules(IEnumerable<TransportRule> adTransportRules, RuleHealthMonitor ruleHealthMonitor)
		{
			RuleCollection ruleCollection = base.ParseRules(adTransportRules, ruleHealthMonitor);
			RuleCollection ruleCollection2 = new RuleCollection(ruleCollection.Name);
			foreach (Rule rule in ruleCollection)
			{
				PolicyTipRule policyTipRule = (PolicyTipRule)rule;
				foreach (Microsoft.Exchange.MessagingPolicies.Rules.Action action in policyTipRule.Actions)
				{
					if (action is SenderNotify)
					{
						if (policyTipRule.ForkConditions != null && policyTipRule.ForkConditions.Count > 0)
						{
							AndCondition andCondition = new AndCondition();
							foreach (Condition item in policyTipRule.ForkConditions)
							{
								andCondition.SubConditions.Add(item);
							}
							andCondition.SubConditions.Add(policyTipRule.Condition);
							policyTipRule.Condition = andCondition;
							policyTipRule.ForkConditions = null;
						}
						ruleCollection2.Add(policyTipRule);
						break;
					}
				}
			}
			return ruleCollection2;
		}
	}
}
