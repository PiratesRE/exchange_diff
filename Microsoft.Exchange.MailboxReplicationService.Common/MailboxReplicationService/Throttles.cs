using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class Throttles
	{
		internal Throttles(ThrottleDurations throttleDurations)
		{
			this.MdbThrottle = this.GetEnhancedTimeSpanFromTicks(throttleDurations.MdbThrottleTicks);
			this.CpuThrottle = this.GetEnhancedTimeSpanFromTicks(throttleDurations.CpuThrottleTicks);
			this.MdbReplicationThrottle = this.GetEnhancedTimeSpanFromTicks(throttleDurations.MdbReplicationThrottleTicks);
			this.ContentIndexingThrottle = this.GetEnhancedTimeSpanFromTicks(throttleDurations.ContentIndexingThrottleTicks);
			this.UnknownThrottle = this.GetEnhancedTimeSpanFromTicks(throttleDurations.UnknownThrottleTicks);
		}

		public EnhancedTimeSpan MdbThrottle { get; set; }

		public EnhancedTimeSpan CpuThrottle { get; set; }

		public EnhancedTimeSpan MdbReplicationThrottle { get; set; }

		public EnhancedTimeSpan ContentIndexingThrottle { get; set; }

		public EnhancedTimeSpan UnknownThrottle { get; set; }

		public override string ToString()
		{
			return MrsStrings.ReportThrottles(this.MdbThrottle.ToString(), this.CpuThrottle.ToString(), this.MdbReplicationThrottle.ToString(), this.ContentIndexingThrottle.ToString(), this.UnknownThrottle.ToString());
		}

		private EnhancedTimeSpan GetEnhancedTimeSpanFromTicks(long ticks)
		{
			return new TimeSpan(ticks - ticks % 10000000L);
		}
	}
}
