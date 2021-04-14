using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpdatableCacheEntry<T> where T : IUpdatableItem
	{
		public DateTime UtcOfExpiration { get; private set; }

		public DateTime UtcOfUpdaterFinish { get; private set; }

		public double TimeForUpdateInSeconds { get; private set; }

		public UpdatableCacheEntry(T itemToCache, DateTime expirationUtc, double timeForUpdateInSeconds)
		{
			this.CachedItem = itemToCache;
			this.UtcOfExpiration = expirationUtc;
			this.UtcOfUpdaterFinish = DateTime.MinValue;
			this.TimeForUpdateInSeconds = ((timeForUpdateInSeconds < 1.0) ? 1.0 : timeForUpdateInSeconds);
		}

		public bool GetCachedItem(out T cachedItem, DateTime currentUtcTime)
		{
			cachedItem = this.CachedItem;
			if (currentUtcTime < this.UtcOfExpiration)
			{
				return false;
			}
			if (this.UtcOfUpdaterFinish == DateTime.MinValue || currentUtcTime >= this.UtcOfUpdaterFinish)
			{
				this.UtcOfUpdaterFinish = currentUtcTime.AddSeconds(this.TimeForUpdateInSeconds);
				return true;
			}
			return false;
		}

		public bool UpdateCachedItem(T newItem, DateTime expirationUtc)
		{
			this.UtcOfExpiration = expirationUtc;
			this.UtcOfUpdaterFinish = DateTime.MinValue;
			return this.CachedItem.UpdateWith(newItem);
		}

		protected T CachedItem;
	}
}
