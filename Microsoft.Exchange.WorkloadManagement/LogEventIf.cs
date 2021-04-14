using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal abstract class LogEventIf
	{
		public LogEventIf(TimeSpan windowBucketLength, ushort numberOfBuckets, ushort minimumBucketCountToEvent)
		{
			if (windowBucketLength <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("windowBucketLength", windowBucketLength, "WindowBucketLength must be greater than zero.");
			}
			if (numberOfBuckets == 0)
			{
				throw new ArgumentOutOfRangeException("numberOfBuckets", numberOfBuckets, "NumberOfBuckets must be greater than zero.");
			}
			if (minimumBucketCountToEvent > numberOfBuckets || minimumBucketCountToEvent == 0)
			{
				throw new ArgumentOutOfRangeException("minimumBucketCountToEvent", minimumBucketCountToEvent, "MinimumBucketCountToEvent must be > 0 and <= numberOfBuckets.");
			}
			this.waitingBuckets = new Queue<LogEventIf.WindowBucket>((int)numberOfBuckets);
			this.windowBucketLength = windowBucketLength;
			this.lastUpdate = TimeProvider.UtcNow;
			this.MinimumBucketCountToEvent = (int)minimumBucketCountToEvent;
			this.totalWindowLength = TimeSpan.FromSeconds(windowBucketLength.TotalSeconds * (double)numberOfBuckets);
		}

		public Action<bool, DateTime> OnBucketExpire { get; set; }

		public Func<LogEventIf, bool> OnLogEvent { get; set; }

		public int MinimumBucketCountToEvent { get; private set; }

		public int Sum { get; private set; }

		public int BucketCount
		{
			get
			{
				this.Update();
				return this.BucketCountNonUpdating;
			}
		}

		private int BucketCountNonUpdating
		{
			get
			{
				return this.waitingBuckets.Count + ((this.currentBucket == null) ? 0 : 1);
			}
		}

		private bool IsEmpty
		{
			get
			{
				return this.BucketCount == 0;
			}
		}

		public void Clear()
		{
			lock (this.instanceLock)
			{
				this.waitingBuckets.Clear();
				this.currentBucket = null;
				this.Sum = 0;
				this.lastUpdate = TimeProvider.UtcNow;
			}
		}

		public void Set(bool value)
		{
			lock (this.instanceLock)
			{
				DateTime utcNow = TimeProvider.UtcNow;
				this.VerifyTime(ref utcNow);
				this.InternalUpdate(utcNow);
				bool flag2;
				LogEventIf.WindowBucket orCreateCurrentBucket = this.GetOrCreateCurrentBucket(utcNow, out flag2);
				if (flag2 || (value && !orCreateCurrentBucket.Value))
				{
					orCreateCurrentBucket.Value = value;
				}
			}
		}

		public void Update()
		{
			lock (this.instanceLock)
			{
				DateTime utcNow = TimeProvider.UtcNow;
				this.VerifyTime(ref utcNow);
				this.InternalUpdate(utcNow);
			}
		}

		protected abstract void InternalLogEvent();

		private void InternalUpdate(DateTime utcNow)
		{
			if (this.currentBucket != null && utcNow >= this.currentBucket.CreationTimeUtc + this.windowBucketLength)
			{
				int count = this.waitingBuckets.Count;
				this.waitingBuckets.Enqueue(this.currentBucket);
				this.Sum += (this.currentBucket.Value ? 1 : 0);
				this.currentBucket = null;
			}
			while (this.waitingBuckets.Count > 0)
			{
				LogEventIf.WindowBucket windowBucket = this.waitingBuckets.Peek();
				if (!(utcNow >= windowBucket.ExpireTimeUtc))
				{
					break;
				}
				if (TimeProvider.UtcNow - this.lastEventTime >= this.totalWindowLength && this.BucketCountNonUpdating >= this.MinimumBucketCountToEvent && this.Sum == 0)
				{
					this.LogEvent();
				}
				int count2 = this.waitingBuckets.Count;
				this.waitingBuckets.Dequeue();
				int num = count2 - 1;
				if (num > 0)
				{
					this.Sum -= (windowBucket.Value ? 1 : 0);
				}
				else
				{
					this.Sum = 0;
				}
				if (this.OnBucketExpire != null)
				{
					this.OnBucketExpire(windowBucket.Value, windowBucket.ExpireTimeUtc);
				}
			}
		}

		private void LogEvent()
		{
			bool flag = true;
			if (this.OnLogEvent != null)
			{
				flag = this.OnLogEvent(this);
			}
			this.lastEventTime = TimeProvider.UtcNow;
			if (flag)
			{
				this.InternalLogEvent();
			}
		}

		private LogEventIf.WindowBucket GetOrCreateCurrentBucket(DateTime currentUtc, out bool isNew)
		{
			LogEventIf.WindowBucket result;
			lock (this.instanceLock)
			{
				this.VerifyTime(ref currentUtc);
				isNew = (this.currentBucket == null);
				if (isNew)
				{
					this.currentBucket = new LogEventIf.WindowBucket(currentUtc, currentUtc.Add(this.totalWindowLength));
				}
				result = this.currentBucket;
			}
			return result;
		}

		private bool VerifyTime(ref DateTime currentUtc)
		{
			if (currentUtc < this.lastUpdate)
			{
				currentUtc = this.lastUpdate;
				return false;
			}
			this.lastUpdate = currentUtc;
			return true;
		}

		private readonly TimeSpan windowBucketLength;

		private readonly TimeSpan totalWindowLength;

		private Queue<LogEventIf.WindowBucket> waitingBuckets;

		private LogEventIf.WindowBucket currentBucket;

		private DateTime lastUpdate;

		private object instanceLock = new object();

		private DateTime lastEventTime = DateTime.MinValue;

		private class WindowBucket
		{
			public WindowBucket(DateTime currentTime, DateTime expireTime)
			{
				this.CreationTimeUtc = currentTime;
				this.ExpireTimeUtc = expireTime;
			}

			internal DateTime ExpireTimeUtc { get; private set; }

			internal DateTime CreationTimeUtc { get; private set; }

			internal bool Value { get; set; }
		}
	}
}
