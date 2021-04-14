using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class InMemorySchedulerMetering : RefreshableComponent, ISchedulerMetering
	{
		public InMemorySchedulerMetering(TimeSpan usageDataTtl, TimeSpan historyLength, TimeSpan historyBucketSize, TimeSpan updateInterval, Func<DateTime> timeProvider) : base(updateInterval, timeProvider)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("usageDataTtl", usageDataTtl, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyLength", historyLength, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyBucketSize", historyBucketSize, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			this.usageDataTtl = usageDataTtl;
			this.historyLength = historyLength;
			this.historyBucketSize = historyBucketSize;
			this.totalUsageData = new MeteringData(this.historyLength, this.historyBucketSize, timeProvider);
		}

		public InMemorySchedulerMetering(TimeSpan usageDataTtl, TimeSpan historyLength, TimeSpan historyBucketSize, TimeSpan updateInterval) : this(usageDataTtl, historyLength, historyBucketSize, updateInterval, () => DateTime.UtcNow)
		{
		}

		public UsageData GetTotalUsage()
		{
			return this.totalUsageData.GetUsageData();
		}

		public void RecordStart(IEnumerable<IMessageScope> scopes, long memorySize)
		{
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			ArgumentValidator.ThrowIfInvalidValue<long>("memorySize", memorySize, (long l) => l >= 0L);
			this.totalUsageData.IncrementJobCount();
			this.totalUsageData.AddMemory(memorySize);
			foreach (IMessageScope scope in scopes)
			{
				this.RecordStart(scope, memorySize);
			}
		}

		public void RecordEnd(IEnumerable<IMessageScope> scopes, TimeSpan duration)
		{
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("duration", duration, TimeSpan.Zero, TimeSpan.MaxValue);
			this.totalUsageData.DecrementJobCount();
			this.totalUsageData.AddProcessing(duration.Ticks);
			foreach (IMessageScope scope in scopes)
			{
				this.RecordEnd(scope, duration);
			}
		}

		public bool TryGetUsage(IMessageScope scope, out UsageData data)
		{
			MeteringData meteringData;
			if (this.perScopeData.TryGetValue(scope, out meteringData))
			{
				data = meteringData.GetUsageData();
				return true;
			}
			data = null;
			return false;
		}

		protected override void Refresh(DateTime timestamp)
		{
			DateTime t = timestamp - this.usageDataTtl;
			List<IMessageScope> list = new List<IMessageScope>();
			foreach (KeyValuePair<IMessageScope, MeteringData> keyValuePair in this.perScopeData)
			{
				if (keyValuePair.Value.IsEmpty() && keyValuePair.Value.LastUpdated < t)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (IMessageScope key in list)
			{
				this.perScopeData.Remove(key);
			}
		}

		private void RecordStart(IMessageScope scope, long memorySize)
		{
			MeteringData meteringData;
			if (!this.perScopeData.TryGetValue(scope, out meteringData))
			{
				meteringData = new MeteringData(this.historyLength, this.historyBucketSize, base.TimeProvider);
				this.perScopeData.Add(scope, meteringData);
			}
			meteringData.IncrementJobCount();
			meteringData.AddMemory(memorySize);
		}

		private void RecordEnd(IMessageScope scope, TimeSpan duration)
		{
			MeteringData meteringData;
			if (this.perScopeData.TryGetValue(scope, out meteringData))
			{
				meteringData.DecrementJobCount();
				meteringData.AddProcessing(duration.Ticks);
				return;
			}
			throw new InvalidOperationException(string.Format("No metering data found for expected scope {0}", scope));
		}

		private readonly MeteringData totalUsageData;

		private readonly TimeSpan historyLength;

		private readonly TimeSpan historyBucketSize;

		private readonly TimeSpan usageDataTtl;

		private readonly IDictionary<IMessageScope, MeteringData> perScopeData = new Dictionary<IMessageScope, MeteringData>();
	}
}
