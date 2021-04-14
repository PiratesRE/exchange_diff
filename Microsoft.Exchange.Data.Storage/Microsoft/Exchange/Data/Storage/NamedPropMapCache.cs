using System;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NamedPropMapCache
	{
		internal NamedPropMapCache()
		{
			this.perfCounters.NamedPropertyCacheEntries.RawValue = 0L;
			this.currentMappingSize = 0;
			Action<int> mappingSizeChanged = delegate(int sizeDelta)
			{
				Interlocked.Add(ref this.currentMappingSize, sizeDelta);
			};
			double minSizeRatio = 0.9;
			this.lru = new LRUCache<string, NamedPropMap>(this.namedPropertyCacheNumberOfUsers, (string key) => new NamedPropMap(mappingSizeChanged), new double?(minSizeRatio), delegate()
			{
				this.perfCounters.NamedPropertyCacheMisses_Base.Increment();
			}, delegate()
			{
				this.perfCounters.NamedPropertyCacheMisses.Increment();
			}, () => this.currentMappingSize >= this.namedPropertyCachePropertiesPerUser * this.namedPropertyCacheNumberOfUsers, () => (double)this.currentMappingSize >= minSizeRatio * (double)this.namedPropertyCachePropertiesPerUser * (double)this.namedPropertyCacheNumberOfUsers, new Action<NamedPropMap>(this.ElementEvictCallback));
		}

		internal NamedPropMap GetMapping(string signature)
		{
			if (string.IsNullOrEmpty(signature))
			{
				return null;
			}
			bool flag;
			NamedPropMap result = this.lru.Get(signature, out flag);
			if (flag)
			{
				NamedPropertyDefinition.NamedPropertyKey.ClearUnreferenced();
			}
			return result;
		}

		private void ElementEvictCallback(NamedPropMap namedPropMapEvicted)
		{
			int num = namedPropMapEvicted.UnregisterSizeChangedDelegate();
			Interlocked.Add(ref this.currentMappingSize, -num);
			this.perfCounters.NamedPropertyCacheEntries.IncrementBy((long)(-(long)num));
		}

		public static NamedPropMapCache Default
		{
			get
			{
				return NamedPropMapCache.defaultInstance;
			}
		}

		internal NamedPropMap GetMapping(StoreSession storeSession)
		{
			if (storeSession != null)
			{
				return storeSession.NamedPropertyResolutionCache;
			}
			return null;
		}

		internal void UpdateCacheLimits(int namedPropertyCacheNumberOfUsers, int namedPropertyCachePropertiesPerUser, out int oldNamedPropertyCacheNumberOfUsers, out int oldNamedPropertyCachePropertiesPerUser)
		{
			int localNamedPropertyCacheNumberOfUsers = 0;
			int localNamedPropertyCachePropertiesPerUser = 0;
			this.lru.UpdateCapacity(this.namedPropertyCacheNumberOfUsers, delegate
			{
				localNamedPropertyCacheNumberOfUsers = this.namedPropertyCacheNumberOfUsers;
				localNamedPropertyCachePropertiesPerUser = this.namedPropertyCachePropertiesPerUser;
				this.namedPropertyCacheNumberOfUsers = namedPropertyCacheNumberOfUsers;
				this.namedPropertyCachePropertiesPerUser = namedPropertyCachePropertiesPerUser;
			});
			oldNamedPropertyCacheNumberOfUsers = localNamedPropertyCacheNumberOfUsers;
			oldNamedPropertyCachePropertiesPerUser = localNamedPropertyCachePropertiesPerUser;
		}

		internal void Reset()
		{
			this.lru.Reset();
			NamedPropertyDefinition.NamedPropertyKey.ClearUnreferenced();
		}

		private const int NamedPropertyCachePropertiesPerUserDefault = 100;

		private const int NamedPropertyCacheNumberOfUsersDefault = 2500;

		private readonly MiddleTierStoragePerformanceCountersInstance perfCounters = NamedPropMap.GetPerfCounters();

		private static readonly NamedPropMapCache defaultInstance = new NamedPropMapCache();

		private int namedPropertyCachePropertiesPerUser = 100;

		private int namedPropertyCacheNumberOfUsers = 100;

		private LRUCache<string, NamedPropMap> lru;

		private int currentMappingSize;
	}
}
