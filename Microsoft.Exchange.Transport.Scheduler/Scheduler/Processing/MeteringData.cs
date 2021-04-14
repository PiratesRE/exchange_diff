using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class MeteringData
	{
		public MeteringData(TimeSpan historyLength, TimeSpan historyBucketSize, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyLength", historyLength, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyBucketSize", historyBucketSize, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.totalMemoryCounter = new SlidingTotalCounter(historyLength, historyBucketSize, timeProvider);
			this.totalProcessingCounter = new SlidingTotalCounter(historyLength, historyBucketSize, timeProvider);
			this.timeProvider = timeProvider;
			this.UpdateLastActivityTimestamp();
		}

		public DateTime LastUpdated { get; set; }

		public bool IsEmpty()
		{
			return this.jobCount == 0;
		}

		public UsageData GetUsageData()
		{
			return new UsageData(this.jobCount, this.totalMemoryCounter.Sum, this.totalProcessingCounter.Sum);
		}

		public void AddMemory(long size)
		{
			this.totalMemoryCounter.AddValue(size);
			this.UpdateLastActivityTimestamp();
		}

		public void AddProcessing(long ticks)
		{
			this.totalProcessingCounter.AddValue(ticks);
			this.UpdateLastActivityTimestamp();
		}

		public void IncrementJobCount()
		{
			this.jobCount++;
			this.UpdateLastActivityTimestamp();
		}

		public void DecrementJobCount()
		{
			this.jobCount--;
			this.UpdateLastActivityTimestamp();
		}

		private void UpdateLastActivityTimestamp()
		{
			this.LastUpdated = this.timeProvider();
		}

		private readonly SlidingTotalCounter totalMemoryCounter;

		private readonly SlidingTotalCounter totalProcessingCounter;

		private readonly Func<DateTime> timeProvider;

		private int jobCount;
	}
}
