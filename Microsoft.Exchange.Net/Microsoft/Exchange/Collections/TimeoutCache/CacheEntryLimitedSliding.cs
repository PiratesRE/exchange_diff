using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class CacheEntryLimitedSliding<K, T> : CacheEntrySliding<K, T>
	{
		internal CacheEntryLimitedSliding(K key, T value, TimeSpan slidingLiveTime, TimeSpan maximumLiveTime) : base(key, value, slidingLiveTime)
		{
			if (maximumLiveTime < slidingLiveTime)
			{
				throw new ArgumentException("MaximumLiveTime must be greater than SlidingLiveTime.", "MaximumLiveTime");
			}
			this.maximumLiveTime = maximumLiveTime;
			this.maximumExpirationUtc = ((maximumLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(maximumLiveTime));
		}

		internal override void OnForceExtend()
		{
			base.OnForceExtend();
			this.maximumExpirationUtc = ((this.maximumLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(this.maximumLiveTime));
		}

		internal override void OnTouch()
		{
			base.OnTouch();
			if (this.touchedExpirationUtc > this.maximumExpirationUtc)
			{
				this.touchedExpirationUtc = this.maximumExpirationUtc;
			}
		}

		private readonly TimeSpan maximumLiveTime;

		private DateTime maximumExpirationUtc;
	}
}
