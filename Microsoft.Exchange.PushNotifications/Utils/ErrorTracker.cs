using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal class ErrorTracker<T> where T : struct, IConvertible
	{
		public ErrorTracker(Dictionary<T, int> errorWeightTable, int maxErrorWeight, int backOffTimeInSeconds = 0, int baseDelayInMilliseconds = 0)
		{
			ArgumentValidator.ThrowIfNull("errorWeightTable", errorWeightTable);
			ArgumentValidator.ThrowIfZeroOrNegative("maxErrorWeight", maxErrorWeight);
			ArgumentValidator.ThrowIfNegative("backOffTimeInSeconds", backOffTimeInSeconds);
			ArgumentValidator.ThrowIfNegative("baseDelayInMilliseconds", baseDelayInMilliseconds);
			ArgumentValidator.ThrowIfZeroOrNegative("errorWeightTable.Count", errorWeightTable.Count);
			this.BackOffTime = backOffTimeInSeconds;
			this.MaxErrorWeight = maxErrorWeight;
			this.ErrorWeightTable = errorWeightTable;
			this.BaseDelay = baseDelayInMilliseconds;
			this.CurrentErrorWeight = 0;
			this.ResetEndTimes();
		}

		public virtual ExDateTime DelayEndTime
		{
			get
			{
				return this.delayEndTime;
			}
		}

		public virtual ExDateTime BackOffEndTime
		{
			get
			{
				return this.backOffEndTime;
			}
		}

		public virtual bool ShouldDelay
		{
			get
			{
				return this.DelayEndTime > ExDateTime.UtcNow;
			}
		}

		public virtual bool ShouldBackOff
		{
			get
			{
				return this.BackOffEndTime > ExDateTime.UtcNow;
			}
		}

		public int CurrentErrorWeight { get; private set; }

		private Dictionary<T, int> ErrorWeightTable { get; set; }

		private int MaxErrorWeight { get; set; }

		private int BaseDelay { get; set; }

		private int BackOffTime { get; set; }

		public virtual void ReportSuccess()
		{
			this.Reset();
		}

		public virtual void ReportError(T failureType)
		{
			int num;
			if (!this.ErrorWeightTable.TryGetValue(failureType, out num))
			{
				throw new ArgumentException(string.Format("Element {0} is not being tracked by error tracker", failureType), "failureType");
			}
			this.CurrentErrorWeight += num;
			if (this.BackOffTime > 0 && this.CurrentErrorWeight >= this.MaxErrorWeight)
			{
				this.SetBackOffEndTime(this.BackOffTime);
				return;
			}
			if (this.BaseDelay > 0)
			{
				this.SetDelayEndTime(this.CurrentErrorWeight * this.BaseDelay);
			}
		}

		public virtual void ConsumeDelay(int amountInMilliseconds)
		{
			ArgumentValidator.ThrowIfNegative("amountInMilliseconds", amountInMilliseconds);
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (utcNow >= this.DelayEndTime)
			{
				return;
			}
			int val = (int)this.DelayEndTime.Subtract(utcNow).TotalMilliseconds;
			int num = Math.Min(val, amountInMilliseconds);
			if (num > 0)
			{
				Thread.Sleep(num);
			}
		}

		public virtual void Reset()
		{
			this.CurrentErrorWeight = 0;
			this.ResetEndTimes();
		}

		private void SetDelayEndTime(int delayTimeInMilliseconds)
		{
			this.delayEndTime = ExDateTime.UtcNow.AddMilliseconds((double)delayTimeInMilliseconds);
		}

		private void SetBackOffEndTime(int backOffTimeInSeconds)
		{
			this.backOffEndTime = ExDateTime.UtcNow.AddSeconds((double)backOffTimeInSeconds);
		}

		private void ResetEndTimes()
		{
			this.delayEndTime = ExDateTime.MinValue;
			this.backOffEndTime = ExDateTime.MinValue;
		}

		private ExDateTime delayEndTime;

		private ExDateTime backOffEndTime;
	}
}
