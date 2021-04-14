using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class CacheEntryFixed<K, T> : CacheEntryBase<K, T>
	{
		internal CacheEntryFixed(K key, T value, TimeSpan absoluteLiveTime) : base(key, value)
		{
			if (absoluteLiveTime <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("absoluteLiveTime", value, "value must be greater than zero.");
			}
			this.absoluteLiveTime = absoluteLiveTime;
			this.expirationUtc = ((absoluteLiveTime == TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.UtcNow.Add(absoluteLiveTime));
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
			this.expirationUtc = DateTime.UtcNow.Add(this.absoluteLiveTime);
		}

		internal override void OnTouch()
		{
		}

		internal override bool OnBeforeExpire()
		{
			return true;
		}

		private readonly TimeSpan absoluteLiveTime;

		private DateTime expirationUtc;
	}
}
