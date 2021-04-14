using System;
using System.Globalization;

namespace Microsoft.Exchange.Diagnostics
{
	[Serializable]
	public class PercentileCounter : IPercentileCounter
	{
		public PercentileCounter(TimeSpan expiryInterval, TimeSpan granularityInterval, long valueGranularity, long valueMaximum, CurrentTimeProvider currentTimeProvider)
		{
			if (granularityInterval <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "granularityInterval: [{0}] must be greater than zero.", new object[]
				{
					granularityInterval
				}));
			}
			if (expiryInterval <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "expiryInterval: [{0}] must be greater than zero.", new object[]
				{
					expiryInterval
				}));
			}
			if (expiryInterval != TimeSpan.MaxValue && granularityInterval >= expiryInterval)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "granularityInterval: [{0}] must be less than or equal to expiryInterval: [{1}]", new object[]
				{
					granularityInterval,
					expiryInterval
				}));
			}
			if (valueGranularity <= 0L)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueGranularity: [{0}] must be greater than 0.", new object[]
				{
					valueGranularity
				}));
			}
			if (valueMaximum <= 0L)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueMaximum: [{0}] must be greater than 0.", new object[]
				{
					valueMaximum
				}));
			}
			if (valueGranularity >= valueMaximum)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueGranularity: [{0}] must be less than valueMaximum: [{1}]", new object[]
				{
					valueGranularity,
					valueMaximum
				}));
			}
			this.granularityInterval = granularityInterval;
			this.expiryInterval = expiryInterval;
			if (currentTimeProvider != null)
			{
				this.currentTimeProvider = currentTimeProvider;
			}
			else
			{
				this.currentTimeProvider = new CurrentTimeProvider(this.CurrentTime);
			}
			this.creationTime = this.currentTimeProvider();
			this.mainSummary = new PercentileCounter.PercentileCounterSummary(valueGranularity, valueMaximum);
			if (this.expiryInterval != TimeSpan.MaxValue)
			{
				int num = (int)((this.expiryInterval.Ticks + this.granularityInterval.Ticks - 1L) / this.granularityInterval.Ticks);
				this.summaryBuckets = new PercentileCounter.PercentileCounterSummary[num];
				for (int i = 0; i < num; i++)
				{
					this.summaryBuckets[i] = new PercentileCounter.PercentileCounterSummary(valueGranularity, valueMaximum);
				}
			}
		}

		public PercentileCounter(TimeSpan expiryInterval, TimeSpan granularityInterval, long valueGranularity, long valueMaximum) : this(expiryInterval, granularityInterval, valueGranularity, valueMaximum, null)
		{
		}

		internal double InfiniteBucketPercentage
		{
			get
			{
				double infiniteBucketPercentage;
				lock (this.syncObject)
				{
					this.ExpireBucketsIfNecessary();
					infiniteBucketPercentage = this.mainSummary.InfiniteBucketPercentage;
				}
				return infiniteBucketPercentage;
			}
		}

		public void AddValue(long value)
		{
			lock (this.syncObject)
			{
				this.ExpireBucketsIfNecessary();
				int index = this.mainSummary.FindValueIndex(value);
				this.mainSummary.IncrementValueCount(index);
				if (this.expiryInterval != TimeSpan.MaxValue)
				{
					this.summaryBuckets[this.currentBucket].IncrementValueCount(index);
				}
			}
		}

		public long PercentileQuery(double percentage)
		{
			long num;
			return this.PercentileQuery(percentage, out num);
		}

		public long PercentileQuery(double percentage, out long samples)
		{
			long result;
			lock (this.syncObject)
			{
				this.ExpireBucketsIfNecessary();
				samples = this.mainSummary.TotalNumberOfValues;
				result = this.mainSummary.PercentileQuery(percentage);
			}
			return result;
		}

		protected void RemoveValue(long value)
		{
			if (this.expiryInterval != TimeSpan.MaxValue)
			{
				throw new InvalidOperationException("Cannot remove values when counter has expiration set");
			}
			lock (this.syncObject)
			{
				int index = this.mainSummary.FindValueIndex(value);
				this.mainSummary.DecrementValueCount(index);
			}
		}

		private void ExpireBucketsIfNecessary()
		{
			if (this.expiryInterval != TimeSpan.MaxValue)
			{
				DateTime d = this.currentTimeProvider();
				long num = (d - this.creationTime).Ticks / this.granularityInterval.Ticks;
				if (this.totalBucketsExpired < num)
				{
					this.UpdatePercentileCounterSummary(num - this.totalBucketsExpired);
				}
			}
		}

		private void UpdatePercentileCounterSummary(long numberOfBucketsExpired)
		{
			this.totalBucketsExpired += numberOfBucketsExpired;
			if (numberOfBucketsExpired > (long)this.summaryBuckets.Length)
			{
				numberOfBucketsExpired = (long)this.summaryBuckets.Length;
			}
			while (numberOfBucketsExpired > 0L)
			{
				if (this.mainSummary.TotalNumberOfValues == 0L)
				{
					return;
				}
				this.ExpireBucket();
				numberOfBucketsExpired -= 1L;
			}
		}

		private void ExpireBucket()
		{
			this.currentBucket = (this.currentBucket + 1) % this.summaryBuckets.Length;
			this.mainSummary.SubtractAndClear(this.summaryBuckets[this.currentBucket]);
		}

		private DateTime CurrentTime()
		{
			return DateTime.UtcNow;
		}

		private readonly TimeSpan expiryInterval;

		private readonly TimeSpan granularityInterval;

		private readonly DateTime creationTime;

		private long totalBucketsExpired;

		private PercentileCounter.PercentileCounterSummary mainSummary;

		private PercentileCounter.PercentileCounterSummary[] summaryBuckets;

		private volatile int currentBucket;

		protected object syncObject = new object();

		private CurrentTimeProvider currentTimeProvider;

		[Serializable]
		private class PercentileCounterSummary
		{
			internal PercentileCounterSummary(long valueGranularity, long valueMaximum)
			{
				if (valueGranularity <= 0L)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueGranularity: [{0}] must be greater than 0.", new object[]
					{
						valueGranularity
					}));
				}
				if (valueMaximum <= 0L)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueMaximum: [{0}] must be greater than 0.", new object[]
					{
						valueMaximum
					}));
				}
				if (valueGranularity >= valueMaximum)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "valueGranularity: [{0}] must be less than valueMaximum: [{1}]", new object[]
					{
						valueGranularity,
						valueMaximum
					}));
				}
				this.valueGranularity = valueGranularity;
				int num = (int)((valueMaximum + this.valueGranularity - 1L) / this.valueGranularity) + 1;
				this.data = new long[num];
			}

			internal long TotalNumberOfValues
			{
				get
				{
					return this.totalNumberOfValues;
				}
			}

			internal void IncrementValueCount(int index)
			{
				if (index < 0 || index >= this.data.Length)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Index {0} not in range [0,{1}]", new object[]
					{
						index,
						this.data.Length - 1
					}));
				}
				this.data[index] += 1L;
				this.totalNumberOfValues += 1L;
			}

			internal void DecrementValueCount(int index)
			{
				if (index < 0 || index >= this.data.Length)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Index {0} not in rage [0, {1}]", new object[]
					{
						index,
						this.data.Length - 1
					}));
				}
				if (this.data[index] == 0L)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Index {0} is not present, cannot be removed", new object[]
					{
						index
					}));
				}
				this.data[index] -= 1L;
				this.totalNumberOfValues -= 1L;
			}

			internal int FindValueIndex(long value)
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "value {0} must be non-negative", new object[]
					{
						value
					}));
				}
				long num = value / this.valueGranularity;
				if (num >= (long)this.data.Length)
				{
					num = (long)(this.data.Length - 1);
				}
				return (int)num;
			}

			internal void SubtractAndClear(PercentileCounter.PercentileCounterSummary bucketToRemove)
			{
				if (this.data.Length != bucketToRemove.data.Length)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Bucket to remove is not of required size.", new object[0]));
				}
				if (bucketToRemove.totalNumberOfValues > 0L)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						this.data[i] -= bucketToRemove.data[i];
						bucketToRemove.totalNumberOfValues -= bucketToRemove.data[i];
						this.totalNumberOfValues -= bucketToRemove.data[i];
						bucketToRemove.data[i] = 0L;
						if (bucketToRemove.totalNumberOfValues == 0L)
						{
							break;
						}
					}
				}
				if (bucketToRemove.totalNumberOfValues != 0L)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "bucketToRemove.totalNumberOfValues: [{0}] has non-zero value after values expire.", new object[]
					{
						bucketToRemove.totalNumberOfValues
					}));
				}
				if (this.totalNumberOfValues < 0L)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Number of values in summary: [{0}] can not be negative.", new object[]
					{
						this.totalNumberOfValues
					}));
				}
			}

			internal double InfiniteBucketPercentage
			{
				get
				{
					if (this.TotalNumberOfValues == 0L)
					{
						return 0.0;
					}
					return (double)this.data[this.data.Length - 1] / (double)this.TotalNumberOfValues * 100.0;
				}
			}

			internal long PercentileQuery(double percentage)
			{
				if (percentage < 0.0 || percentage > 100.0)
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "percentage:[{0}] must be in range [0,100]", new object[]
					{
						percentage
					}));
				}
				long num = (long)(percentage * (double)this.totalNumberOfValues / 100.0);
				long num2 = 0L;
				int i;
				for (i = 0; i < this.data.Length; i++)
				{
					num2 += this.data[i];
					if (num <= num2)
					{
						return this.valueGranularity * (long)i;
					}
				}
				if (i == this.data.Length)
				{
					i--;
				}
				return this.valueGranularity * (long)i;
			}

			private long[] data;

			private long valueGranularity;

			private long totalNumberOfValues;
		}
	}
}
