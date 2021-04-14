using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	public class SlidingTotalCounter : SlidingWindow
	{
		public SlidingTotalCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength) : this(slidingWindowLength, bucketLength, () => DateTime.UtcNow)
		{
		}

		internal SlidingTotalCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider) : base(slidingWindowLength, bucketLength, currentTimeProvider)
		{
			this.valueBuckets = new long[this.NumberOfBuckets];
		}

		public long Sum
		{
			get
			{
				long result;
				lock (this.syncObject)
				{
					base.ExpireBucketsIfNecessary();
					result = this.valueTotal;
				}
				return result;
			}
		}

		public long LastTotalValue
		{
			get
			{
				return this.valueTotal;
			}
		}

		public IEnumerable<long> PastTotalValues
		{
			get
			{
				List<long> buckets = new List<long>(this.valueBuckets);
				int startIndex = 0;
				lock (this.syncObject)
				{
					startIndex = base.OldestBucketIndex % buckets.Count;
				}
				int curr = startIndex;
				long sum = 0L;
				for (int i = curr; i < buckets.Count; i++)
				{
					sum += buckets[i];
					yield return sum;
				}
				for (int j = 0; j < startIndex; j++)
				{
					sum += buckets[j];
					yield return sum;
				}
				yield break;
			}
		}

		protected override Array ValueBuckets
		{
			get
			{
				return this.valueBuckets;
			}
		}

		public long AddValue(long value)
		{
			return this.ExpireAndUpdate(value);
		}

		private long ExpireAndUpdate(long value)
		{
			long result;
			lock (this.syncObject)
			{
				base.ExpireBucketsIfNecessary();
				this.valueTotal += value;
				this.valueBuckets[this.currentBucket] += value;
				base.SetLastUpdateTime();
				result = this.valueTotal;
			}
			return result;
		}

		protected override void ExpireBucket(int bucket)
		{
			this.valueTotal -= this.valueBuckets[this.currentBucket];
			this.valueBuckets[this.currentBucket] = 0L;
		}

		public SlidingTotalCounter Merge(SlidingTotalCounter other)
		{
			ArgumentValidator.ThrowIfNull("other", other);
			if (!this.BucketLength.Equals(other.BucketLength))
			{
				throw new InvalidOperationException("Cannot merge two slidingTotalCounters with different values for BucketLength");
			}
			if (this.NumberOfBuckets != other.NumberOfBuckets)
			{
				throw new InvalidOperationException("Cannot merge two slidingTotalCounters with different values for NumberOfBuckets");
			}
			lock (this.syncObject)
			{
				base.ExpireBucketsIfNecessary();
				other.ExpireBucketsIfNecessary();
				for (int i = 0; i < this.valueBuckets.Length; i++)
				{
					this.valueBuckets[i] += other.valueBuckets[i];
					this.valueTotal += other.valueBuckets[i];
				}
			}
			return this;
		}

		private long valueTotal;

		private readonly long[] valueBuckets;

		private readonly object syncObject = new object();
	}
}
