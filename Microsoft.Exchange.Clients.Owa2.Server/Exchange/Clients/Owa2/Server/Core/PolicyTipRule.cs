using System;
using System.Collections.Generic;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class PolicyTipRule : Rule
	{
		public PolicyTipRule(string name) : base(name)
		{
		}

		public List<Condition> ForkConditions
		{
			get
			{
				return this.forkConditions;
			}
			set
			{
				this.forkConditions = value;
			}
		}

		private List<Condition> forkConditions;
	}
}
