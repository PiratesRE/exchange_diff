using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class SegmentedSlidingCounter
	{
		public SegmentedSlidingCounter(TimeSpan[] segments, TimeSpan bucketLength) : this(segments, bucketLength, null)
		{
		}

		internal SegmentedSlidingCounter(TimeSpan[] segments, TimeSpan bucketLength, Func<DateTime> currentTimeProvider)
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments");
			}
			if (segments.Length == 0)
			{
				throw new ArgumentException("segments cannot be empty");
			}
			this.bucketLength = bucketLength;
			if (currentTimeProvider == null)
			{
				this.currentTimeProvider = new Func<DateTime>(this.GetCurrentTime);
				this.creationTime = this.RoundToBucketLength(DateTime.UtcNow);
				this.currentTime = this.creationTime;
			}
			else
			{
				this.currentTimeProvider = currentTimeProvider;
				this.creationTime = this.RoundToBucketLength(this.currentTimeProvider());
			}
			this.numSegments = (long)segments.Length;
			this.numBucketsPerSegment = new long[this.numSegments];
			this.segmentValues = new long[this.numSegments + 1L];
			this.numBuckets = 0L;
			for (long num = 0L; num < this.numSegments; num += 1L)
			{
				long num2;
				checked
				{
					TimeSpan slidingWindowLength = segments[(int)((IntPtr)num)];
					SlidingWindow.ValidateSlidingWindowAndBucketLength(slidingWindowLength, bucketLength);
					num2 = slidingWindowLength.Ticks / this.bucketLength.Ticks;
					this.numBucketsPerSegment[(int)((IntPtr)num)] = num2;
				}
				this.numBuckets += num2;
			}
			this.bucketValues = new long[this.numBuckets];
		}

		public DateTime AddEventsAt(DateTime timeStampUtc, long eventCount)
		{
			DateTime result;
			lock (this)
			{
				result = this.AddOrRemoveEventsAt(timeStampUtc, eventCount, false);
			}
			return result;
		}

		public void RemoveEventsAt(DateTime timeStampUtc, long eventCount)
		{
			lock (this)
			{
				this.AddOrRemoveEventsAt(timeStampUtc, eventCount, true);
			}
		}

		public long TimedUpdate()
		{
			return this.TimedUpdate(this.segmentValues);
		}

		public long TimedUpdate(long[] segmentValues)
		{
			long result;
			lock (this)
			{
				this.ExpireBucketsIfNecessary(this.RoundToBucketLength(this.currentTimeProvider()));
				result = this.GetSegmentValues(segmentValues);
			}
			return result;
		}

		internal long GetSegmentValues(long[] segmentValues)
		{
			if (segmentValues == null)
			{
				throw new ArgumentNullException("segmentValues");
			}
			if ((long)segmentValues.Length < this.numSegments + 1L)
			{
				throw new ArgumentException("segmentValues");
			}
			long num = 0L;
			long num2 = this.currentBucketIndex;
			for (long num3 = 0L; num3 < this.numSegments; num3 += 1L)
			{
				segmentValues[(int)(checked((IntPtr)num3))] = 0L;
				for (long num4 = 0L; num4 < this.numBucketsPerSegment[(int)(checked((IntPtr)num3))]; num4 += 1L)
				{
					segmentValues[(int)(checked((IntPtr)num3))] += this.bucketValues[(int)(checked((IntPtr)num2))];
					num2 = (num2 + 1L) % this.numBuckets;
				}
				num += segmentValues[(int)(checked((IntPtr)num3))];
			}
			segmentValues[(int)(checked((IntPtr)this.numSegments))] = this.excessBucket;
			return num;
		}

		private DateTime AddOrRemoveEventsAt(DateTime timeStampUtc, long eventCount, bool isRemove)
		{
			if (eventCount <= 0L)
			{
				throw new ArgumentOutOfRangeException("eventCount: " + eventCount);
			}
			DateTime dateTime = this.currentTimeProvider();
			if (timeStampUtc > dateTime)
			{
				if (isRemove)
				{
					throw new ArgumentOutOfRangeException(string.Format("event timestamp [{0}] cannot be later than the current time [{1}] on Remove()", timeStampUtc, dateTime));
				}
				timeStampUtc = dateTime;
			}
			DateTime now = this.RoundToBucketLength(dateTime);
			this.ExpireBucketsIfNecessary(now);
			long num = isRemove ? (-1L * eventCount) : eventCount;
			long targetBucket = this.GetTargetBucket(now, this.RoundToBucketLength(timeStampUtc));
			long num2;
			if (targetBucket == -1L)
			{
				this.excessBucket += num;
				num2 = this.excessBucket;
			}
			else
			{
				this.bucketValues[(int)(checked((IntPtr)targetBucket))] += num;
				num2 = this.bucketValues[(int)(checked((IntPtr)targetBucket))];
			}
			if (num2 < 0L)
			{
				throw new InvalidOperationException("bucket value is negative: " + num2);
			}
			return timeStampUtc;
		}

		private void ExpireBucketsIfNecessary(DateTime now)
		{
			long num = (now - this.creationTime).Ticks / this.bucketLength.Ticks;
			if (this.totalBucketsExpired < num)
			{
				long num2 = 0L;
				while (num2 < this.numBuckets && this.totalBucketsExpired < num)
				{
					this.currentBucketIndex = ((this.currentBucketIndex > 0L) ? (this.currentBucketIndex - 1L) : (this.numBuckets - 1L));
					this.excessBucket += this.bucketValues[(int)(checked((IntPtr)this.currentBucketIndex))];
					this.bucketValues[(int)(checked((IntPtr)this.currentBucketIndex))] = 0L;
					this.totalBucketsExpired += 1L;
					num2 += 1L;
				}
				this.totalBucketsExpired = num;
			}
		}

		private long GetTargetBucket(DateTime now, DateTime timeStampUtc)
		{
			long num = (now - timeStampUtc).Ticks / this.bucketLength.Ticks;
			if (num >= this.numBuckets)
			{
				return -1L;
			}
			return (this.currentBucketIndex + num) % this.numBuckets;
		}

		private DateTime GetCurrentTime()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > this.currentTime)
			{
				this.currentTime = utcNow;
			}
			return this.currentTime;
		}

		private DateTime RoundToBucketLength(DateTime dateTime)
		{
			return new DateTime(dateTime.Ticks / this.bucketLength.Ticks * this.bucketLength.Ticks);
		}

		private readonly Func<DateTime> currentTimeProvider;

		private readonly DateTime creationTime;

		private readonly TimeSpan bucketLength;

		private readonly long numBuckets;

		private readonly long numSegments;

		private readonly long[] segmentValues;

		private DateTime currentTime;

		private long totalBucketsExpired;

		private long currentBucketIndex;

		private long excessBucket;

		private long[] bucketValues;

		private long[] numBucketsPerSegment;
	}
}
