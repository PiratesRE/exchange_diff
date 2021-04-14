using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SetCounterIf
	{
		public SetCounterIf(ICounterWrapper counter, CounterCompareType compareType, int interval)
		{
			this.Counter = counter;
			this.CompareType = compareType;
			this.Interval = interval;
		}

		internal void SetLastRefreshForTest(int lastRefresh)
		{
			this.lastUpdateTicks = lastRefresh;
		}

		internal long OldValue { get; private set; }

		internal CounterCompareType CompareType { get; private set; }

		internal ICounterWrapper Counter { get; private set; }

		internal int Interval { get; private set; }

		internal void Set(long newValue)
		{
			this.Set(newValue, Environment.TickCount);
		}

		internal void Set(long newValue, int nowTicks)
		{
			if (newValue == this.OldValue)
			{
				return;
			}
			if (this.Counter != null)
			{
				bool flag = false;
				if (this.Interval != 2147483647 && TickDiffer.Elapsed(this.lastUpdateTicks, nowTicks).TotalMilliseconds > (double)this.Interval)
				{
					flag = true;
				}
				else
				{
					switch (this.CompareType)
					{
					case CounterCompareType.Lower:
						flag = (newValue < this.OldValue);
						break;
					case CounterCompareType.Higher:
						flag = (newValue > this.OldValue);
						break;
					case CounterCompareType.Changed:
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.Counter.SetRawValue(newValue);
					this.OldValue = newValue;
					this.lastUpdateTicks = nowTicks;
				}
			}
		}

		internal void ForceSet(long newValue)
		{
			this.Counter.SetRawValue(newValue);
			this.OldValue = newValue;
			this.lastUpdateTicks = Environment.TickCount;
		}

		internal void SetOldValue(long oldValue)
		{
			this.OldValue = oldValue;
		}

		public const int NoIntervalCheck = 2147483647;

		private int lastUpdateTicks = Environment.TickCount;

		internal class CounterWrapper : ICounterWrapper
		{
			public CounterWrapper(ExPerformanceCounter counter)
			{
				if (counter == null)
				{
					throw new ArgumentNullException("counter");
				}
				this.counter = counter;
			}

			public void SetRawValue(long value)
			{
				this.counter.RawValue = value;
			}

			private ExPerformanceCounter counter;
		}
	}
}
