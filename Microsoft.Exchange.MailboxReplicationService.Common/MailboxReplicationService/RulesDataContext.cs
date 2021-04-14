using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RulesDataContext : DataContext
	{
		public RulesDataContext(RuleData[] rules)
		{
			this.rules = rules;
		}

		public override string ToString()
		{
			return string.Format("Rules: {0}", CommonUtils.ConcatEntries<RuleData>(this.rules, null));
		}

		private RuleData[] rules;
	}
}
