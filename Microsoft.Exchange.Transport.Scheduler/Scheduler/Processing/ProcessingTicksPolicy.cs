using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class ProcessingTicksPolicy : IThrottlingPolicy
	{
		public ProcessingTicksPolicy(long maximumTicksAllowed)
		{
			this.maximumTicksAllowed = maximumTicksAllowed;
		}

		public PolicyDecision Evaluate(IMessageScope scope, UsageData usage, UsageData totalUsage)
		{
			if (usage != null && usage.ProcessingTicks >= this.maximumTicksAllowed)
			{
				return PolicyDecision.Deny;
			}
			return PolicyDecision.None;
		}

		private readonly long maximumTicksAllowed;
	}
}
