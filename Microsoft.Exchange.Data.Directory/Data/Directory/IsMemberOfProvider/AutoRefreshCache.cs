using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal class AutoRefreshCache<K, V, StateType> : IDisposable where V : CachableItem
	{
		public AutoRefreshCache(long cacheSizeInBytes, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval, TimeSpan cachePurgeInterval, TimeSpan refreshInterval, ICacheTracer<K> tracer, ICachePerformanceCounters perfCounters, AutoRefreshCache<K, V, StateType>.CreateEntryDelegate createEntry)
		{
			if (createEntry == null)
			{
				throw new ArgumentNullException("createEntry");
			}
			this.createEntry = createEntry;
			this.tracer = tracer;
			this.cache = new Cache<K, V>(cacheSizeInBytes, cacheExpirationInterval, cacheCleanupInterval, cachePurgeInterval, tracer, perfCounters);
			this.itemsToRefresh = new List<K>();
			this.refreshExpiredEntriesTimer = new GuardedTimer(new TimerCallback(this.RefreshExpiredEntries), null, refreshInterval);
			for (int i = 0; i < this.groupsInADLookupSyncObjects.Length; i++)
			{
				this.groupsInADLookupSyncObjects[i] = new object();
			}
		}

		public int Count
		{
			get
			{
				return this.cache.Count;
			}
		}

		public V GetValue(StateType state, K key)
		{
			V result = default(V);
			if (!this.disposed)
			{
				bool flag = false;
				if (!this.cache.TryGetValue(key, out result, out flag))
				{
					result = this.CreateAndCache(state, key, false);
				}
				if (flag)
				{
					lock (this.itemsToRefresh)
					{
						if (!this.itemsToRefresh.Contains(key))
						{
							this.itemsToRefresh.Add(key);
						}
					}
				}
			}
			return result;
		}

		public void Remove(K key)
		{
			lock (this.itemsToRefresh)
			{
				this.itemsToRefresh.Remove(key);
			}
			this.cache.Remove(key);
		}

		public void Clear()
		{
			this.cache.Clear();
			lock (this.itemsToRefresh)
			{
				this.itemsToRefresh.Clear();
			}
		}

		public void Dispose()
		{
			this.refreshExpiredEntriesTimer.Dispose(true);
			this.cache.Dispose();
			this.disposed = true;
		}

		private void RefreshExpiredEntries(object unused)
		{
			List<K> list;
			lock (this.itemsToRefresh)
			{
				list = new List<K>(this.itemsToRefresh);
				this.itemsToRefresh.Clear();
			}
			foreach (K k in list)
			{
				if (this.disposed)
				{
					break;
				}
				if (this.cache.ContainsKey(k))
				{
					try
					{
						this.CreateAndCache(default(StateType), k, true);
					}
					catch (TransientException exception)
					{
						this.tracer.TraceException(string.Format("Encountered a Transient exception while refreshing the expired entry {0}", k), exception);
						lock (this.itemsToRefresh)
						{
							this.itemsToRefresh.Add(k);
						}
					}
				}
			}
		}

		private V CreateAndCache(StateType state, K key, bool forceRefreshEntry)
		{
			uint hashCode = (uint)key.GetHashCode();
			uint num = (hashCode & 255U) ^ (hashCode >> 8 & 255U) ^ (hashCode >> 16 & 255U) ^ (hashCode >> 24 & 255U);
			V v = default(V);
			lock (this.groupsInADLookupSyncObjects[(int)((UIntPtr)num)])
			{
				if (!forceRefreshEntry && this.cache.ContainsKey(key))
				{
					bool flag2;
					this.cache.TryGetValue(key, out v, out flag2);
				}
				else
				{
					v = this.createEntry(state, key);
					if (!this.disposed)
					{
						this.cache.TryAdd(key, v);
					}
				}
			}
			return v;
		}

		private const int GroupsInADLookupSyncObjectsCount = 256;

		private readonly ICacheTracer<K> tracer;

		private Cache<K, V> cache;

		private List<K> itemsToRefresh;

		private AutoRefreshCache<K, V, StateType>.CreateEntryDelegate createEntry;

		private GuardedTimer refreshExpiredEntriesTimer;

		private object[] groupsInADLookupSyncObjects = new object[256];

		private bool disposed;

		public delegate V CreateEntryDelegate(StateType state, K key);
	}
}
