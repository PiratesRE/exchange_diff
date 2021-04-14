using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class TimeoutCacheBucket<K, T> : IDisposable
	{
		public TimeoutCacheBucket(ShouldRemoveDelegate<K, T> shouldRemoveDelegate, int cacheSizeLimit, bool callbackOnDispose)
		{
			this.InitTimer();
			this.shouldRemoveDelegate = shouldRemoveDelegate;
			this.cacheSizeLimit = cacheSizeLimit;
			this.callbackOnDispose = callbackOnDispose;
		}

		internal int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal T Remove(K key)
		{
			CacheEntry<K, T> cacheEntry;
			lock (this.instanceLock)
			{
				if (this.items.TryGetValue(key, out cacheEntry))
				{
					this.InternalRemoveItem(cacheEntry, RemoveReason.Removed);
				}
			}
			if (cacheEntry != null)
			{
				return cacheEntry.Value;
			}
			return default(T);
		}

		internal bool Contains(K key)
		{
			bool result;
			lock (this.instanceLock)
			{
				result = this.items.ContainsKey(key);
			}
			return result;
		}

		internal T Get(K key)
		{
			T result;
			if (!this.TryGetValue(key, out result))
			{
				throw new KeyNotFoundException();
			}
			return result;
		}

		internal bool TryGetValue(K key, out T value)
		{
			lock (this.instanceLock)
			{
				CacheEntry<K, T> cacheEntry;
				if (this.items.TryGetValue(key, out cacheEntry))
				{
					if (cacheEntry.TimeoutType == TimeoutType.Sliding)
					{
						this.InternalRemoveFromSortedList(cacheEntry);
						cacheEntry.Extend();
						this.InternalAddToSortedList(cacheEntry);
					}
					value = cacheEntry.Value;
					return true;
				}
			}
			value = default(T);
			return false;
		}

		internal void AddAbsolute(K key, T value, TimeSpan expiration, RemoveItemDelegate<K, T> callback)
		{
			CacheEntry<K, T> entry = CacheEntry<K, T>.CreateAbsolute(expiration, key, value, callback);
			this.InternalAdd(entry);
		}

		internal void AddAbsolute(K key, T value, DateTime absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			CacheEntry<K, T> entry = CacheEntry<K, T>.CreateAbsolute(absoluteExpiration, key, value, callback);
			this.InternalAdd(entry);
		}

		internal void InsertAbsolute(K key, T value, DateTime absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			lock (this.instanceLock)
			{
				this.Remove(key);
				this.AddAbsolute(key, value, absoluteExpiration, callback);
			}
		}

		internal void InsertAbsolute(K key, T value, TimeSpan expiration, RemoveItemDelegate<K, T> callback)
		{
			lock (this.instanceLock)
			{
				this.Remove(key);
				this.AddAbsolute(key, value, expiration, callback);
			}
		}

		internal void AddSliding(K key, T value, TimeSpan slidingExpiration, RemoveItemDelegate<K, T> callback)
		{
			CacheEntry<K, T> entry = CacheEntry<K, T>.CreateSliding(slidingExpiration, key, value, callback);
			this.InternalAdd(entry);
		}

		internal void InsertSliding(K key, T value, TimeSpan slidingExpiration, RemoveItemDelegate<K, T> callback)
		{
			lock (this.instanceLock)
			{
				this.Remove(key);
				this.AddSliding(key, value, slidingExpiration, callback);
			}
		}

		internal void AddLimitedSliding(K key, T value, TimeSpan absoluteLiveTime, TimeSpan slidingLiveTime, RemoveItemDelegate<K, T> callback)
		{
			CacheEntry<K, T> entry = CacheEntry<K, T>.CreateLimitedSliding(slidingLiveTime, absoluteLiveTime, key, value, callback);
			this.InternalAdd(entry);
		}

		internal void InsertLimitedSliding(K key, T value, TimeSpan absoluteLiveTime, TimeSpan slidingLiveTime, RemoveItemDelegate<K, T> callback)
		{
			lock (this.instanceLock)
			{
				this.Remove(key);
				this.AddLimitedSliding(key, value, absoluteLiveTime, slidingLiveTime, callback);
			}
		}

		internal virtual void Clear()
		{
			List<CacheEntry<K, T>> list = null;
			lock (this.instanceLock)
			{
				if (this.callbackOnDispose)
				{
					foreach (KeyValuePair<K, CacheEntry<K, T>> keyValuePair in this.items)
					{
						if (keyValuePair.Value.Callback != null)
						{
							if (list == null)
							{
								list = new List<CacheEntry<K, T>>();
							}
							list.Add(keyValuePair.Value);
						}
					}
				}
				this.itemsByExpiration.Clear();
				this.items.Clear();
			}
			if (list != null)
			{
				this.FireRemoveCallbackAsync(list, RemoveReason.Cleanup);
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			lock (this.instanceLock)
			{
				this.Clear();
				this.DisposeTimer();
			}
		}

		private CacheEntry<K, T> AddEntryToChain(CacheEntry<K, T> chain, CacheEntry<K, T> entryToAdd)
		{
			if (chain == null)
			{
				return entryToAdd;
			}
			while (chain.Next != null)
			{
				chain = chain.Next;
			}
			chain.Next = entryToAdd;
			entryToAdd.Previous = chain;
			return chain;
		}

		private void InternalRemoveItemsByExpiration(DateTime expiration, RemoveReason reason)
		{
			List<CacheEntry<K, T>> list = null;
			lock (this.instanceLock)
			{
				while (this.itemsByExpiration.Count > 0 && this.GetOldestExpiration() <= expiration)
				{
					CacheEntry<K, T> next;
					for (CacheEntry<K, T> cacheEntry = this.PopOldestExpiration(); cacheEntry != null; cacheEntry = next)
					{
						next = cacheEntry.Next;
						cacheEntry.Extend();
						if (this.shouldRemoveDelegate == null || cacheEntry.NextExpirationTime <= expiration || this.shouldRemoveDelegate(cacheEntry.Key, cacheEntry.Value))
						{
							this.items.Remove(cacheEntry.Key);
							if (cacheEntry.Callback != null)
							{
								if (list == null)
								{
									list = new List<CacheEntry<K, T>>();
								}
								list.Add(cacheEntry);
							}
						}
						else
						{
							this.InternalAddToSortedList(cacheEntry);
						}
					}
				}
			}
			if (list != null)
			{
				this.FireRemoveCallbackAsync(list, reason);
			}
		}

		private object InternalRemoveItem(CacheEntry<K, T> entry, RemoveReason reason)
		{
			lock (this.instanceLock)
			{
				this.InternalRemoveFromSortedList(entry);
				this.items.Remove(entry.Key);
			}
			if (entry.Callback != null)
			{
				this.FireRemoveCallbackAsync(entry, reason);
			}
			return entry.Value;
		}

		private void RemoveCallbackWorker(object state)
		{
			TimeoutCacheBucket<K, T>.EntryAndReason entryAndReason = state as TimeoutCacheBucket<K, T>.EntryAndReason;
			this.RemoveCallbackSingleEntry(entryAndReason.Entry, entryAndReason.Reason);
		}

		private void RemoveCallbackWorkerArray(object state)
		{
			TimeoutCacheBucket<K, T>.EntryListAndReason entryListAndReason = state as TimeoutCacheBucket<K, T>.EntryListAndReason;
			List<CacheEntry<K, T>> entryList = entryListAndReason.EntryList;
			foreach (CacheEntry<K, T> cacheEntry in entryList)
			{
				this.RemoveCallbackSingleEntry(cacheEntry, entryListAndReason.Reason);
			}
		}

		private void RemoveCallbackSingleEntry(CacheEntry<K, T> cacheEntry, RemoveReason reason)
		{
			if (cacheEntry.Callback != null)
			{
				cacheEntry.Callback(cacheEntry.Key, cacheEntry.Value, reason);
			}
		}

		private void InternalAdd(CacheEntry<K, T> entry)
		{
			lock (this.instanceLock)
			{
				if (this.Count >= this.cacheSizeLimit)
				{
					this.PreemptiveExpire();
				}
				if (this.items.ContainsKey(entry.Key))
				{
					throw new DuplicateKeyException();
				}
				this.items.Add(entry.Key, entry);
				this.InternalAddToSortedList(entry);
			}
		}

		private void PreemptiveExpire()
		{
			if (this.itemsByExpiration.Count == 0)
			{
				lock (this.instanceLock)
				{
					CacheEntry<K, T> entry = null;
					using (Dictionary<K, CacheEntry<K, T>>.KeyCollection.Enumerator enumerator = this.items.Keys.GetEnumerator())
					{
						enumerator.MoveNext();
						entry = this.items[enumerator.Current];
					}
					this.InternalRemoveItem(entry, RemoveReason.PreemptivelyExpired);
					return;
				}
			}
			DateTime expiration = (DateTime.UtcNow > this.GetOldestExpiration()) ? DateTime.UtcNow : this.GetOldestExpiration();
			this.InternalRemoveItemsByExpiration(expiration, RemoveReason.PreemptivelyExpired);
		}

		private void InternalAddToSortedList(CacheEntry<K, T> entry)
		{
			if (entry.TimeoutType == TimeoutType.Absolute && entry.AbsoluteExpirationTime == DateTime.MaxValue)
			{
				return;
			}
			lock (this.instanceLock)
			{
				this.InternalRemoveFromSortedList(entry);
				CacheEntry<K, T> chain;
				if (!this.itemsByExpiration.TryGetValue(entry.NextExpirationTime, out chain))
				{
					this.itemsByExpiration.Add(entry.NextExpirationTime, entry);
				}
				else
				{
					this.AddEntryToChain(chain, entry);
				}
			}
		}

		private void InternalRemoveFromSortedList(CacheEntry<K, T> entryToRemove)
		{
			lock (this.instanceLock)
			{
				CacheEntry<K, T> previous = entryToRemove.Previous;
				CacheEntry<K, T> next = entryToRemove.Next;
				entryToRemove.Previous = null;
				entryToRemove.Next = null;
				if (previous != null)
				{
					previous.Next = next;
				}
				if (next != null)
				{
					next.Previous = previous;
				}
				CacheEntry<K, T> cacheEntry;
				if (this.itemsByExpiration.TryGetValue(entryToRemove.NextExpirationTime, out cacheEntry) && cacheEntry == entryToRemove)
				{
					if (next == null)
					{
						this.itemsByExpiration.Remove(entryToRemove.NextExpirationTime);
					}
					else
					{
						this.itemsByExpiration[cacheEntry.NextExpirationTime] = next;
					}
				}
			}
		}

		private void HandleTimer(object sender, ElapsedEventArgs e)
		{
			lock (this.instanceLock)
			{
				if (this.itemsByExpiration.Count > 0)
				{
					this.InternalRemoveItemsByExpiration(DateTime.UtcNow, RemoveReason.Expired);
				}
			}
		}

		private void FireRemoveCallbackAsync(CacheEntry<K, T> entry, RemoveReason reason)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveCallbackWorker), new TimeoutCacheBucket<K, T>.EntryAndReason(entry, reason));
		}

		private void FireRemoveCallbackAsync(List<CacheEntry<K, T>> entries, RemoveReason reason)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveCallbackWorkerArray), new TimeoutCacheBucket<K, T>.EntryListAndReason(entries, reason));
		}

		private DateTime GetOldestExpiration()
		{
			return this.itemsByExpiration.Keys[0];
		}

		private CacheEntry<K, T> PopOldestExpiration()
		{
			CacheEntry<K, T> result = this.itemsByExpiration.Values[0];
			this.itemsByExpiration.RemoveAt(0);
			return result;
		}

		private void InitTimer()
		{
			this.timer.AutoReset = true;
			this.timer.Enabled = true;
			this.timer.Interval = 10000.0;
			this.timer.Elapsed += this.HandleTimer;
		}

		private void DisposeTimer()
		{
			this.timer.Dispose();
			this.timer = null;
		}

		internal const int TimerInterval = 10000;

		private Dictionary<K, CacheEntry<K, T>> items = new Dictionary<K, CacheEntry<K, T>>();

		private object instanceLock = new object();

		private ShouldRemoveDelegate<K, T> shouldRemoveDelegate;

		private bool disposed;

		private int cacheSizeLimit;

		private bool callbackOnDispose;

		private SortedList<DateTime, CacheEntry<K, T>> itemsByExpiration = new SortedList<DateTime, CacheEntry<K, T>>();

		private System.Timers.Timer timer = new System.Timers.Timer();

		private class EntryAndReason
		{
			public CacheEntry<K, T> Entry { get; private set; }

			public RemoveReason Reason { get; private set; }

			public EntryAndReason(CacheEntry<K, T> entry, RemoveReason reason)
			{
				this.Entry = entry;
				this.Reason = reason;
			}
		}

		private class EntryListAndReason
		{
			public List<CacheEntry<K, T>> EntryList { get; private set; }

			public RemoveReason Reason { get; private set; }

			public EntryListAndReason(List<CacheEntry<K, T>> entryList, RemoveReason reason)
			{
				this.EntryList = entryList;
				this.Reason = reason;
			}
		}
	}
}
