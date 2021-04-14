using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class ExactTimeoutCache<K, T> : IDisposable
	{
		public ExactTimeoutCache(RemoveItemDelegate<K, T> removeItemDelegate, ShouldRemoveDelegate<K, T> shouldRemoveDelegate, UnhandledExceptionDelegate unhandledExceptionDelegate, int cacheSizeLimit, bool callbackOnDispose) : this(removeItemDelegate, shouldRemoveDelegate, unhandledExceptionDelegate, cacheSizeLimit, callbackOnDispose, CacheFullBehavior.ExpireExisting)
		{
		}

		public ExactTimeoutCache(RemoveItemDelegate<K, T> removeItemDelegate, ShouldRemoveDelegate<K, T> shouldRemoveDelegate, UnhandledExceptionDelegate unhandledExceptionDelegate, int cacheSizeLimit, bool callbackOnDispose, CacheFullBehavior cacheFullBehavior)
		{
			this.removeItemDelegate = removeItemDelegate;
			this.shouldRemoveDelegate = shouldRemoveDelegate;
			this.cacheSizeLimit = cacheSizeLimit;
			this.callbackOnDispose = callbackOnDispose;
			this.unhandledExceptionDelegate = unhandledExceptionDelegate;
			this.cacheFullBehavior = cacheFullBehavior;
			this.CreateWorkerThread();
		}

		~ExactTimeoutCache()
		{
			this.Dispose(false);
		}

		internal int Count
		{
			get
			{
				bool flag = false;
				int count;
				try
				{
					flag = this.readerWriterLock.TryEnterReadLock(-1);
					count = this.items.Count;
				}
				finally
				{
					if (flag || this.readerWriterLock.IsReadLockHeld)
					{
						this.readerWriterLock.ExitReadLock();
					}
				}
				return count;
			}
		}

		internal List<K> Keys
		{
			get
			{
				bool flag = false;
				List<K> result;
				try
				{
					flag = this.readerWriterLock.TryEnterReadLock(-1);
					List<K> list = new List<K>(this.items.Count);
					foreach (KeyValuePair<K, CacheEntryBase<K, T>> keyValuePair in this.items)
					{
						list.Add(keyValuePair.Key);
					}
					result = list;
				}
				finally
				{
					if (flag || this.readerWriterLock.IsReadLockHeld)
					{
						this.readerWriterLock.ExitReadLock();
					}
				}
				return result;
			}
		}

		internal List<T> Values
		{
			get
			{
				bool flag = false;
				List<T> result;
				try
				{
					flag = this.readerWriterLock.TryEnterReadLock(-1);
					List<T> list = new List<T>(this.items.Count);
					foreach (CacheEntryBase<K, T> cacheEntryBase in this.items.Values)
					{
						list.Add(cacheEntryBase.Value);
					}
					result = list;
				}
				finally
				{
					if (flag || this.readerWriterLock.IsReadLockHeld)
					{
						this.readerWriterLock.ExitReadLock();
					}
				}
				return result;
			}
		}

		internal Func<bool> WorkerThreadTestHookDelegate { get; set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal T Remove(K key)
		{
			CacheEntryBase<K, T> cacheEntryBase = null;
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				cacheEntryBase = this.InternalRemoveItem(key);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			if (cacheEntryBase != null)
			{
				this.FireRemoveCallbackAsync(cacheEntryBase, RemoveReason.Removed);
				return cacheEntryBase.Value;
			}
			return default(T);
		}

		internal bool Contains(K key)
		{
			bool flag = false;
			bool result;
			try
			{
				flag = this.readerWriterLock.TryEnterReadLock(-1);
				result = this.items.ContainsKey(key);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsReadLockHeld)
				{
					this.readerWriterLock.ExitReadLock();
				}
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
			this.RecreateThreadIfNecessary();
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				CacheEntryBase<K, T> cacheEntryBase;
				if (this.items.TryGetValue(key, out cacheEntryBase))
				{
					cacheEntryBase.OnTouch();
					value = cacheEntryBase.Value;
					return true;
				}
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			value = default(T);
			return false;
		}

		internal bool TryAddAbsolute(K key, T value, TimeSpan expiration)
		{
			CacheEntryBase<K, T> entry = new CacheEntryFixed<K, T>(key, value, expiration);
			return this.TryInternalAddOuter(entry);
		}

		internal bool TryAddAbsolute(K key, T value, DateTime absoluteExpiration)
		{
			CacheEntryBase<K, T> entry = new CacheEntryFixed<K, T>(key, value, (absoluteExpiration == DateTime.MaxValue) ? TimeSpan.MaxValue : (absoluteExpiration - DateTime.UtcNow));
			return this.TryInternalAddOuter(entry);
		}

		internal bool TryAddSliding(K key, T value, TimeSpan slidingExpiration)
		{
			CacheEntryBase<K, T> entry = new CacheEntrySliding<K, T>(key, value, slidingExpiration);
			return this.TryInternalAddOuter(entry);
		}

		internal bool TryAddLimitedSliding(K key, T value, TimeSpan absoluteLiveTime, TimeSpan slidingLiveTime)
		{
			CacheEntryBase<K, T> entry = new CacheEntryLimitedSliding<K, T>(key, value, slidingLiveTime, absoluteLiveTime);
			return this.TryInternalAddOuter(entry);
		}

		internal bool TryInsertAbsolute(K key, T value, TimeSpan absoluteLiveTime)
		{
			return this.TryInternalInsertOuter(new CacheEntryFixed<K, T>(key, value, absoluteLiveTime));
		}

		internal bool TryInsertSliding(K key, T value, TimeSpan slidingLiveTime)
		{
			return this.TryInternalInsertOuter(new CacheEntrySliding<K, T>(key, value, slidingLiveTime));
		}

		internal bool TryInsertLimitedSliding(K key, T value, TimeSpan absoluteLiveTime, TimeSpan slidingLiveTime)
		{
			return this.TryInternalInsertOuter(new CacheEntryLimitedSliding<K, T>(key, value, slidingLiveTime, absoluteLiveTime));
		}

		internal void Clear()
		{
			List<CacheEntryBase<K, T>> entries = null;
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				this.InternalClear(false, out entries);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			this.FireRemoveCallbackAsync(entries, RemoveReason.Cleanup);
		}

		private void InternalClear(bool fromDispose, out List<CacheEntryBase<K, T>> callbackList)
		{
			callbackList = new List<CacheEntryBase<K, T>>(this.items.Count);
			if (!fromDispose || this.callbackOnDispose)
			{
				foreach (KeyValuePair<K, CacheEntryBase<K, T>> keyValuePair in this.items)
				{
					callbackList.Add(keyValuePair.Value);
				}
			}
			this.itemsByExpiration.Clear();
			this.items.Clear();
			this.nextExpirationDate = DateTime.MaxValue;
			if (!fromDispose)
			{
				this.TriggerModifyEvent();
			}
		}

		private void TriggerModifyEvent()
		{
			if (!this.disposed)
			{
				this.RecreateThreadIfNecessary();
				this.collectionModifyEvent.Set();
			}
		}

		private void WorkerThreadProc()
		{
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				this.collectionModifyEvent,
				this.abortEvent
			};
			bool flag = true;
			while (flag)
			{
				try
				{
					bool flag2 = false;
					TimeSpan timeSpan = TimeSpan.Zero;
					bool flag3 = false;
					try
					{
						flag3 = this.readerWriterLock.TryEnterReadLock(-1);
						if (this.itemsByExpiration.Count > 0)
						{
							timeSpan = this.nextExpirationDate - DateTime.UtcNow;
							if (timeSpan.TotalMilliseconds > 2147483647.0)
							{
								timeSpan = TimeSpan.FromMilliseconds(2147483647.0);
							}
							flag2 = (timeSpan <= TimeSpan.Zero);
						}
					}
					finally
					{
						if (flag3 || this.readerWriterLock.IsReadLockHeld)
						{
							this.readerWriterLock.ExitReadLock();
						}
					}
					if (flag2)
					{
						this.InternalRemoveItemsByExpiration(DateTime.UtcNow, RemoveReason.Expired);
						if (this.abortEvent.WaitOne(TimeSpan.Zero))
						{
							flag = false;
						}
					}
					else
					{
						if (timeSpan != TimeSpan.Zero && timeSpan < ExactTimeoutCache<K, T>.MinimumWaitInterval)
						{
							timeSpan = ExactTimeoutCache<K, T>.MinimumWaitInterval;
						}
						int num = (timeSpan > TimeSpan.Zero) ? WaitHandle.WaitAny(waitHandles, timeSpan) : WaitHandle.WaitAny(waitHandles);
						int num2 = num;
						switch (num2)
						{
						case 0:
							break;
						case 1:
							flag = false;
							break;
						default:
							if (num2 == 258)
							{
								this.InternalRemoveItemsByExpiration(DateTime.UtcNow, RemoveReason.Expired);
							}
							break;
						}
					}
					if (flag && this.WorkerThreadTestHookDelegate != null)
					{
						flag = this.WorkerThreadTestHookDelegate();
					}
				}
				catch (Exception e)
				{
					if (this.unhandledExceptionDelegate == null)
					{
						throw;
					}
					this.unhandledExceptionDelegate(e);
				}
			}
		}

		private CacheEntryBase<K, T> AddEntryToChain(CacheEntryBase<K, T> chain, CacheEntryBase<K, T> entryToAdd)
		{
			if (chain == null)
			{
				return entryToAdd;
			}
			CacheEntryBase<K, T> next = chain.Next;
			if (next != null)
			{
				next.Previous = entryToAdd;
				entryToAdd.Next = next;
			}
			chain.Next = entryToAdd;
			entryToAdd.Previous = chain;
			return chain;
		}

		private void InternalRemoveItemsByExpiration(DateTime expiration, RemoveReason reason)
		{
			List<CacheEntryBase<K, T>> entries = null;
			List<CacheEntryBase<K, T>> entries2 = null;
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				this.InternalRemoveItemsByExpiration(expiration, reason, out entries, out entries2);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			this.FireRemoveCallbackAsync(entries, reason);
			this.FireShouldRemoveItemsAsync(entries2, reason);
		}

		private void InternalRemoveItemsByExpiration(DateTime expiration, RemoveReason reason, out List<CacheEntryBase<K, T>> removeCallbacks, out List<CacheEntryBase<K, T>> shouldRemoveCallbacks)
		{
			CacheEntryBase<K, T> cacheEntryBase = null;
			removeCallbacks = null;
			shouldRemoveCallbacks = null;
			while (this.itemsByExpiration.Count > 0 && this.nextExpirationDate <= expiration)
			{
				try
				{
					cacheEntryBase = this.itemsByExpiration[this.nextExpirationDate];
				}
				catch (KeyNotFoundException)
				{
					this.UpdateNextExpirationTime();
					continue;
				}
				this.itemsByExpiration.Remove(this.nextExpirationDate);
				this.UpdateNextExpirationTime();
				while (cacheEntryBase != null)
				{
					CacheEntryBase<K, T> next = cacheEntryBase.Next;
					cacheEntryBase.Previous = null;
					cacheEntryBase.Next = null;
					bool flag = false;
					if (reason == RemoveReason.Expired && !cacheEntryBase.OnBeforeExpire())
					{
						this.InternalAddToSortedList(cacheEntryBase);
						flag = true;
					}
					if (!flag)
					{
						if (reason != RemoveReason.Expired || this.shouldRemoveDelegate == null)
						{
							this.items.Remove(cacheEntryBase.Key);
							if (this.removeItemDelegate != null)
							{
								if (removeCallbacks == null)
								{
									removeCallbacks = new List<CacheEntryBase<K, T>>();
								}
								removeCallbacks.Add(cacheEntryBase);
							}
						}
						else
						{
							if (shouldRemoveCallbacks == null)
							{
								shouldRemoveCallbacks = new List<CacheEntryBase<K, T>>();
							}
							cacheEntryBase.InShouldRemoveCycle = true;
							shouldRemoveCallbacks.Add(cacheEntryBase);
						}
					}
					cacheEntryBase = next;
				}
			}
			this.TriggerModifyEvent();
		}

		private CacheEntryBase<K, T> InternalRemoveItem(K key)
		{
			CacheEntryBase<K, T> cacheEntryBase;
			if (!this.items.TryGetValue(key, out cacheEntryBase))
			{
				return null;
			}
			bool flag = this.InternalRemoveFromSortedList(cacheEntryBase);
			this.items.Remove(cacheEntryBase.Key);
			if (flag)
			{
				this.TriggerModifyEvent();
			}
			return cacheEntryBase;
		}

		private void RemoveCallbackWorker(object state)
		{
			ExactTimeoutCache<K, T>.EntryAndReason entryAndReason = state as ExactTimeoutCache<K, T>.EntryAndReason;
			this.RemoveCallbackSingleEntry(entryAndReason.Entry, entryAndReason.Reason);
		}

		private void RemoveCallbackWorkerArray(object state)
		{
			ExactTimeoutCache<K, T>.EntryListAndReason entryListAndReason = state as ExactTimeoutCache<K, T>.EntryListAndReason;
			List<CacheEntryBase<K, T>> entryList = entryListAndReason.EntryList;
			foreach (CacheEntryBase<K, T> cacheEntry in entryList)
			{
				this.RemoveCallbackSingleEntry(cacheEntry, entryListAndReason.Reason);
			}
		}

		private void RemoveCallbackSingleEntry(CacheEntryBase<K, T> cacheEntry, RemoveReason reason)
		{
			if (this.removeItemDelegate != null)
			{
				this.removeItemDelegate(cacheEntry.Key, cacheEntry.Value, reason);
			}
		}

		private bool TryInternalAdd(CacheEntryBase<K, T> entry, out CacheEntryBase<K, T> singleItemRemoval, out List<CacheEntryBase<K, T>> removeCallbacks, out List<CacheEntryBase<K, T>> shouldRemoveCallbacks)
		{
			removeCallbacks = null;
			shouldRemoveCallbacks = null;
			singleItemRemoval = null;
			bool flag = false;
			if (this.items.Count >= this.cacheSizeLimit)
			{
				if (this.cacheFullBehavior != CacheFullBehavior.ExpireExisting)
				{
					return false;
				}
				this.PreemptiveExpire(out singleItemRemoval, out removeCallbacks, out shouldRemoveCallbacks);
			}
			if (this.items.ContainsKey(entry.Key))
			{
				throw new DuplicateKeyException();
			}
			this.items.Add(entry.Key, entry);
			if (entry.ExpirationUtc < DateTime.MaxValue)
			{
				flag = this.InternalAddToSortedList(entry);
			}
			if (flag)
			{
				this.TriggerModifyEvent();
			}
			return true;
		}

		private bool TryInternalInsertOuter(CacheEntryBase<K, T> entry)
		{
			bool result = false;
			CacheEntryBase<K, T> entry2 = null;
			CacheEntryBase<K, T> entry3 = null;
			List<CacheEntryBase<K, T>> entries = null;
			List<CacheEntryBase<K, T>> entries2 = null;
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				entry2 = this.InternalRemoveItem(entry.Key);
				result = this.TryInternalAdd(entry, out entry3, out entries, out entries2);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			this.FireRemoveCallbackAsync(entry2, RemoveReason.Removed);
			this.FireRemoveCallbackAsync(entry3, RemoveReason.Removed);
			this.FireRemoveCallbackAsync(entries, RemoveReason.PreemptivelyExpired);
			this.FireShouldRemoveItemsAsync(entries2, RemoveReason.PreemptivelyExpired);
			return result;
		}

		private bool TryInternalAddOuter(CacheEntryBase<K, T> entry)
		{
			bool result = false;
			CacheEntryBase<K, T> entry2 = null;
			List<CacheEntryBase<K, T>> entries = null;
			List<CacheEntryBase<K, T>> entries2 = null;
			bool flag = false;
			try
			{
				flag = this.readerWriterLock.TryEnterWriteLock(-1);
				result = this.TryInternalAdd(entry, out entry2, out entries, out entries2);
			}
			finally
			{
				if (flag || this.readerWriterLock.IsWriteLockHeld)
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			this.FireRemoveCallbackAsync(entry2, RemoveReason.PreemptivelyExpired);
			this.FireRemoveCallbackAsync(entries, RemoveReason.PreemptivelyExpired);
			this.FireShouldRemoveItemsAsync(entries2, RemoveReason.PreemptivelyExpired);
			return result;
		}

		private void PreemptiveExpire(out CacheEntryBase<K, T> singleRemovedItem, out List<CacheEntryBase<K, T>> removeCallbacks, out List<CacheEntryBase<K, T>> shouldRemoveCallbacks)
		{
			removeCallbacks = null;
			shouldRemoveCallbacks = null;
			singleRemovedItem = null;
			if (this.itemsByExpiration.Count == 0)
			{
				using (Dictionary<K, CacheEntryBase<K, T>>.Enumerator enumerator = this.items.GetEnumerator())
				{
					enumerator.MoveNext();
					KeyValuePair<K, CacheEntryBase<K, T>> keyValuePair = enumerator.Current;
					singleRemovedItem = this.InternalRemoveItem(keyValuePair.Key);
					return;
				}
			}
			DateTime expiration = (DateTime.UtcNow > this.nextExpirationDate) ? DateTime.UtcNow : this.nextExpirationDate;
			this.InternalRemoveItemsByExpiration(expiration, RemoveReason.PreemptivelyExpired, out removeCallbacks, out shouldRemoveCallbacks);
		}

		private void UpdateNextExpirationTime()
		{
			if (this.itemsByExpiration.Count == 0)
			{
				this.nextExpirationDate = DateTime.MaxValue;
				return;
			}
			using (SortedDictionary<DateTime, CacheEntryBase<K, T>>.Enumerator enumerator = this.itemsByExpiration.GetEnumerator())
			{
				enumerator.MoveNext();
				KeyValuePair<DateTime, CacheEntryBase<K, T>> keyValuePair = enumerator.Current;
				this.nextExpirationDate = keyValuePair.Key;
			}
		}

		private bool InternalAddToSortedList(CacheEntryBase<K, T> entry)
		{
			CacheEntryBase<K, T> chain;
			if (!this.itemsByExpiration.TryGetValue(entry.ExpirationUtc, out chain))
			{
				bool flag = this.itemsByExpiration.Count == 0 || this.nextExpirationDate > entry.ExpirationUtc;
				this.itemsByExpiration.Add(entry.ExpirationUtc, entry);
				if (flag)
				{
					this.nextExpirationDate = entry.ExpirationUtc;
				}
				return flag;
			}
			this.AddEntryToChain(chain, entry);
			return false;
		}

		private bool InternalRemoveFromSortedList(CacheEntryBase<K, T> entryToRemove)
		{
			if (this.itemsByExpiration.Count == 0)
			{
				return false;
			}
			CacheEntryBase<K, T> previous = entryToRemove.Previous;
			CacheEntryBase<K, T> next = entryToRemove.Next;
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
			CacheEntryBase<K, T> cacheEntryBase;
			if (this.itemsByExpiration.TryGetValue(entryToRemove.ExpirationUtc, out cacheEntryBase) && cacheEntryBase == entryToRemove)
			{
				if (next == null)
				{
					bool result = this.nextExpirationDate == entryToRemove.ExpirationUtc;
					this.itemsByExpiration.Remove(entryToRemove.ExpirationUtc);
					this.UpdateNextExpirationTime();
					return result;
				}
				this.itemsByExpiration[cacheEntryBase.ExpirationUtc] = next;
			}
			return false;
		}

		private void CreateWorkerThread()
		{
			using (ActivityContext.SuppressThreadScope())
			{
				this.workerThread = new Thread(new ThreadStart(this.WorkerThreadProc));
				this.workerThread.IsBackground = true;
				this.workerThread.Start();
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing && !this.disposed)
			{
				this.abortEvent.Set();
				if (!this.workerThread.Join(TimeSpan.FromMinutes(1.0)))
				{
					this.workerThread.Abort();
				}
				List<CacheEntryBase<K, T>> entries = null;
				bool flag = false;
				try
				{
					flag = this.readerWriterLock.TryEnterWriteLock(-1);
					if (!this.disposed)
					{
						this.InternalClear(true, out entries);
						this.disposed = true;
						this.abortEvent.Close();
						this.abortEvent = null;
						this.collectionModifyEvent.Close();
						this.collectionModifyEvent = null;
					}
				}
				finally
				{
					if (flag || this.readerWriterLock.IsWriteLockHeld)
					{
						this.readerWriterLock.ExitWriteLock();
						if (this.readerWriterLock != null)
						{
							this.readerWriterLock.Dispose();
							this.readerWriterLock = null;
						}
					}
				}
				this.FireRemoveCallbackAsync(entries, RemoveReason.Cleanup);
			}
		}

		private void RecreateThreadIfNecessary()
		{
			if (!this.disposed && ((this.workerThread.ThreadState & ThreadState.Aborted) == ThreadState.Aborted || (this.workerThread.ThreadState & ThreadState.Stopped) == ThreadState.Stopped))
			{
				bool flag = false;
				try
				{
					Monitor.TryEnter(this.threadCreateCritSect, 0, ref flag);
					if (flag && !this.disposed && ((this.workerThread.ThreadState & ThreadState.Aborted) == ThreadState.Aborted || (this.workerThread.ThreadState & ThreadState.Stopped) == ThreadState.Stopped))
					{
						this.CreateWorkerThread();
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this.threadCreateCritSect);
					}
				}
			}
		}

		private void FireShouldRemoveItemsAsync(List<CacheEntryBase<K, T>> entries, RemoveReason reason)
		{
			if (entries == null || entries.Count == 0)
			{
				return;
			}
			using (ActivityContext.SuppressThreadScope())
			{
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					List<CacheEntryBase<K, T>> list = null;
					bool flag = false;
					foreach (CacheEntryBase<K, T> cacheEntryBase in entries)
					{
						if (this.shouldRemoveDelegate(cacheEntryBase.Key, cacheEntryBase.Value))
						{
							if (list == null)
							{
								list = new List<CacheEntryBase<K, T>>();
							}
							list.Add(cacheEntryBase);
						}
						else
						{
							cacheEntryBase.OnForceExtend();
							cacheEntryBase.InShouldRemoveCycle = false;
							bool flag2 = false;
							bool flag3 = false;
							try
							{
								flag3 = this.readerWriterLock.TryEnterWriteLock(-1);
								flag2 = this.InternalAddToSortedList(cacheEntryBase);
							}
							finally
							{
								if (flag3 || this.readerWriterLock.IsWriteLockHeld)
								{
									this.readerWriterLock.ExitWriteLock();
								}
							}
							flag = (flag2 || flag);
						}
					}
					if (list != null)
					{
						bool flag4 = false;
						try
						{
							flag4 = this.readerWriterLock.TryEnterWriteLock(-1);
							foreach (CacheEntryBase<K, T> cacheEntryBase2 in list)
							{
								this.items.Remove(cacheEntryBase2.Key);
							}
						}
						finally
						{
							if (flag4 || this.readerWriterLock.IsWriteLockHeld)
							{
								this.readerWriterLock.ExitWriteLock();
							}
						}
					}
					if (flag)
					{
						this.TriggerModifyEvent();
					}
					if (list != null && this.removeItemDelegate != null)
					{
						foreach (CacheEntryBase<K, T> cacheEntry in list)
						{
							this.RemoveCallbackSingleEntry(cacheEntry, reason);
						}
					}
				});
			}
		}

		private void FireRemoveCallbackAsync(CacheEntryBase<K, T> entry, RemoveReason reason)
		{
			if (entry == null)
			{
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveCallbackWorker), new ExactTimeoutCache<K, T>.EntryAndReason(entry, reason));
		}

		private void FireRemoveCallbackAsync(List<CacheEntryBase<K, T>> entries, RemoveReason reason)
		{
			if (entries == null || entries.Count == 0)
			{
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveCallbackWorkerArray), new ExactTimeoutCache<K, T>.EntryListAndReason(entries, reason));
		}

		private const int CollectionModifiedSignaled = 0;

		private const int AbortSignaled = 1;

		private static readonly TimeSpan MinimumWaitInterval = TimeSpan.FromMilliseconds(100.0);

		private Dictionary<K, CacheEntryBase<K, T>> items = new Dictionary<K, CacheEntryBase<K, T>>();

		private SortedDictionary<DateTime, CacheEntryBase<K, T>> itemsByExpiration = new SortedDictionary<DateTime, CacheEntryBase<K, T>>();

		private ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		private readonly ShouldRemoveDelegate<K, T> shouldRemoveDelegate;

		private readonly RemoveItemDelegate<K, T> removeItemDelegate;

		private bool disposed;

		private int cacheSizeLimit;

		private bool callbackOnDispose;

		private AutoResetEvent collectionModifyEvent = new AutoResetEvent(false);

		private AutoResetEvent abortEvent = new AutoResetEvent(false);

		private UnhandledExceptionDelegate unhandledExceptionDelegate;

		private CacheFullBehavior cacheFullBehavior;

		private DateTime nextExpirationDate = DateTime.MaxValue;

		private Thread workerThread;

		private object threadCreateCritSect = new object();

		private class EntryAndReason
		{
			public EntryAndReason(CacheEntryBase<K, T> entry, RemoveReason reason)
			{
				this.Entry = entry;
				this.Reason = reason;
			}

			public CacheEntryBase<K, T> Entry { get; private set; }

			public RemoveReason Reason { get; private set; }
		}

		private class EntryListAndReason
		{
			public EntryListAndReason(List<CacheEntryBase<K, T>> entryList, RemoveReason reason)
			{
				this.EntryList = entryList;
				this.Reason = reason;
			}

			public List<CacheEntryBase<K, T>> EntryList { get; private set; }

			public RemoveReason Reason { get; private set; }
		}
	}
}
