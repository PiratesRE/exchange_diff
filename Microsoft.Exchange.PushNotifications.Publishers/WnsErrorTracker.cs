using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsErrorTracker
	{
		public WnsErrorTracker(int maxFailedAuthRequests, int baseDelayInMilliseconds, int backOffTimeInSeconds)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("maxFailedAuthRequests", maxFailedAuthRequests);
			ArgumentValidator.ThrowIfZeroOrNegative("baseDelayInMilliseconds", baseDelayInMilliseconds);
			ArgumentValidator.ThrowIfZeroOrNegative("backOffTimeInSeconds", backOffTimeInSeconds);
			this.MaxFailedAuthRequests = maxFailedAuthRequests;
			this.BaseDelay = baseDelayInMilliseconds;
			this.BackOffTime = backOffTimeInSeconds;
			this.ResetEndTimes();
		}

		public ExDateTime DelayEndTime { get; private set; }

		public ExDateTime BackOffEndTime { get; private set; }

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

		public int FailedAuthRequests { get; private set; }

		public int NotificationErrorWeight { get; private set; }

		private int MaxFailedAuthRequests { get; set; }

		private int BaseDelay { get; set; }

		private int BackOffTime { get; set; }

		public virtual void ReportAuthenticationSuccess()
		{
			this.FailedAuthRequests = 0;
			this.ResetEndTimes();
		}

		public virtual void ReportAuthenticationFailure()
		{
			if (this.FailedAuthRequests < this.MaxFailedAuthRequests)
			{
				this.FailedAuthRequests++;
			}
			this.SetDelayEndTime();
			if (this.FailedAuthRequests == this.MaxFailedAuthRequests)
			{
				this.SetBackOffEndTime();
			}
		}

		public virtual void ReportWnsRequestSuccess()
		{
			this.NotificationErrorWeight = 0;
			this.ResetEndTimes();
		}

		public virtual void ReportWnsRequestFailure(WnsResultErrorType errorType)
		{
			if (this.NotificationErrorWeight < 100)
			{
				switch (errorType)
				{
				case WnsResultErrorType.Unknown:
					this.NotificationErrorWeight += 10;
					break;
				case WnsResultErrorType.Timeout:
					this.NotificationErrorWeight += 40;
					break;
				case WnsResultErrorType.Throttle:
					this.NotificationErrorWeight += 100;
					break;
				case WnsResultErrorType.AuthTokenExpired:
					this.NotificationErrorWeight += 40;
					break;
				case WnsResultErrorType.ServerUnavailable:
					this.NotificationErrorWeight += 2;
					break;
				}
			}
			if (this.NotificationErrorWeight >= 100)
			{
				this.SetBackOffEndTime();
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
			this.FailedAuthRequests = 0;
			this.NotificationErrorWeight = 0;
			this.ResetEndTimes();
		}

		private void ResetEndTimes()
		{
			this.DelayEndTime = ExDateTime.MinValue;
			this.BackOffEndTime = ExDateTime.MinValue;
		}

		private void SetDelayEndTime()
		{
			this.DelayEndTime = ExDateTime.UtcNow.AddMilliseconds((double)(this.FailedAuthRequests * this.BaseDelay));
		}

		private void SetBackOffEndTime()
		{
			this.BackOffEndTime = ExDateTime.UtcNow.AddSeconds((double)this.BackOffTime);
		}

		private const int UnknownErrorWeight = 10;

		private const int TimeoutErrorWeight = 40;

		private const int ThrottlingErrorWeight = 100;

		private const int AuthTokenExpiredWeight = 40;

		private const int ServerUnavailableWeight = 2;

		private const int MaxNotificationErrorWeight = 100;
	}
}
