using System;
using System.Collections.Generic;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Collections
{
	internal class FixedTimeAverage
	{
		internal static Action OnCorrectiveActionForTest { get; set; }

		public FixedTimeAverage(ushort windowBucketLength, ushort numberOfBuckets, int currentTicks) : this(windowBucketLength, numberOfBuckets, currentTicks, FixedTimeAverage.DefaultCorrectiveInterval)
		{
		}

		public FixedTimeAverage(ushort windowBucketLength, ushort numberOfBuckets, int currentTicks, TimeSpan correctiveInterval)
		{
			if (windowBucketLength == 0)
			{
				throw new ArgumentOutOfRangeException("windowBucketLength", windowBucketLength, "WindowBucketLength must be greater than zero.");
			}
			if (numberOfBuckets == 0)
			{
				throw new ArgumentOutOfRangeException("numberOfBuckets", numberOfBuckets, "NumberOfBuckets must be greater than zero.");
			}
			if (correctiveInterval < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("correctiveInterval", correctiveInterval, "CorrectiveInterval must be positive.");
			}
			this.waitingBuckets = new Queue<FixedTimeAverage.WindowBucket>((int)numberOfBuckets);
			this.windowBucketLength = windowBucketLength;
			this.lastCall = currentTicks;
			this.lastCorrectiveFix = currentTicks;
			this.correctiveInterval = correctiveInterval;
			this.totalWindowLength = TimeSpan.FromMilliseconds((double)(windowBucketLength * numberOfBuckets));
		}

		public bool TryGetValue(out float value)
		{
			return this.TryGetValue(0, out value);
		}

		public bool TryGetValue(int minTrustedBucketCount, out float value)
		{
			bool result;
			lock (this.instanceLock)
			{
				value = this.GetValue();
				result = (!this.IsEmpty && this.BucketCount >= minTrustedBucketCount);
			}
			return result;
		}

		public float GetValue()
		{
			return this.GetValue(Environment.TickCount);
		}

		public void Add(uint value)
		{
			this.Add(Environment.TickCount, value);
		}

		public void Clear()
		{
			this.Clear(Environment.TickCount);
		}

		public void Clear(int currentTicks)
		{
			lock (this.instanceLock)
			{
				this.waitingBuckets.Clear();
				this.currentBucket = null;
				this.value = 0f;
				this.lastCall = currentTicks;
				this.lastCorrectiveFix = currentTicks;
			}
		}

		public void Add(int currentTicks, uint value)
		{
			lock (this.instanceLock)
			{
				this.VerifyTime(ref currentTicks);
				this.InternalUpdate(currentTicks);
				this.GetCurrentBucket(currentTicks).Add(currentTicks, value);
			}
		}

		public void Update(int currentTicks)
		{
			lock (this.instanceLock)
			{
				this.VerifyTime(ref currentTicks);
				this.InternalUpdate(currentTicks);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.BucketCount == 0 && this.currentBucket == null;
			}
		}

		private void InternalUpdate(int currentTicks)
		{
			if (this.currentBucket != null && TickDiffer.Elapsed(this.currentBucket.ExpireTicks, currentTicks) >= TimeSpan.Zero)
			{
				int count = this.waitingBuckets.Count;
				this.waitingBuckets.Enqueue(this.currentBucket);
				int num = count + 1;
				float num2 = 1f / (float)num;
				float num3 = 1f - num2;
				this.value = num2 * this.currentBucket.Average + num3 * this.value;
				this.currentBucket = null;
			}
			while (this.waitingBuckets.Count > 0)
			{
				FixedTimeAverage.WindowBucket windowBucket = this.waitingBuckets.Peek();
				if (!(TickDiffer.Elapsed(windowBucket.ExpireTicks, currentTicks) >= this.totalWindowLength))
				{
					break;
				}
				int count2 = this.waitingBuckets.Count;
				this.waitingBuckets.Dequeue();
				int num4 = count2 - 1;
				if (num4 > 0)
				{
					float num5 = 1f / (float)count2;
					float num6 = 1f - num5;
					this.value = (this.value - num5 * windowBucket.Average) / num6;
				}
				else
				{
					this.value = 0f;
				}
			}
			if (TickDiffer.Elapsed(this.lastCorrectiveFix, currentTicks) > this.correctiveInterval)
			{
				this.lastCorrectiveFix = currentTicks;
				float num7 = 0f;
				if (this.waitingBuckets.Count == 0)
				{
					this.value = 0f;
					return;
				}
				int count3 = this.waitingBuckets.Count;
				foreach (FixedTimeAverage.WindowBucket windowBucket2 in this.waitingBuckets)
				{
					if (windowBucket2 != null)
					{
						num7 += windowBucket2.Average / (float)count3;
					}
				}
				if (FixedTimeAverage.OnCorrectiveActionForTest != null)
				{
					FixedTimeAverage.OnCorrectiveActionForTest();
				}
				this.value = num7;
			}
		}

		internal float GetValue(int now)
		{
			this.Update(now);
			return this.value;
		}

		private int BucketCount
		{
			get
			{
				return this.waitingBuckets.Count;
			}
		}

		private FixedTimeAverage.WindowBucket GetCurrentBucket(int currentTicks)
		{
			FixedTimeAverage.WindowBucket result;
			lock (this.instanceLock)
			{
				this.VerifyTime(ref currentTicks);
				if (this.currentBucket == null)
				{
					this.currentBucket = new FixedTimeAverage.WindowBucket(currentTicks, this.windowBucketLength);
				}
				result = this.currentBucket;
			}
			return result;
		}

		private bool VerifyTime(ref int currentTicks)
		{
			if (TickDiffer.Elapsed(this.lastCall, currentTicks) < TimeSpan.Zero)
			{
				currentTicks = this.lastCall;
				return false;
			}
			this.lastCall = currentTicks;
			return true;
		}

		private Queue<FixedTimeAverage.WindowBucket> waitingBuckets;

		private FixedTimeAverage.WindowBucket currentBucket;

		private ushort windowBucketLength;

		private TimeSpan totalWindowLength;

		private volatile float value;

		private int lastCall;

		private TimeSpan correctiveInterval;

		private int lastCorrectiveFix;

		private object instanceLock = new object();

		private static readonly TimeSpan DefaultCorrectiveInterval = TimeSpan.FromMinutes(5.0);

		private class WindowBucket
		{
			public WindowBucket(int currentTicks, ushort windowSizeMsec)
			{
				this.ExpireTicks = TickDiffer.Add(currentTicks, (int)windowSizeMsec);
				this.WindowTimeSpan = TimeSpan.FromMilliseconds((double)windowSizeMsec);
			}

			internal int ExpireTicks { get; private set; }

			internal TimeSpan WindowTimeSpan { get; private set; }

			internal int Count
			{
				get
				{
					return this.count;
				}
			}

			internal bool Add(int currentTicks, uint value)
			{
				if (TickDiffer.Elapsed(this.ExpireTicks, currentTicks) < TimeSpan.Zero)
				{
					this.value += value;
					this.count++;
					return true;
				}
				return false;
			}

			internal float Average
			{
				get
				{
					if (this.count != 0)
					{
						return this.value / (float)this.count;
					}
					return 0f;
				}
			}

			private volatile float value;

			private volatile int count;
		}
	}
}
