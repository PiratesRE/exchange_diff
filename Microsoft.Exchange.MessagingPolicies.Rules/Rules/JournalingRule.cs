using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class JournalingRule : TransportRule
	{
		public JournalingRule(string name) : this(name, null)
		{
			this.gccRuleType = GccType.None;
		}

		public JournalingRule(string name, Condition condition) : base(name, condition)
		{
			this.gccRuleType = GccType.None;
		}

		public GccType GccRuleType
		{
			get
			{
				return this.gccRuleType;
			}
			set
			{
				this.gccRuleType = value;
			}
		}

		public override int GetEstimatedSize()
		{
			int num = 0;
			num += 4;
			return num + base.GetEstimatedSize();
		}

		private GccType gccRuleType;
	}
}
