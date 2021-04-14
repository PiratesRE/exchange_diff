using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal abstract class CacheEntryBase<K, T>
	{
		protected CacheEntryBase(K key, T value)
		{
			this.key = key;
			this.value = value;
		}

		internal T Value
		{
			get
			{
				return this.value;
			}
		}

		internal K Key
		{
			get
			{
				return this.key;
			}
		}

		internal bool InShouldRemoveCycle { get; set; }

		internal CacheEntryBase<K, T> Next { get; set; }

		internal CacheEntryBase<K, T> Previous { get; set; }

		internal abstract void OnTouch();

		internal abstract bool OnBeforeExpire();

		internal abstract void OnForceExtend();

		internal abstract DateTime ExpirationUtc { get; }

		private readonly K key;

		private readonly T value;
	}
}
