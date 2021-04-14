using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class UsageData
	{
		public UsageData(int outstandingJobs, long memoryUsed, long processingTicks)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("oustrandingJobs", outstandingJobs, 0, int.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<long>("memoryUsed", memoryUsed, 0L, long.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<long>("processingTicks", processingTicks, 0L, long.MaxValue);
			this.outstandingJobs = outstandingJobs;
			this.memoryUsed = memoryUsed;
			this.processingTicks = processingTicks;
		}

		public int OutstandingJobs
		{
			get
			{
				return this.outstandingJobs;
			}
		}

		public long MemoryUsed
		{
			get
			{
				return this.memoryUsed;
			}
		}

		public long ProcessingTicks
		{
			get
			{
				return this.processingTicks;
			}
		}

		public static readonly UsageData EmptyUsage = new UsageData(0, 0L, 0L);

		private readonly int outstandingJobs;

		private readonly long memoryUsed;

		private readonly long processingTicks;
	}
}
