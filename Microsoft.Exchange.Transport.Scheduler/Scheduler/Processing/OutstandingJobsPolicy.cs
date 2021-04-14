using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class OutstandingJobsPolicy : IThrottlingPolicy
	{
		public OutstandingJobsPolicy(int maxJobsAllowed, double allowedFactor)
		{
			this.allowedFactor = allowedFactor;
			this.maxJobsAllowed = maxJobsAllowed;
		}

		public PolicyDecision Evaluate(IMessageScope scope, UsageData usage, UsageData totalUsage)
		{
			if (totalUsage != null && usage != null)
			{
				long num = (long)(this.allowedFactor * (double)Math.Min(0, this.maxJobsAllowed - totalUsage.OutstandingJobs));
				if ((long)usage.OutstandingJobs >= num)
				{
					return PolicyDecision.Deny;
				}
			}
			return PolicyDecision.None;
		}

		private readonly int maxJobsAllowed;

		private readonly double allowedFactor;
	}
}
