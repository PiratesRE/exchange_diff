using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal class SchedulerDiagnosticsInfo
	{
		public long Received { get; set; }

		public long Dispatched { get; set; }

		public long Throttled { get; set; }

		public long Processed { get; set; }

		public long ReceiveRate { get; set; }

		public long DispatchRate { get; set; }

		public long ThrottleRate { get; set; }

		public long ProcessRate { get; set; }

		public long TotalScopedQueues { get; set; }

		public long ScopedQueuesCreatedRate { get; set; }

		public long ScopedQueuesDestroyedRate { get; set; }

		public DateTime OldestScopedQueueCreateTime { get; set; }

		public DateTime OldestLockTimeStamp { get; set; }

		public double DispatchingVelocity
		{
			get
			{
				return (double)(this.DispatchRate - this.ReceiveRate) * 1.0 / 60.0;
			}
		}

		public double ProcessingVelocity
		{
			get
			{
				return (double)(this.ProcessRate - this.ReceiveRate) * 1.0 / 60.0;
			}
		}
	}
}
