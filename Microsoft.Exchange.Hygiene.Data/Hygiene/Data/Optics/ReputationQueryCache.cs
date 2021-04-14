using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;
using Microsoft.Exchange.Common.Reputation;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	internal class ReputationQueryCache
	{
		public bool Initialized { get; private set; }

		public void InitializeCache()
		{
			if (this.Initialized)
			{
				return;
			}
			lock (this.cacheInitializeLock)
			{
				if (!this.Initialized)
				{
					this.caches = new Dictionary<ReputationEntityType, MemoryCache>();
					for (ReputationEntityType reputationEntityType = ReputationEntityType.IP; reputationEntityType < ReputationEntityType.Max; reputationEntityType += 1)
					{
						this.caches.Add(reputationEntityType, new MemoryCache(string.Format("ReputationCache_{0}", reputationEntityType.ToString()), new NameValueCollection
						{
							{
								"physicalMemoryLimitPercentage",
								"1"
							},
							{
								"pollingInterval",
								"00:00:01"
							}
						}));
					}
					this.Initialized = true;
				}
			}
		}

		public bool TryGetValue(ReputationEntityType entityType, int dataPoint, string reputationEntityKey, out long value)
		{
			value = 0L;
			MemoryCache memoryCache = null;
			if (!this.caches.TryGetValue(entityType, out memoryCache))
			{
				return false;
			}
			object obj = memoryCache.Get(this.GetCacheIndex(reputationEntityKey, dataPoint), null);
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(long))
			{
				return false;
			}
			value = (long)obj;
			return true;
		}

		public bool TryAddValue(ReputationEntityType entityType, int dataPoint, string reputationEntityKey, long value, int ttl)
		{
			MemoryCache memoryCache = null;
			return this.caches.TryGetValue(entityType, out memoryCache) && ttl >= 0 && memoryCache.Add(new CacheItem(this.GetCacheIndex(reputationEntityKey, dataPoint), value), new CacheItemPolicy
			{
				SlidingExpiration = TimeSpan.FromMilliseconds((double)ttl)
			});
		}

		private string GetCacheIndex(string reputationEntityKey, int dataPoint)
		{
			return string.Format("{0}:{1}", reputationEntityKey, dataPoint);
		}

		private Dictionary<ReputationEntityType, MemoryCache> caches;

		private object cacheInitializeLock = new object();
	}
}
