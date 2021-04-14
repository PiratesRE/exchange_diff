using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpdatableCache<K, T> where K : class where T : IUpdatableItem
	{
		public UpdatableCache(int cacheCapacity, double timeForUpdateInSeconds)
		{
			this.MaxCount = ((cacheCapacity < 1) ? 1 : cacheCapacity);
			this.TimeForUpdateInSeconds = timeForUpdateInSeconds;
			this.CachedItems = new Dictionary<K, UpdatableCacheEntry<T>>(this.MaxCount);
		}

		public bool UpdateCacheEntry(K key, ref T value, DateTime expirationUtc)
		{
			if (key == null || value == null)
			{
				return false;
			}
			lock (this.Lock)
			{
				UpdatableCacheEntry<T> updatableCacheEntry;
				if (this.CachedItems.TryGetValue(key, out updatableCacheEntry))
				{
					if (!updatableCacheEntry.UpdateCachedItem(value, expirationUtc))
					{
						T t;
						updatableCacheEntry.GetCachedItem(out t, DateTime.UtcNow);
						value = t;
						return false;
					}
				}
				else
				{
					if (this.CachedItems.Count == this.MaxCount)
					{
						K key2 = (from pair in this.CachedItems
						orderby pair.Value.UtcOfExpiration
						select pair).First<KeyValuePair<K, UpdatableCacheEntry<T>>>().Key;
						this.CachedItems.Remove(key2);
					}
					this.CachedItems[key] = new UpdatableCacheEntry<T>(value, expirationUtc, this.TimeForUpdateInSeconds);
				}
			}
			return true;
		}

		public bool GetCacheEntry(K key, out T value, out bool expired, DateTime currentUtcTime)
		{
			value = default(T);
			expired = false;
			if (key == null)
			{
				return false;
			}
			lock (this.Lock)
			{
				UpdatableCacheEntry<T> updatableCacheEntry;
				if (this.CachedItems.TryGetValue(key, out updatableCacheEntry))
				{
					expired = updatableCacheEntry.GetCachedItem(out value, currentUtcTime);
					return true;
				}
			}
			return false;
		}

		protected readonly int MaxCount;

		protected readonly double TimeForUpdateInSeconds;

		protected object Lock = new object();

		protected readonly Dictionary<K, UpdatableCacheEntry<T>> CachedItems;
	}
}
