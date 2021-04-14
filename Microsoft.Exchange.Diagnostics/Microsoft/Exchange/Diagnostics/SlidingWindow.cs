using System;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class SlidingWindow
	{
		protected SlidingWindow(TimeSpan slidingWindowLength, TimeSpan bucketLength) : this(slidingWindowLength, bucketLength, () => DateTime.UtcNow)
		{
		}

		protected SlidingWindow(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider)
		{
			SlidingWindow.ValidateSlidingWindowAndBucketLength(slidingWindowLength, bucketLength);
			ExAssert.RetailAssert(currentTimeProvider != null, "Current time provider should not be null.");
			this.BucketLength = bucketLength;
			this.currentTimeProvider = currentTimeProvider;
			this.creationTime = this.currentTimeProvider();
			this.NumberOfBuckets = (int)(slidingWindowLength.Ticks / this.BucketLength.Ticks);
			this.filled = new bool[this.NumberOfBuckets];
		}

		public bool IsEmpty
		{
			get
			{
				return this.GetTargetBucket(this.currentTimeProvider(), this.lastUpdateTime) == -1;
			}
		}

		protected DateTime OldestDataTime
		{
			get
			{
				int oldestBucketIndex = this.OldestBucketIndex;
				int num = oldestBucketIndex;
				while (!this.filled[num])
				{
					num = (num + 1) % this.ValueBuckets.Length;
					if (num == oldestBucketIndex)
					{
						return DateTime.MaxValue;
					}
				}
				return this.GetTimeForBucket(num);
			}
		}

		protected int OldestBucketIndex
		{
			get
			{
				this.ExpireBucketsIfNecessary();
				int num = 1;
				if (this.totalBucketsExpired < (long)this.NumberOfBuckets)
				{
					num = this.NumberOfBuckets - (int)this.totalBucketsExpired;
				}
				return (this.currentBucket + num) % this.ValueBuckets.Length;
			}
		}

		protected abstract Array ValueBuckets { get; }

		internal static void ValidateSlidingWindowAndBucketLength(TimeSpan slidingWindowLength, TimeSpan bucketLength)
		{
			ExAssert.RetailAssert(bucketLength >= SlidingWindow.MinBucketLength, "bucketLength: [{0}] must be at least [{1}].", new object[]
			{
				bucketLength,
				SlidingWindow.MinBucketLength
			});
			ExAssert.RetailAssert(slidingWindowLength > TimeSpan.Zero, "slidingWindowLength: [{0}] must be greater than zero.", new object[]
			{
				slidingWindowLength
			});
			ExAssert.RetailAssert(bucketLength < slidingWindowLength, "bucketLength: [{0}] must be less than slidingWindowLength: [{1}]", new object[]
			{
				bucketLength,
				slidingWindowLength
			});
			ExAssert.RetailAssert(slidingWindowLength <= SlidingWindow.MaxSlidingWindowLength, "slidingWindowLength: [{0}] must be less than or equal to slidingWindowLength: [{1}]", new object[]
			{
				slidingWindowLength,
				SlidingWindow.MaxSlidingWindowLength
			});
			ExAssert.RetailAssert(slidingWindowLength.Ticks % bucketLength.Ticks == 0L, "slidingWindowLength: [{0}] must be a multiple of bucketLength: [{1}]", new object[]
			{
				slidingWindowLength,
				bucketLength
			});
		}

		protected abstract void ExpireBucket(int bucket);

		protected void ExpireBucketsIfNecessary()
		{
			ExAssert.RetailAssert(this.ValueBuckets != null, "Child class did not create a ValueBuckets array");
			if (this.ValueBuckets.Length != this.NumberOfBuckets)
			{
				ExAssert.RetailAssert(false, "The child class did not define the appropriate number of elements in the ValueBuckets array. Expected: {0}, Actual: {1}", new object[]
				{
					this.NumberOfBuckets,
					this.ValueBuckets.Length
				});
			}
			DateTime d = this.currentTimeProvider();
			long num = (d - this.creationTime).Ticks / this.BucketLength.Ticks;
			int num2 = 0;
			while (num2 < this.ValueBuckets.Length && this.totalBucketsExpired < num)
			{
				this.currentBucket = (this.currentBucket + 1) % this.ValueBuckets.Length;
				this.ExpireBucket(this.currentBucket);
				this.filled[this.currentBucket] = false;
				this.totalBucketsExpired += 1L;
				num2++;
			}
			this.totalBucketsExpired = num;
		}

		protected void SetLastUpdateTime()
		{
			this.lastUpdateTime = this.currentTimeProvider();
			this.filled[this.currentBucket] = true;
		}

		protected DateTime RoundToBucketLength(DateTime dateTime)
		{
			return new DateTime(dateTime.Ticks / this.BucketLength.Ticks * this.BucketLength.Ticks);
		}

		protected int GetTargetBucket(DateTime now, DateTime timeStampUtc)
		{
			long num = (now - timeStampUtc).Ticks / this.BucketLength.Ticks;
			if (num < (long)this.NumberOfBuckets)
			{
				long num2 = (long)this.currentBucket - num;
				if (num2 < 0L)
				{
					num2 = (long)this.NumberOfBuckets + num2;
				}
				return (int)num2;
			}
			return -1;
		}

		private DateTime GetTimeForBucket(int index)
		{
			if (index < 0 || index >= this.NumberOfBuckets)
			{
				throw new InvalidOperationException(string.Format("index {0} is out of range", index));
			}
			long num = 0L;
			if (index < this.currentBucket)
			{
				num = (long)(this.currentBucket - index);
			}
			else if (index > this.currentBucket)
			{
				num = (long)(this.currentBucket + this.NumberOfBuckets - index);
			}
			return new DateTime(this.creationTime.Ticks + (this.totalBucketsExpired - num) * this.BucketLength.Ticks);
		}

		internal static readonly TimeSpan MaxSlidingWindowLength = TimeSpan.FromHours(12.0);

		internal static readonly TimeSpan MinBucketLength = TimeSpan.FromSeconds(1.0);

		protected readonly TimeSpan BucketLength;

		protected readonly int NumberOfBuckets;

		protected Func<DateTime> currentTimeProvider;

		protected int currentBucket;

		private readonly DateTime creationTime;

		private long totalBucketsExpired;

		private DateTime lastUpdateTime;

		private bool[] filled;
	}
}
