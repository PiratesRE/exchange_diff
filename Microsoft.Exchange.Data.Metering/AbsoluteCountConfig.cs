using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class AbsoluteCountConfig : CountConfig, IAbsoluteCountConfig, ICountedConfig, IEquatable<AbsoluteCountConfig>
	{
		private AbsoluteCountConfig(bool isPromotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool isRemovable, TimeSpan idleCleanupInterval, TimeSpan historyLookbackWindow, Func<DateTime> timeProvider) : base(isPromotable, minActivityThreshold, timeToLive, idleTimeToLive, isRemovable, idleCleanupInterval, timeProvider)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("historyLookbackWindow", historyLookbackWindow, TimeSpan.Zero, TimeSpan.MaxValue);
			this.historyLookbackWindow = historyLookbackWindow;
		}

		public TimeSpan HistoryLookbackWindow
		{
			get
			{
				base.UpdateAccessTime();
				return this.historyLookbackWindow;
			}
		}

		public static AbsoluteCountConfig Create(bool promotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool removable, TimeSpan idleCleanupInterval, TimeSpan historyLookbackWindow)
		{
			return AbsoluteCountConfig.Create(promotable, minActivityThreshold, timeToLive, idleTimeToLive, removable, idleCleanupInterval, historyLookbackWindow, () => DateTime.UtcNow);
		}

		public static AbsoluteCountConfig Create(bool promotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool removable, TimeSpan idleCleanupInterval, TimeSpan historyLookbackWindow, Func<DateTime> timeProvider)
		{
			AbsoluteCountConfig config = new AbsoluteCountConfig(promotable, minActivityThreshold, timeToLive, idleTimeToLive, removable, idleCleanupInterval, historyLookbackWindow, timeProvider);
			return (AbsoluteCountConfig)CountConfig.GetCachedObject(config);
		}

		public bool Equals(AbsoluteCountConfig config)
		{
			return !object.ReferenceEquals(null, config) && (object.ReferenceEquals(this, config) || (this.historyLookbackWindow.Equals(config.historyLookbackWindow) && base.Equals(config)));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is AbsoluteCountConfig && this.Equals(obj as AbsoluteCountConfig)));
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			return hashCode * 397 ^ this.historyLookbackWindow.GetHashCode();
		}

		private readonly TimeSpan historyLookbackWindow;
	}
}
