using System;
using System.Collections.Generic;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal sealed class HygieneTransportRule : Rule
	{
		public HygieneTransportRule(string name) : this(name, null)
		{
		}

		public HygieneTransportRule(string name, Condition condition) : base(name, condition)
		{
			this.Fork = new List<BifurcationInfo>();
		}

		public List<BifurcationInfo> Fork { get; set; }

		public override int GetEstimatedSize()
		{
			int num = 0;
			foreach (BifurcationInfo bifurcationInfo in this.Fork)
			{
				num += bifurcationInfo.GetEstimatedSize();
			}
			return num + base.GetEstimatedSize();
		}
	}
}
