using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class Trendline : ITrendline
	{
		public Trendline(TimeSpan historyInterval, TimeSpan idleCleanupInterval, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyInterval", historyInterval, TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleCleanupInterval", idleCleanupInterval, TimeSpan.Zero, TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.historyInterval = historyInterval;
			this.idleCleanupInterval = idleCleanupInterval;
			this.timeProvider = timeProvider;
		}

		public Trendline(IEnumerable<long> values, TimeSpan bucketLength, TimeSpan idleCleanupInterval, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfNull("values", values);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("bucketLength", bucketLength, TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleCleanupInterval", idleCleanupInterval, TimeSpan.Zero, TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.idleCleanupInterval = idleCleanupInterval;
			this.timeProvider = timeProvider;
			int num = values.Count<long>();
			this.historyInterval = new TimeSpan((long)num * bucketLength.Ticks);
			DateTime currentTime = new DateTime(this.timeProvider().Ticks - (long)num * bucketLength.Ticks);
			foreach (long value in values)
			{
				this.AddDataPoint(value, currentTime);
				currentTime = currentTime.AddTicks(bucketLength.Ticks);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.counter == null || this.CheckIfCounterExpired(this.timeProvider());
			}
		}

		public DateTime OldestPointTime
		{
			get
			{
				if (this.missingFirstPoint)
				{
					return DateTime.MaxValue;
				}
				if (this.HasAtLeastTwoPoints && !this.counter.IsEmpty)
				{
					lock (this.syncObject)
					{
						bool flag;
						if (flag)
						{
							return this.counter.OldestDataTime;
						}
					}
					return DateTime.MaxValue;
				}
				if (this.timeProvider() - this.updateTime < this.historyInterval)
				{
					return this.updateTime;
				}
				return DateTime.MaxValue;
			}
		}

		private bool HasAtLeastTwoPoints
		{
			get
			{
				return this.isIncreasing != null;
			}
		}

		public bool WasAbove(long high)
		{
			if (this.counter == null)
			{
				return false;
			}
			bool result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints())
					{
						bool flag2;
						if (this.currentPoint <= high)
						{
							if (this.HasAtLeastTwoPoints)
							{
								flag2 = this.counter.Any((long i) => i > high);
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = true;
						}
						result = flag2;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool WasBelow(long low)
		{
			if (this.counter == null)
			{
				return false;
			}
			bool result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints())
					{
						bool flag2;
						if (this.currentPoint >= low)
						{
							if (this.HasAtLeastTwoPoints)
							{
								flag2 = this.counter.Any((long i) => i < low);
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = true;
						}
						result = flag2;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool HasCrossedBelowAfterLastCrossingAbove(long high, long low)
		{
			return this.EnumeratePointsCompareHighLow(high, low, this.currentPoint < low, (bool b) => Trendline.DelegateResult.SetStateTrue, delegate(bool b)
			{
				if (!b)
				{
					return Trendline.DelegateResult.ReturnFalse;
				}
				return Trendline.DelegateResult.ReturnTrue;
			});
		}

		public bool HasCrossedAboveAfterLastCrossingBelow(long low, long high)
		{
			return this.EnumeratePointsCompareHighLow(high, low, this.currentPoint > high, delegate(bool b)
			{
				if (!b)
				{
					return Trendline.DelegateResult.ReturnFalse;
				}
				return Trendline.DelegateResult.ReturnTrue;
			}, (bool b) => Trendline.DelegateResult.SetStateTrue);
		}

		public bool StillAboveLowAfterCrossingHigh(long high, long low)
		{
			if (low >= high)
			{
				return false;
			}
			if (!this.missingFirstPoint && this.currentPoint < low)
			{
				return false;
			}
			if (!this.missingFirstPoint && this.currentPoint > high)
			{
				return true;
			}
			return this.EnumeratePointsCompareHighLow(high, low, false, (bool b) => Trendline.DelegateResult.ReturnFalse, (bool b) => Trendline.DelegateResult.ReturnTrue);
		}

		public bool StillBelowHighAfterCrossingLow(long low, long high)
		{
			if (low >= high)
			{
				return false;
			}
			if (!this.missingFirstPoint && this.currentPoint > high)
			{
				return false;
			}
			if (!this.missingFirstPoint && this.currentPoint < low)
			{
				return true;
			}
			return this.EnumeratePointsCompareHighLow(high, low, false, (bool b) => Trendline.DelegateResult.ReturnTrue, (bool b) => Trendline.DelegateResult.ReturnFalse);
		}

		public long GetMax()
		{
			if (this.counter == null)
			{
				return 0L;
			}
			long result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints())
					{
						if (this.HasAtLeastTwoPoints)
						{
							long max = this.counter.GetMax();
							result = Math.Max(max, this.currentPoint);
						}
						else
						{
							result = this.currentPoint;
						}
					}
					else
					{
						result = 0L;
					}
				}
				else
				{
					result = 0L;
				}
			}
			return result;
		}

		public long GetMin()
		{
			if (this.counter == null)
			{
				return 0L;
			}
			long result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints())
					{
						if (this.HasAtLeastTwoPoints)
						{
							long min = this.counter.GetMin();
							result = Math.Min(min, this.currentPoint);
						}
						else
						{
							result = this.currentPoint;
						}
					}
					else
					{
						result = 0L;
					}
				}
				else
				{
					result = 0L;
				}
			}
			return result;
		}

		public long GetAverage()
		{
			if (this.counter == null)
			{
				return 0L;
			}
			long result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints())
					{
						if (this.HasAtLeastTwoPoints)
						{
							result = Convert.ToInt64(this.counter.Concat(new List<long>
							{
								this.currentPoint
							}).Average());
						}
						else
						{
							result = this.currentPoint;
						}
					}
					else
					{
						result = 0L;
					}
				}
				else
				{
					result = 0L;
				}
			}
			return result;
		}

		public void AddDataPoint(long value)
		{
			this.AddDataPoint(value, this.timeProvider());
		}

		private void AddDataPoint(long value, DateTime currentTime)
		{
			if (this.missingFirstPoint)
			{
				this.RecordFirstPoint(value, currentTime);
				return;
			}
			if (value == this.currentPoint)
			{
				this.updateTime = currentTime;
				return;
			}
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.isIncreasing == null)
					{
						this.counter.AddValue(this.currentPoint, this.updateTime);
						this.previousPoint = this.currentPoint;
						this.isIncreasing = new bool?(value > this.currentPoint);
						this.UpdateCurrentPoint(value, currentTime);
					}
					else if ((this.currentPoint < value && this.isIncreasing.Value) || (this.currentPoint > value && !this.isIncreasing.Value))
					{
						this.UpdateCurrentPoint(value, currentTime);
					}
					else if ((this.currentPoint > value && this.isIncreasing.Value) || (this.currentPoint < value && !this.isIncreasing.Value))
					{
						this.RecordPoint(value, currentTime);
					}
				}
			}
		}

		private void UpdateCurrentPoint(long value, DateTime currentTime)
		{
			this.currentPoint = value;
			this.updateTime = currentTime;
		}

		private void RecordPoint(long value, DateTime currentTime)
		{
			if (this.ExpireOldPoints())
			{
				this.previousPoint = this.currentPoint;
				this.UpdateCurrentPoint(value, currentTime);
				this.isIncreasing = !this.isIncreasing;
				this.counter.AddValue(this.previousPoint, this.updateTime);
				return;
			}
			this.RecordFirstPoint(value, currentTime);
		}

		private void RecordFirstPoint(long value, DateTime currentTime)
		{
			if (this.counter == null)
			{
				lock (this.syncObject)
				{
					if (this.counter == null)
					{
						this.counter = new SlidingSequence<long>(this.historyInterval, TimeSpan.FromMilliseconds(Math.Max(1000.0, this.historyInterval.TotalMilliseconds / 30.0)), this.timeProvider);
					}
				}
			}
			this.UpdateCurrentPoint(value, currentTime);
			this.missingFirstPoint = false;
		}

		private bool ExpireOldPoints()
		{
			DateTime dateTime = this.timeProvider();
			if (dateTime - this.updateTime >= this.historyInterval)
			{
				if (this.CheckIfCounterExpired(dateTime))
				{
					this.counter = null;
				}
				this.currentPoint = 0L;
				this.previousPoint = 0L;
				this.isIncreasing = null;
				this.missingFirstPoint = true;
				return false;
			}
			if (this.isIncreasing != null && this.counter.GetLast() != this.previousPoint)
			{
				this.counter.AddValue(this.previousPoint, this.updateTime.Subtract(TimeSpan.FromSeconds(1.0)));
			}
			return true;
		}

		private bool CheckIfCounterExpired(DateTime now)
		{
			return now - this.updateTime >= this.idleCleanupInterval + this.historyInterval;
		}

		private bool EnumeratePointsCompareHighLow(long high, long low, bool initState, Func<bool, Trendline.DelegateResult> actionOnLow, Func<bool, Trendline.DelegateResult> actionOnHigh)
		{
			if (low >= high)
			{
				return false;
			}
			if (this.counter == null)
			{
				return false;
			}
			bool result;
			lock (this.syncObject)
			{
				bool flag;
				if (flag)
				{
					if (this.ExpireOldPoints() && this.HasAtLeastTwoPoints)
					{
						foreach (object obj in this.counter)
						{
							long num = (long)obj;
							if (num > high)
							{
								Trendline.DelegateResult delegateResult = actionOnHigh(initState);
								if (delegateResult == Trendline.DelegateResult.ReturnFalse)
								{
									return false;
								}
								if (delegateResult == Trendline.DelegateResult.ReturnTrue)
								{
									return true;
								}
								if (delegateResult == Trendline.DelegateResult.SetStateFalse)
								{
									initState = false;
								}
								else if (delegateResult == Trendline.DelegateResult.SetStateTrue)
								{
									initState = true;
								}
							}
							if (num < low)
							{
								Trendline.DelegateResult delegateResult2 = actionOnLow(initState);
								if (delegateResult2 == Trendline.DelegateResult.ReturnFalse)
								{
									return false;
								}
								if (delegateResult2 == Trendline.DelegateResult.ReturnTrue)
								{
									return true;
								}
								if (delegateResult2 == Trendline.DelegateResult.SetStateFalse)
								{
									initState = false;
								}
								else if (delegateResult2 == Trendline.DelegateResult.SetStateTrue)
								{
									initState = true;
								}
							}
						}
					}
					result = false;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private readonly TimeSpan historyInterval;

		private readonly TimeSpan idleCleanupInterval;

		private readonly Func<DateTime> timeProvider;

		private readonly object syncObject = new object();

		private SlidingSequence<long> counter;

		private long currentPoint;

		private DateTime updateTime;

		private long previousPoint;

		private bool? isIncreasing;

		private bool missingFirstPoint = true;

		private enum DelegateResult
		{
			Nothing,
			ReturnTrue,
			ReturnFalse,
			SetStateTrue,
			SetStateFalse
		}
	}
}
