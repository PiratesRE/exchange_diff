using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PerformanceMonitor
	{
		public PerformanceMonitor(string name, ServerManagerLog.Subcomponent component, int minimumRequiredSampleCount, TimeSpan minimumRequiredSamplePeriod)
		{
			this.name = name;
			this.component = component;
			this.minimumRequiredSampleCount = minimumRequiredSampleCount;
			this.minimumRequiredSamplePeriod = minimumRequiredSamplePeriod;
		}

		public void Record(Stopwatch watch)
		{
			if (watch == null)
			{
				throw new ArgumentNullException("watch");
			}
			watch.Stop();
			this.Record(watch.ElapsedMilliseconds);
			watch.Reset();
		}

		internal void Record(long elapsedMilliseconds)
		{
			bool flag = false;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			int num4 = 0;
			TimeSpan timeSpan = TimeSpan.MinValue;
			lock (this.syncObject)
			{
				this.totalCostMilliseconds += elapsedMilliseconds;
				this.sampleCount++;
				if (elapsedMilliseconds > this.highestCost)
				{
					this.highestCost = elapsedMilliseconds;
				}
				if (elapsedMilliseconds < this.lowestCost)
				{
					this.lowestCost = elapsedMilliseconds;
				}
				timeSpan = DateTime.UtcNow - this.lastReset;
				num4 = this.sampleCount;
				if (num4 >= this.minimumRequiredSampleCount && timeSpan >= this.minimumRequiredSamplePeriod)
				{
					num = this.lowestCost;
					num2 = this.highestCost;
					num3 = this.totalCostMilliseconds / (long)this.sampleCount;
					flag = true;
					this.lowestCost = long.MaxValue;
					this.highestCost = 0L;
					this.totalCostMilliseconds = 0L;
					this.sampleCount = 0;
					this.lastReset = DateTime.UtcNow;
				}
			}
			if (flag)
			{
				ServerManagerLog.LogEvent(this.component, ServerManagerLog.EventType.Statistics, null, string.Format("Performance for {0}: AverageCost {1} ms, HighestCost {2} ms, LowestCost {3} ms, SampleCount {4}, SamplePeriod {5}", new object[]
				{
					this.name,
					num3,
					num2,
					num,
					num4,
					timeSpan
				}));
			}
		}

		private readonly object syncObject = new object();

		private readonly int minimumRequiredSampleCount;

		private readonly TimeSpan minimumRequiredSamplePeriod;

		private readonly string name;

		private readonly ServerManagerLog.Subcomponent component;

		private long totalCostMilliseconds;

		private int sampleCount;

		private long highestCost;

		private long lowestCost = long.MaxValue;

		private DateTime lastReset = DateTime.UtcNow;
	}
}
