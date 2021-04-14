using System;
using System.Runtime.Caching;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal abstract class Entry
	{
		public int RetryCount { get; set; }

		public Guid CorrelationId { get; set; }

		public string MessageId { get; set; }

		public ComplianceMessage Message { get; set; }

		protected TimeSpan ExpiryTime
		{
			get
			{
				return this.expiryTime;
			}
			set
			{
				this.expiryTime = value;
			}
		}

		protected bool KeepAlive
		{
			get
			{
				return this.keepAlive;
			}
			set
			{
				this.keepAlive = value;
			}
		}

		public abstract string GetKey();

		public virtual CacheItem GetCacheItem()
		{
			if (this.cacheItem == null)
			{
				this.cacheItem = new CacheItem(this.GetKey(), this);
			}
			return this.cacheItem;
		}

		public virtual CacheItemPolicy GetCachePolicy()
		{
			if (this.cachePolicy == null)
			{
				this.cachePolicy = new CacheItemPolicy
				{
					SlidingExpiration = this.ExpiryTime,
					RemovedCallback = new CacheEntryRemovedCallback(this.CacheExpiry)
				};
			}
			return this.cachePolicy;
		}

		public Entry UpdateCache(ObjectCache cache)
		{
			CacheItem value = this.GetCacheItem();
			CacheItem cacheItem = cache.AddOrGetExisting(value, this.GetCachePolicy());
			if (cacheItem.Value != null)
			{
				Entry entry = cacheItem.Value as Entry;
				this.UpdateExistingEntry(entry);
				return entry;
			}
			return this;
		}

		public abstract void EvaluateState(bool expired);

		protected abstract void UpdateExistingEntry(Entry existing);

		protected virtual void CacheExpiry(CacheEntryRemovedArguments e)
		{
			if (e.RemovedReason == CacheEntryRemovedReason.Expired)
			{
				this.RetryCount++;
				this.EvaluateState(true);
				if (!this.KeepAlive)
				{
					return;
				}
			}
			else if (e.RemovedReason == CacheEntryRemovedReason.Removed)
			{
				return;
			}
			this.UpdateCache(e.Source);
		}

		private TimeSpan expiryTime = new TimeSpan(0, 5, 0);

		private bool keepAlive = true;

		private CacheItem cacheItem;

		private CacheItemPolicy cachePolicy;
	}
}
