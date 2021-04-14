using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class SlidingPercentageCounter : SlidingWindow
	{
		public SlidingPercentageCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength, bool isFailureReporting, Func<DateTime> currentTimeProvider) : base(slidingWindowLength, bucketLength, currentTimeProvider)
		{
			this.isFailureReporting = isFailureReporting;
			this.valuesExpires = true;
			this.numeratorBuckets = new long[this.NumberOfBuckets];
			this.denominatorBuckets = new long[this.NumberOfBuckets];
		}

		public SlidingPercentageCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength, bool isFailureReporting) : this(slidingWindowLength, bucketLength, isFailureReporting, () => DateTime.UtcNow)
		{
		}

		public SlidingPercentageCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength) : this(slidingWindowLength, bucketLength, false)
		{
		}

		public SlidingPercentageCounter() : base(SlidingWindow.MaxSlidingWindowLength, SlidingWindow.MinBucketLength)
		{
			this.valuesExpires = false;
		}

		public long Numerator
		{
			get
			{
				return this.numeratorTotal;
			}
		}

		public long Denominator
		{
			get
			{
				return this.denominatorTotal;
			}
		}

		public new bool IsEmpty
		{
			get
			{
				return this.valuesExpires && base.IsEmpty;
			}
		}

		protected override Array ValueBuckets
		{
			get
			{
				return this.numeratorBuckets;
			}
		}

		public double AddNumerator(long value)
		{
			return this.Add(value, 0L);
		}

		public double AddDenominator(long value)
		{
			return this.Add(0L, value);
		}

		public double Add(long numerator, long denominator)
		{
			base.SetLastUpdateTime();
			return this.ExpireAndUpdate(numerator, denominator);
		}

		public double GetSlidingPercentage()
		{
			return this.ExpireAndUpdate(0L, 0L);
		}

		private double ExpireAndUpdate(long numerator, long denominator)
		{
			double result;
			lock (this.syncObject)
			{
				if (this.valuesExpires)
				{
					base.ExpireBucketsIfNecessary();
				}
				this.numeratorTotal += numerator;
				this.denominatorTotal += denominator;
				if (this.valuesExpires)
				{
					this.denominatorBuckets[this.currentBucket] += denominator;
					this.numeratorBuckets[this.currentBucket] += numerator;
				}
				if (this.denominatorTotal == 0L)
				{
					if (this.numeratorTotal == 0L)
					{
						if (this.isFailureReporting)
						{
							result = 0.0;
						}
						else
						{
							result = 100.0;
						}
					}
					else if (this.numeratorTotal > 0L)
					{
						result = double.MaxValue;
					}
					else
					{
						result = double.MinValue;
					}
				}
				else
				{
					result = (double)this.numeratorTotal * 100.0 / (double)this.denominatorTotal;
				}
			}
			return result;
		}

		protected override void ExpireBucket(int bucket)
		{
			this.numeratorTotal -= this.numeratorBuckets[this.currentBucket];
			this.denominatorTotal -= this.denominatorBuckets[this.currentBucket];
			this.numeratorBuckets[this.currentBucket] = 0L;
			this.denominatorBuckets[this.currentBucket] = 0L;
		}

		private readonly bool valuesExpires;

		private readonly bool isFailureReporting;

		private long numeratorTotal;

		private long denominatorTotal;

		private long[] numeratorBuckets;

		private long[] denominatorBuckets;

		private object syncObject = new object();
	}
}
