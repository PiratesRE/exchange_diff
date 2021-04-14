using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class RollingCountConfig : CountConfig, IRollingCountConfig, ICountedConfig, IEquatable<RollingCountConfig>
	{
		private RollingCountConfig(bool isPromotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool isRemovable, TimeSpan windowInterval, TimeSpan windowBucketSize, TimeSpan idleCleanupInterval, Func<DateTime> timeProvider) : base(isPromotable, minActivityThreshold, timeToLive, idleTimeToLive, isRemovable, idleCleanupInterval, timeProvider)
		{
			SlidingWindow.ValidateSlidingWindowAndBucketLength(windowInterval, windowBucketSize);
			this.windowInterval = windowInterval;
			this.windowBucketSize = windowBucketSize;
		}

		public TimeSpan WindowInterval
		{
			get
			{
				base.UpdateAccessTime();
				return this.windowInterval;
			}
		}

		public TimeSpan WindowBucketSize
		{
			get
			{
				base.UpdateAccessTime();
				return this.windowBucketSize;
			}
		}

		public static RollingCountConfig Create(bool promotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool removable, TimeSpan idleCleanupInterval, TimeSpan windowInterval, TimeSpan windowBucketSize)
		{
			return RollingCountConfig.Create(promotable, minActivityThreshold, timeToLive, idleTimeToLive, removable, idleCleanupInterval, windowInterval, windowBucketSize, () => DateTime.UtcNow);
		}

		public static RollingCountConfig Create(bool promotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool removable, TimeSpan idleCleanupInterval, TimeSpan windowInterval, TimeSpan windowBucketSize, Func<DateTime> timeProvider)
		{
			RollingCountConfig config = new RollingCountConfig(promotable, minActivityThreshold, timeToLive, idleTimeToLive, removable, windowInterval, windowBucketSize, idleCleanupInterval, timeProvider);
			return (RollingCountConfig)CountConfig.GetCachedObject(config);
		}

		public bool Equals(RollingCountConfig config)
		{
			return !object.ReferenceEquals(null, config) && (object.ReferenceEquals(this, config) || (this.windowInterval.Equals(config.windowInterval) && this.windowBucketSize.Equals(config.windowBucketSize) && base.Equals(config)));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is RollingCountConfig && this.Equals(obj as RollingCountConfig)));
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num = (num * 397 ^ this.windowInterval.GetHashCode());
			return num * 397 ^ this.windowBucketSize.GetHashCode();
		}

		private readonly TimeSpan windowInterval;

		private readonly TimeSpan windowBucketSize;
	}
}
