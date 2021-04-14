using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal abstract class RefreshableComponent
	{
		protected RefreshableComponent(TimeSpan updateInterval, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("updateInterval", updateInterval, TimeSpan.Zero.Add(TimeSpan.FromTicks(1L)), TimeSpan.MaxValue);
			this.UpdateInterval = updateInterval;
			this.timeProvider = timeProvider;
			this.LastUpdated = this.GetCurrentTime();
		}

		protected RefreshableComponent(TimeSpan updateInterval) : this(updateInterval, () => DateTime.UtcNow)
		{
		}

		public DateTime LastUpdated { get; private set; }

		protected Func<DateTime> TimeProvider
		{
			get
			{
				return this.timeProvider;
			}
		}

		private TimeSpan UpdateInterval { get; set; }

		public void RefreshIfNecessary()
		{
			DateTime currentTime = this.GetCurrentTime();
			if (this.LastUpdated.Add(this.UpdateInterval) < currentTime)
			{
				this.Refresh(currentTime);
				this.LastUpdated = currentTime;
			}
		}

		protected DateTime GetCurrentTime()
		{
			if (this.timeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.timeProvider();
		}

		protected abstract void Refresh(DateTime timestamp);

		private readonly Func<DateTime> timeProvider;
	}
}
