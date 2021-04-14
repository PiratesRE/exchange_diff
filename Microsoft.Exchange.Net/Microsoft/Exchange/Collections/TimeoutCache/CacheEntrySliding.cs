using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class CacheEntrySliding<K, T> : CacheEntryBase<K, T>
	{
		internal CacheEntrySliding(K key, T value, TimeSpan slidingLiveTime) : base(key, value)
		{
			if (slidingLiveTime <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("slidingLiveTime", slidingLiveTime, "value must be positive.");
			}
			this.slidingLiveTime = slidingLiveTime;
			this.expirationUtc = ((slidingLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(slidingLiveTime));
			this.touchedExpirationUtc = this.expirationUtc;
		}

		internal override DateTime ExpirationUtc
		{
			get
			{
				return this.expirationUtc;
			}
		}

		internal override void OnForceExtend()
		{
			this.expirationUtc = ((this.slidingLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(this.slidingLiveTime));
			this.touchedExpirationUtc = this.expirationUtc;
		}

		internal override void OnTouch()
		{
			if (!base.InShouldRemoveCycle)
			{
				this.touchedExpirationUtc = ((this.slidingLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(this.slidingLiveTime));
			}
		}

		internal override bool OnBeforeExpire()
		{
			if (this.touchedExpirationUtc > this.expirationUtc)
			{
				this.expirationUtc = this.touchedExpirationUtc;
				return false;
			}
			return true;
		}

		private readonly TimeSpan slidingLiveTime;

		private DateTime expirationUtc;

		protected DateTime touchedExpirationUtc;
	}
}
