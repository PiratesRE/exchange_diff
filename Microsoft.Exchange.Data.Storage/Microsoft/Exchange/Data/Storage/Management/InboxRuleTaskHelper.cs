using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class InboxRuleTaskHelper
	{
		public static ulong GetRuleIdentity(RuleId ruleId)
		{
			if (ruleId == null)
			{
				throw new ArgumentNullException("ruleId");
			}
			return (ulong)ruleId.StoreRuleId;
		}

		public static void SetRuleId(Rule rule, RuleId ruleId)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (ruleId == null)
			{
				throw new ArgumentNullException("ruleId");
			}
			rule.Id = ruleId;
		}

		public static void SetRuleSequence(Rule rule, int newSequence)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			rule.Sequence = newSequence;
		}
	}
}
