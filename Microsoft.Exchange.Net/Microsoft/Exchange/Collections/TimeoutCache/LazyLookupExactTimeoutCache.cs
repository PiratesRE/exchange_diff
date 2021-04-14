using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal abstract class LazyLookupExactTimeoutCache<K, T> : DisposeTrackableBase
	{
		protected LazyLookupExactTimeoutCache(int maxCount, bool shouldCallbackOnDispose, TimeSpan absoluteLiveTime, CacheFullBehavior cacheFullBehavior) : this(maxCount, shouldCallbackOnDispose, TimeoutType.Absolute, absoluteLiveTime, absoluteLiveTime, cacheFullBehavior)
		{
		}

		protected LazyLookupExactTimeoutCache(int maxCount, bool shouldCallbackOnDispose, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime, CacheFullBehavior cacheFullBehavior) : this(maxCount, shouldCallbackOnDispose, TimeoutType.Sliding, slidingLiveTime, absoluteLiveTime, cacheFullBehavior)
		{
		}

		private LazyLookupExactTimeoutCache(int maxCount, bool shouldCallbackOnDispose, TimeoutType timeoutType, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime, CacheFullBehavior cacheFullBehavior)
		{
			this.timeoutCache = new ExactTimeoutCache<K, T>(new RemoveItemDelegate<K, T>(this.HandleRemove), new ShouldRemoveDelegate<K, T>(this.HandleShouldRemove), new UnhandledExceptionDelegate(this.HandleThreadException), maxCount, shouldCallbackOnDispose, cacheFullBehavior);
			this.timeoutType = timeoutType;
			this.slidingLiveTime = slidingLiveTime;
			this.absoluteLiveTime = absoluteLiveTime;
		}

		internal virtual void Clear()
		{
			base.CheckDisposed();
			this.timeoutCache.Clear();
		}

		protected virtual void AfterCacheHit(K key, T value)
		{
		}

		protected virtual void BeforeGet(K key)
		{
		}

		protected virtual bool HandleShouldRemove(K key, T value)
		{
			return true;
		}

		protected virtual void HandleThreadException(Exception e)
		{
			if (!(e is ThreadAbortException) && !(e is AppDomainUnloadedException))
			{
				ExWatson.SendReport(e, ReportOptions.ReportTerminateAfterSend, null);
			}
		}

		protected virtual void HandleRemove(K key, T value, RemoveReason reason)
		{
			if (reason != RemoveReason.Removed)
			{
				this.CleanupValue(key, value);
			}
		}

		protected abstract T CreateOnCacheMiss(K key, ref bool shouldAdd);

		protected virtual void CleanupValue(K key, T value)
		{
		}

		private bool TryPerformAdd(K key, T value)
		{
			bool result;
			try
			{
				if (this.timeoutType == TimeoutType.Absolute)
				{
					result = this.timeoutCache.TryAddAbsolute(key, value, this.absoluteLiveTime);
				}
				else
				{
					if (this.absoluteLiveTime == TimeSpan.MaxValue)
					{
						this.timeoutCache.TryAddSliding(key, value, this.slidingLiveTime);
					}
					else
					{
						this.timeoutCache.TryAddLimitedSliding(key, value, this.absoluteLiveTime, this.slidingLiveTime);
					}
					result = true;
				}
			}
			catch (DuplicateKeyException)
			{
				result = false;
			}
			return result;
		}

		internal List<K> Keys
		{
			get
			{
				base.CheckDisposed();
				return this.timeoutCache.Keys;
			}
		}

		internal List<T> Values
		{
			get
			{
				base.CheckDisposed();
				return this.timeoutCache.Values;
			}
		}

		internal T Remove(K key)
		{
			base.CheckDisposed();
			return this.timeoutCache.Remove(key);
		}

		internal bool Contains(K key)
		{
			base.CheckDisposed();
			return this.timeoutCache.Contains(key);
		}

		internal int Count
		{
			get
			{
				base.CheckDisposed();
				return this.timeoutCache.Count;
			}
		}

		internal bool TryAdd(K key, ref T value)
		{
			base.CheckDisposed();
			this.BeforeGet(key);
			T t;
			if (this.timeoutCache.TryGetValue(key, out t))
			{
				this.AfterCacheHit(key, t);
				value = t;
				return false;
			}
			return this.InternalTryAdd(key, ref value);
		}

		internal T Get(K key)
		{
			base.CheckDisposed();
			T t = default(T);
			this.BeforeGet(key);
			if (this.timeoutCache.TryGetValue(key, out t))
			{
				this.AfterCacheHit(key, t);
				return t;
			}
			bool flag = true;
			t = this.CreateOnCacheMiss(key, ref flag);
			if (flag)
			{
				this.InternalTryAdd(key, ref t);
			}
			return t;
		}

		private bool InternalTryAdd(K key, ref T value)
		{
			bool flag = false;
			T value2 = default(T);
			while (!this.TryPerformAdd(key, value))
			{
				T t;
				if (this.timeoutCache.TryGetValue(key, out t))
				{
					flag = true;
					value2 = value;
					value = t;
					break;
				}
			}
			if (flag)
			{
				this.CleanupValue(key, value2);
			}
			return !flag;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.timeoutCache != null)
			{
				this.timeoutCache.Dispose();
				this.timeoutCache = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LazyLookupExactTimeoutCache<K, T>>(this);
		}

		private ExactTimeoutCache<K, T> timeoutCache;

		private readonly TimeoutType timeoutType;

		private readonly TimeSpan slidingLiveTime;

		private readonly TimeSpan absoluteLiveTime;
	}
}
