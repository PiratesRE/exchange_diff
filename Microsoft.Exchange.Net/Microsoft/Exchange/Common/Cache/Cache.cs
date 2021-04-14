using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Common.Cache
{
	internal class Cache<K, V> : IDisposable where V : CachableItem
	{
		static Cache()
		{
			if (typeof(V).GetInterface(typeof(IDisposable).FullName) != null)
			{
				throw new ArgumentException("Disposable cached item are not supported", "V");
			}
		}

		public Cache(long cacheSizeInBytes, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval) : this(cacheSizeInBytes, cacheExpirationInterval, cacheCleanupInterval, null, null)
		{
		}

		public Cache(long cacheSizeInBytes, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval, ICacheTracer<K> tracer, ICachePerformanceCounters perfCounters) : this(cacheSizeInBytes, cacheExpirationInterval, cacheCleanupInterval, Cache<K, V>.DefaultPurgeInterval, tracer, perfCounters)
		{
		}

		public Cache(long cacheSizeInBytes, TimeSpan cacheExpirationInterval, TimeSpan cacheCleanupInterval, TimeSpan cachePurgeInterval, ICacheTracer<K> tracer, ICachePerformanceCounters perfCounters)
		{
			if (cacheSizeInBytes < 0L)
			{
				throw new ArgumentOutOfRangeException("cacheSizeInBytes", cacheSizeInBytes, "cacheSizeInBytes must be greater than or equal to 0 bytes");
			}
			if (cacheExpirationInterval.TotalSeconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("cacheExpirationInterval", cacheExpirationInterval, "Expire time must be greater than or equal to 0 seconds");
			}
			if (cacheCleanupInterval.TotalSeconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("cacheCleanupInterval", cacheCleanupInterval, "Cleanup time must be greater than or equal to 0 seconds");
			}
			if (cachePurgeInterval.TotalSeconds <= 0.0)
			{
				throw new ArgumentOutOfRangeException("cachePurgeInterval", cachePurgeInterval, "Purge time must be greater than 0 seconds");
			}
			this.cacheTracer = (tracer ?? new Cache<K, V>.NoopCacheTracer());
			this.cachePerfCounters = (perfCounters ?? new Cache<K, V>.NoopCachePerformanceCounters());
			this.maxCacheSize = cacheSizeInBytes;
			this.cacheExpirationInterval = cacheExpirationInterval;
			this.cleanupInterval = cacheCleanupInterval + cacheExpirationInterval;
			this.currentSize = 0L;
			this.memoryCache = new Dictionary<K, Cache<K, V>.CacheItemWrapper>();
			this.mruList = new Cache<K, V>.MruList();
			this.cleanupTimer = new GuardedTimer(new TimerCallback(this.HandleCleanUp), null, cachePurgeInterval);
		}

		public event Cache<K, V>.OnRemovedEventHandler OnRemoved;

		public int Count
		{
			get
			{
				return this.memoryCache.Count;
			}
		}

		public virtual bool TryAdd(K key, V value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			long itemSize = value.ItemSize;
			if (itemSize > this.maxCacheSize || this.maxCacheSize == 0L)
			{
				return false;
			}
			DateTime utcNow;
			lock (this.syncObject)
			{
				utcNow = DateTime.UtcNow;
				if (this.memoryCache.ContainsKey(key))
				{
					this.Remove(key, CacheItemRemovedReason.OverWritten);
				}
				Cache<K, V>.CacheItemWrapper cacheItemWrapper;
				while (this.currentSize + itemSize > this.maxCacheSize)
				{
					cacheItemWrapper = this.mruList.Oldest;
					this.Remove(cacheItemWrapper.ItemKey, CacheItemRemovedReason.Scavenged);
				}
				cacheItemWrapper = new Cache<K, V>.CacheItemWrapper(key, value);
				this.mruList.Add(cacheItemWrapper);
				this.memoryCache[key] = cacheItemWrapper;
				this.currentSize += itemSize;
			}
			this.cacheTracer.ItemAdded(key, value, utcNow);
			this.cachePerfCounters.SizeUpdated(this.currentSize);
			return true;
		}

		public void Add(K key, V value)
		{
			if (!this.TryAdd(key, value))
			{
				throw new ArgumentException("Value cannot be added for given key");
			}
		}

		public void Remove(K key)
		{
			this.Remove(key, CacheItemRemovedReason.Removed);
		}

		public bool GetAllValues(out ICollection<V> values)
		{
			values = null;
			bool flag = false;
			List<V> list = new List<V>();
			if (!this.disposed)
			{
				lock (this.syncObject)
				{
					foreach (K key in new List<K>(this.memoryCache.Keys))
					{
						V item;
						bool flag3;
						if (this.TryGetValue(key, out item, out flag3))
						{
							list.Add(item);
						}
						else
						{
							flag3 = true;
						}
						if (flag3 && !flag)
						{
							flag = true;
						}
					}
				}
			}
			values = list.AsReadOnly();
			return flag;
		}

		public V GetValue(K key)
		{
			bool flag;
			return this.GetValue(key, out flag);
		}

		public V GetValue(K key, out bool hasExpired)
		{
			V result;
			if (!this.TryGetValue(key, out result, out hasExpired))
			{
				throw new ArgumentException("Value does not exist for given key");
			}
			return result;
		}

		public bool TryGetValue(K key, out V value)
		{
			bool flag;
			return this.TryGetValue(key, out value, out flag);
		}

		public bool TryGetValue(K key, out V value, out bool hasExpired)
		{
			value = default(V);
			hasExpired = false;
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			AccessStatus accessStatus = AccessStatus.Miss;
			if (!this.disposed)
			{
				DateTime utcNow;
				lock (this.syncObject)
				{
					utcNow = DateTime.UtcNow;
					Cache<K, V>.CacheItemWrapper cacheItemWrapper;
					if (this.memoryCache.TryGetValue(key, out cacheItemWrapper) && cacheItemWrapper != null)
					{
						if (!(cacheItemWrapper.CreationTime < utcNow.Subtract(this.cleanupInterval)))
						{
							V cacheItem = cacheItemWrapper.CacheItem;
							if (!cacheItem.IsExpired(utcNow))
							{
								this.mruList.Remove(cacheItemWrapper);
								this.mruList.Add(cacheItemWrapper);
								value = cacheItemWrapper.CacheItem;
								if (cacheItemWrapper.CreationTime < utcNow.Subtract(this.cacheExpirationInterval))
								{
									hasExpired = true;
								}
								accessStatus = AccessStatus.Hit;
								goto IL_D7;
							}
						}
						this.Remove(key, CacheItemRemovedReason.Expired);
					}
					IL_D7:;
				}
				this.cacheTracer.Accessed(key, value, accessStatus, utcNow);
				this.cachePerfCounters.Accessed(accessStatus);
			}
			return accessStatus == AccessStatus.Hit;
		}

		public virtual bool ContainsKey(K key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!this.disposed)
			{
				lock (this.syncObject)
				{
					Cache<K, V>.CacheItemWrapper cacheItemWrapper;
					return this.memoryCache.TryGetValue(key, out cacheItemWrapper) && cacheItemWrapper != null;
				}
				return false;
			}
			return false;
		}

		public void Clear()
		{
			Dictionary<K, Cache<K, V>.CacheItemWrapper> dictionary = null;
			DateTime utcNow;
			lock (this.syncObject)
			{
				utcNow = DateTime.UtcNow;
				this.currentSize = 0L;
				dictionary = this.memoryCache;
				this.memoryCache = new Dictionary<K, Cache<K, V>.CacheItemWrapper>();
				this.mruList.Clear();
			}
			this.cacheTracer.Flushed(this.maxCacheSize, utcNow);
			this.cachePerfCounters.SizeUpdated(this.currentSize);
			if (this.OnRemoved != null)
			{
				foreach (KeyValuePair<K, Cache<K, V>.CacheItemWrapper> keyValuePair in dictionary)
				{
					this.RaiseOnRemovedEvent(keyValuePair.Key, keyValuePair.Value.CacheItem, CacheItemRemovedReason.Clear);
				}
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.cleanupTimer.Dispose(true);
				this.disposed = true;
				this.Clear();
			}
		}

		private void HandleCleanUp(object state)
		{
			lock (this.syncObject)
			{
				List<K> list = new List<K>();
				DateTime utcNow = DateTime.UtcNow;
				DateTime t = utcNow.Subtract(this.cleanupInterval);
				foreach (KeyValuePair<K, Cache<K, V>.CacheItemWrapper> keyValuePair in this.memoryCache)
				{
					Cache<K, V>.CacheItemWrapper value = keyValuePair.Value;
					if (!(value.CreationTime < t))
					{
						V cacheItem = value.CacheItem;
						if (!cacheItem.IsExpired(utcNow))
						{
							continue;
						}
					}
					list.Add(value.ItemKey);
				}
				foreach (K key in list)
				{
					this.Remove(key, CacheItemRemovedReason.Expired);
				}
			}
		}

		private void Remove(K key, CacheItemRemovedReason removalReason)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!this.disposed)
			{
				bool flag = false;
				Cache<K, V>.CacheItemWrapper cacheItemWrapper = null;
				DateTime utcNow;
				lock (this.syncObject)
				{
					utcNow = DateTime.UtcNow;
					if (this.memoryCache.TryGetValue(key, out cacheItemWrapper))
					{
						this.mruList.Remove(cacheItemWrapper);
						this.memoryCache.Remove(key);
						long num = this.currentSize;
						V cacheItem = cacheItemWrapper.CacheItem;
						this.currentSize = num - cacheItem.ItemSize;
						flag = true;
					}
				}
				if (flag)
				{
					this.cacheTracer.ItemRemoved(key, cacheItemWrapper.CacheItem, removalReason, utcNow);
					this.cachePerfCounters.SizeUpdated(this.currentSize);
					this.RaiseOnRemovedEvent(key, cacheItemWrapper.CacheItem, removalReason);
				}
			}
		}

		private void RaiseOnRemovedEvent(K key, V value, CacheItemRemovedReason removalReason)
		{
			Cache<K, V>.OnRemovedEventHandler onRemoved = this.OnRemoved;
			if (onRemoved != null)
			{
				OnRemovedEventArgs<K, V> e = new OnRemovedEventArgs<K, V>(key, value, removalReason);
				onRemoved(this, e);
			}
		}

		public static readonly TimeSpan DefaultPurgeInterval = TimeSpan.FromMinutes(5.0);

		private readonly long maxCacheSize;

		private readonly TimeSpan cacheExpirationInterval;

		private readonly ICacheTracer<K> cacheTracer;

		private readonly ICachePerformanceCounters cachePerfCounters;

		private readonly TimeSpan cleanupInterval;

		private long currentSize;

		private GuardedTimer cleanupTimer;

		private bool disposed;

		private object syncObject = new object();

		private Dictionary<K, Cache<K, V>.CacheItemWrapper> memoryCache;

		private Cache<K, V>.MruList mruList;

		public delegate void OnRemovedEventHandler(object sender, OnRemovedEventArgs<K, V> e);

		[DebuggerDisplay("{itemKey.ToString()}, Created = {creationTime.ToString()}")]
		private class CacheItemWrapper
		{
			public CacheItemWrapper(K itemKey, V cacheItem)
			{
				this.creationTime = DateTime.UtcNow;
				this.cacheItem = cacheItem;
				this.itemKey = itemKey;
			}

			public DateTime CreationTime
			{
				get
				{
					return this.creationTime;
				}
			}

			public K ItemKey
			{
				get
				{
					return this.itemKey;
				}
			}

			public V CacheItem
			{
				get
				{
					return this.cacheItem;
				}
			}

			public Cache<K, V>.CacheItemWrapper Next
			{
				get
				{
					return this.next;
				}
				set
				{
					this.next = value;
				}
			}

			public Cache<K, V>.CacheItemWrapper Previous
			{
				get
				{
					return this.previous;
				}
				set
				{
					this.previous = value;
				}
			}

			private DateTime creationTime;

			private V cacheItem;

			private K itemKey;

			private Cache<K, V>.CacheItemWrapper next;

			private Cache<K, V>.CacheItemWrapper previous;
		}

		private class MruList
		{
			public Cache<K, V>.CacheItemWrapper Oldest
			{
				get
				{
					return this.tail;
				}
			}

			public void Add(Cache<K, V>.CacheItemWrapper item)
			{
				item.Next = this.head;
				item.Previous = null;
				if (this.head != null)
				{
					this.head.Previous = item;
				}
				this.head = item;
				if (this.tail == null)
				{
					this.tail = item;
				}
			}

			public void Remove(Cache<K, V>.CacheItemWrapper item)
			{
				if (this.head == null || this.tail == null)
				{
					throw new InvalidOperationException("Cannot remove from an empty list");
				}
				if (item.Previous != null)
				{
					item.Previous.Next = item.Next;
				}
				else
				{
					this.head = this.head.Next;
				}
				if (item.Next != null)
				{
					item.Next.Previous = item.Previous;
				}
				else
				{
					this.tail = this.tail.Previous;
				}
				item.Previous = null;
				item.Next = null;
			}

			public void Clear()
			{
				this.head = null;
				this.tail = null;
			}

			[Conditional("DEBUG")]
			private void Validate(Cache<K, V>.CacheItemWrapper item)
			{
				if (this.head != null && this.head.Previous != null)
				{
					throw new InvalidOperationException("Head does not point to the start of the list.");
				}
				if (this.tail != null && this.tail.Next != null)
				{
					throw new InvalidOperationException("Tail does not point to the end of the list.");
				}
				if ((this.head == null && this.tail != null) || (this.head != null && this.head == null))
				{
					throw new InvalidOperationException("Head and tail are inconsistently null.");
				}
				if (this.head != null)
				{
					bool flag = false;
					bool flag2 = false;
					for (Cache<K, V>.CacheItemWrapper cacheItemWrapper = this.head; cacheItemWrapper != null; cacheItemWrapper = cacheItemWrapper.Next)
					{
						flag = false;
						if (cacheItemWrapper == item)
						{
							flag2 = true;
						}
						if (cacheItemWrapper == this.tail)
						{
							flag = true;
						}
					}
					if (item != null && !flag2)
					{
						throw new InvalidOperationException("Item not reachable from head.");
					}
					if (!flag)
					{
						throw new InvalidOperationException("Tail not reachable from head.");
					}
					bool flag3 = false;
					flag2 = false;
					for (Cache<K, V>.CacheItemWrapper cacheItemWrapper = this.tail; cacheItemWrapper != null; cacheItemWrapper = cacheItemWrapper.Previous)
					{
						flag3 = false;
						if (cacheItemWrapper == item)
						{
							flag2 = true;
						}
						if (cacheItemWrapper == this.head)
						{
							flag3 = true;
						}
					}
					if (item != null && !flag2)
					{
						throw new InvalidOperationException("Item not reachable from tail.");
					}
					if (!flag3)
					{
						throw new InvalidOperationException("Head not reachable from tail.");
					}
				}
			}

			private Cache<K, V>.CacheItemWrapper head;

			private Cache<K, V>.CacheItemWrapper tail;
		}

		private class NoopCacheTracer : ICacheTracer<K>
		{
			public void Accessed(K key, CachableItem value, AccessStatus accessStatus, DateTime timestamp)
			{
			}

			public void Flushed(long cacheSize, DateTime timestamp)
			{
			}

			public void ItemAdded(K key, CachableItem value, DateTime timestamp)
			{
			}

			public void ItemRemoved(K key, CachableItem value, CacheItemRemovedReason removeReason, DateTime timestamp)
			{
			}

			public void TraceException(string details, Exception exception)
			{
			}
		}

		private class NoopCachePerformanceCounters : ICachePerformanceCounters
		{
			public void Accessed(AccessStatus accessStatus)
			{
			}

			public void SizeUpdated(long cacheSize)
			{
			}
		}
	}
}
