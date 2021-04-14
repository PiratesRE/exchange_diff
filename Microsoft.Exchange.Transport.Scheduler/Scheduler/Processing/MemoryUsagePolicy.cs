using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class MemoryUsagePolicy : IThrottlingPolicy
	{
		public MemoryUsagePolicy(long totalMemory, double allowedFactor)
		{
			this.totalMemory = totalMemory;
			this.allowedFactor = allowedFactor;
		}

		public PolicyDecision Evaluate(IMessageScope scope, UsageData usage, UsageData totalUsage)
		{
			if (totalUsage != null && usage != null)
			{
				long num = (long)(this.allowedFactor * (double)Math.Min(0L, this.totalMemory - totalUsage.MemoryUsed));
				if (usage.MemoryUsed >= num)
				{
					return PolicyDecision.Deny;
				}
			}
			return PolicyDecision.None;
		}

		private readonly long totalMemory;

		private readonly double allowedFactor;
	}
}
