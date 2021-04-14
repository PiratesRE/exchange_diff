using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class SlidingAverageCounter : SlidingWindow
	{
		public SlidingAverageCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength) : this(slidingWindowLength, bucketLength, () => DateTime.UtcNow)
		{
		}

		public SlidingAverageCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider) : base(slidingWindowLength, bucketLength, currentTimeProvider)
		{
			this.valueBuckets = new SlidingAverageCounter.CounterData[this.NumberOfBuckets];
		}

		protected override Array ValueBuckets
		{
			get
			{
				return this.valueBuckets;
			}
		}

		public void AddValue(long value)
		{
			this.ExpireAndUpdate(value);
			base.SetLastUpdateTime();
		}

		public long CalculateAverage()
		{
			long result;
			lock (this.syncObject)
			{
				base.ExpireBucketsIfNecessary();
				if (this.bucketsFilled == 0)
				{
					result = 0L;
				}
				else
				{
					result = this.valueTotal / (long)this.bucketsFilled;
				}
			}
			return result;
		}

		public long CalculateAverageAcrossAllSamples(out long numberOfSamples)
		{
			long result;
			lock (this.syncObject)
			{
				base.ExpireBucketsIfNecessary();
				numberOfSamples = this.numberTotal;
				if (0L == this.numberTotal)
				{
					result = 0L;
				}
				else
				{
					result = this.valueTotal / this.numberTotal;
				}
			}
			return result;
		}

		private void ExpireAndUpdate(long value)
		{
			lock (this.syncObject)
			{
				if (this.bucketsFilled == 0)
				{
					this.bucketsFilled = 1;
				}
				base.ExpireBucketsIfNecessary();
				this.valueTotal += value;
				this.numberTotal += 1L;
				this.valueBuckets[this.currentBucket].AddValue(value);
			}
		}

		protected override void ExpireBucket(int bucket)
		{
			if (this.valueBuckets.Length > this.bucketsFilled)
			{
				this.bucketsFilled++;
			}
			this.valueTotal -= this.valueBuckets[this.currentBucket].TotalValue;
			this.numberTotal -= this.valueBuckets[this.currentBucket].TotalNumber;
			this.valueBuckets[this.currentBucket].Reset();
		}

		private long valueTotal;

		private long numberTotal;

		private SlidingAverageCounter.CounterData[] valueBuckets;

		private int bucketsFilled;

		private object syncObject = new object();

		private struct CounterData
		{
			public void AddValue(long value)
			{
				this.totalValue += value;
				this.totalNumber += 1L;
			}

			public long TotalValue
			{
				get
				{
					return this.totalValue;
				}
			}

			public long TotalNumber
			{
				get
				{
					return this.totalNumber;
				}
			}

			public long AverageValue
			{
				get
				{
					if (this.totalNumber == 0L)
					{
						return 0L;
					}
					return this.totalValue / this.totalNumber;
				}
			}

			public void Reset()
			{
				this.totalValue = 0L;
				this.totalNumber = 0L;
			}

			private long totalValue;

			private long totalNumber;
		}
	}
}
