using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	public class SlidingSequence<T> : SlidingWindow, IEnumerable<!0>, IEnumerable
	{
		public SlidingSequence(TimeSpan slidingWindowLength, TimeSpan bucketLength) : this(slidingWindowLength, bucketLength, () => DateTime.UtcNow)
		{
		}

		public SlidingSequence(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider) : base(slidingWindowLength, bucketLength, currentTimeProvider)
		{
			this.valueBuckets = new List<T>[this.NumberOfBuckets];
			this.isComparable = typeof(T).GetInterfaces().Any(delegate(Type x)
			{
				if (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IComparable<>))
				{
					return x.GetGenericArguments().All((Type t) => t == typeof(T));
				}
				return false;
			});
		}

		public new DateTime OldestDataTime
		{
			get
			{
				return base.OldestDataTime;
			}
		}

		protected override Array ValueBuckets
		{
			get
			{
				return this.valueBuckets;
			}
		}

		public void AddValue(T value)
		{
			base.ExpireBucketsIfNecessary();
			this.AddToBucket(this.currentBucket, value);
		}

		public void AddValue(T value, DateTime time)
		{
			DateTime dateTime = this.currentTimeProvider();
			if (time > dateTime)
			{
				throw new ArgumentOutOfRangeException("time", "Time must be in the past");
			}
			DateTime timeStampUtc = base.RoundToBucketLength(time);
			DateTime now = base.RoundToBucketLength(dateTime);
			base.ExpireBucketsIfNecessary();
			int targetBucket = base.GetTargetBucket(now, timeStampUtc);
			if (targetBucket == -1)
			{
				return;
			}
			this.AddToBucket(targetBucket, value);
		}

		public T GetLast()
		{
			base.ExpireBucketsIfNecessary();
			if (this.valueBuckets[this.currentBucket] != null)
			{
				return this.valueBuckets[this.currentBucket][this.valueBuckets[this.currentBucket].Count - 1];
			}
			for (int num = this.currentBucket - 1; num != this.currentBucket; num--)
			{
				if (num < 0)
				{
					num = this.valueBuckets.Length - 1;
				}
				if (this.valueBuckets[num] != null)
				{
					if (this.valueBuckets[num].Count != 0)
					{
						return this.valueBuckets[num][this.valueBuckets[num].Count - 1];
					}
					this.valueBuckets[num] = null;
				}
			}
			return default(T);
		}

		public T GetMax()
		{
			if (!this.isComparable)
			{
				return default(T);
			}
			base.ExpireBucketsIfNecessary();
			return this.RecalculateMax();
		}

		public T GetMin()
		{
			if (!this.isComparable)
			{
				return default(T);
			}
			base.ExpireBucketsIfNecessary();
			return this.RecalculateMin();
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			base.ExpireBucketsIfNecessary();
			for (int i = this.currentBucket; i >= 0; i--)
			{
				if (this.valueBuckets[i] != null)
				{
					for (int j = this.valueBuckets[i].Count - 1; j >= 0; j--)
					{
						yield return this.valueBuckets[i][j];
					}
				}
			}
			for (int k = this.valueBuckets.Length - 1; k > this.currentBucket; k--)
			{
				if (this.valueBuckets[k] != null)
				{
					for (int l = this.valueBuckets[k].Count - 1; l >= 0; l--)
					{
						yield return this.valueBuckets[k][l];
					}
				}
			}
			yield break;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (!0 ! in ((IEnumerable<!0>)this))
			{
				yield return !;
			}
			yield break;
		}

		protected override void ExpireBucket(int bucket)
		{
			if (this.isComparable)
			{
				if (this.maxSet && this.valueBuckets[bucket] != null && ((IComparable)((object)this.valueBuckets[bucket].Max<T>())).CompareTo(this.max) == 0)
				{
					this.max = default(T);
					this.maxSet = false;
				}
				if (this.minSet && this.valueBuckets[bucket] != null && ((IComparable)((object)this.valueBuckets[bucket].Min<T>())).CompareTo(this.min) == 0)
				{
					this.min = default(T);
					this.minSet = false;
				}
			}
			this.valueBuckets[bucket] = null;
		}

		private void AddToBucket(int bucket, T value)
		{
			if (this.valueBuckets[bucket] == null)
			{
				this.valueBuckets[bucket] = new List<T>();
			}
			this.valueBuckets[bucket].Add(value);
			base.SetLastUpdateTime();
			if (this.isComparable)
			{
				this.RecalculateMax();
				this.RecalculateMin();
				if (((IComparable<T>)((object)value)).CompareTo(this.max) > 0)
				{
					this.max = value;
				}
				if (((IComparable<T>)((object)value)).CompareTo(this.min) < 0)
				{
					this.min = value;
				}
			}
		}

		private T RecalculateMax()
		{
			if (!this.maxSet)
			{
				if (this.Any((T t) => true))
				{
					this.max = this.Max<T>();
					this.maxSet = true;
				}
			}
			return this.max;
		}

		private T RecalculateMin()
		{
			if (!this.minSet)
			{
				if (this.Any((T t) => true))
				{
					this.min = this.Min<T>();
					this.minSet = true;
				}
			}
			return this.min;
		}

		private readonly List<T>[] valueBuckets;

		private readonly bool isComparable;

		private T max;

		private bool maxSet;

		private T min;

		private bool minSet;
	}
}
