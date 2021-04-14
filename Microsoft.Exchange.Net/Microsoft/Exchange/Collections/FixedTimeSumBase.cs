using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	internal class FixedTimeSumBase
	{
		protected FixedTimeSumBase(uint windowBucketLength, ushort numberOfBuckets, uint? limit)
		{
			ArgumentValidator.ThrowIfOutOfRange<uint>("windowBucketLength", windowBucketLength, 1U, uint.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<int>("numberOfBuckets", (int)numberOfBuckets, 1, 65535);
			this.waitingBuckets = new Queue<FixedTimeSumBase.WindowBucket>((int)numberOfBuckets);
			this.windowBucketLength = windowBucketLength;
			this.lastCall = TimeProvider.UtcNow;
			this.limit = limit;
			this.totalWindowLength = TimeSpan.FromMilliseconds(windowBucketLength * (uint)numberOfBuckets);
		}

		internal uint GetValue()
		{
			this.Update();
			return this.value;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.BucketCount == 0 && this.currentBucket == null;
			}
		}

		internal int BucketCount
		{
			get
			{
				return this.waitingBuckets.Count;
			}
		}

		internal void Clear()
		{
			lock (this.instanceLock)
			{
				this.waitingBuckets.Clear();
				this.currentBucket = null;
				this.value = 0U;
				this.lastCall = TimeProvider.UtcNow;
			}
		}

		internal void Update()
		{
			lock (this.instanceLock)
			{
				DateTime utcNow = this.SetLastCall(TimeProvider.UtcNow);
				this.InternalUpdate(utcNow);
			}
		}

		protected bool TryAdd(uint addend)
		{
			bool result = false;
			DateTime utcNow = this.SetLastCall(TimeProvider.UtcNow);
			lock (this.instanceLock)
			{
				this.InternalUpdate(utcNow);
				if (addend > 0U)
				{
					if (this.limit == null || this.IsUnderLimit(addend))
					{
						this.value += addend;
						result = this.GetCurrentBucket(utcNow).Add(utcNow, addend);
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void InternalUpdate(DateTime utcNow)
		{
			if (this.currentBucket != null && utcNow >= this.currentBucket.ExpireAt)
			{
				if (this.currentBucket.Value != 0U)
				{
					this.waitingBuckets.Enqueue(this.currentBucket);
				}
				this.currentBucket = null;
			}
			while (this.waitingBuckets.Count > 0)
			{
				FixedTimeSumBase.WindowBucket windowBucket = this.waitingBuckets.Peek();
				if (!(utcNow - windowBucket.ExpireAt >= this.totalWindowLength))
				{
					break;
				}
				this.waitingBuckets.Dequeue();
				this.value -= windowBucket.Value;
			}
			if (this.currentBucket == null)
			{
				int count = this.waitingBuckets.Count;
			}
		}

		private FixedTimeSumBase.WindowBucket GetCurrentBucket(DateTime utcNow)
		{
			FixedTimeSumBase.WindowBucket result;
			lock (this.instanceLock)
			{
				if (this.currentBucket == null)
				{
					this.currentBucket = new FixedTimeSumBase.WindowBucket(utcNow, this.windowBucketLength);
				}
				result = this.currentBucket;
			}
			return result;
		}

		private DateTime SetLastCall(DateTime utcNow)
		{
			if (this.lastCall < utcNow)
			{
				this.lastCall = utcNow;
			}
			return this.lastCall;
		}

		private bool IsUnderLimit(uint addend)
		{
			ulong num = (ulong)this.value + (ulong)addend;
			if (num <= (ulong)-1)
			{
				ulong num2 = num;
				uint? num3 = this.limit;
				return num2 <= (ulong)num3.GetValueOrDefault() && num3 != null;
			}
			return false;
		}

		protected readonly uint windowBucketLength;

		private readonly TimeSpan totalWindowLength;

		private readonly uint? limit;

		protected Queue<FixedTimeSumBase.WindowBucket> waitingBuckets;

		protected FixedTimeSumBase.WindowBucket currentBucket;

		protected volatile uint value;

		private DateTime lastCall;

		private object instanceLock = new object();

		protected class WindowBucket
		{
			public WindowBucket(DateTime utcNow, uint windowSizeMsec)
			{
				this.ExpireAt = utcNow.AddMilliseconds(windowSizeMsec);
				this.WindowTimeSpan = TimeSpan.FromMilliseconds(windowSizeMsec);
			}

			internal DateTime ExpireAt { get; private set; }

			internal TimeSpan WindowTimeSpan { get; private set; }

			internal bool Add(DateTime utcNow, uint value)
			{
				if (utcNow < this.ExpireAt)
				{
					this.value += value;
					return true;
				}
				return false;
			}

			public uint Value
			{
				get
				{
					return this.value;
				}
			}

			private volatile uint value;
		}
	}
}
