using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal abstract class LazyLookupTimeoutCache<K, T> : DisposeTrackableBase
	{
		protected LazyLookupTimeoutCache(int buckets, int maxBucketSize, bool shouldCallbackOnDispose, TimeSpan absoluteLiveTime) : this(buckets, maxBucketSize, shouldCallbackOnDispose, TimeoutType.Absolute, absoluteLiveTime, absoluteLiveTime)
		{
		}

		protected LazyLookupTimeoutCache(int buckets, int maxBucketSize, bool shouldCallbackOnDispose, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime) : this(buckets, maxBucketSize, shouldCallbackOnDispose, TimeoutType.Sliding, slidingLiveTime, absoluteLiveTime)
		{
		}

		private LazyLookupTimeoutCache(int buckets, int maxBucketSize, bool shouldCallbackOnDispose, TimeoutType timeoutType, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime)
		{
			this.timeoutCache = new TimeoutCache<K, T>(buckets, maxBucketSize, shouldCallbackOnDispose, new PreprocessKeyDelegate<K>(this.PreprocessKey), new ShouldRemoveDelegate<K, T>(this.HandleShouldRemove), new HandleBeforeAdd<K, T>(this.HandleBeforeAdd));
			this.timeoutType = timeoutType;
			this.slidingLiveTime = slidingLiveTime;
			this.absoluteLiveTime = absoluteLiveTime;
			this.handleRemoveDelegate = new RemoveItemDelegate<K, T>(this.HandleRemove);
		}

		internal virtual void Clear()
		{
			this.timeoutCache.Clear();
		}

		protected virtual bool HandleShouldRemove(K key, T value)
		{
			return true;
		}

		protected virtual bool HandleBeforeAdd(K key, T value, TimeoutCacheBucket<K, T> bucket)
		{
			return true;
		}

		protected virtual K PreprocessKey(K key)
		{
			return key;
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

		protected virtual bool TryPerformAdd(K key, T value)
		{
			bool result;
			try
			{
				if (this.timeoutType == TimeoutType.Absolute)
				{
					this.timeoutCache.AddAbsolute(key, value, this.absoluteLiveTime, this.handleRemoveDelegate);
				}
				else if (this.absoluteLiveTime == TimeSpan.MaxValue)
				{
					this.timeoutCache.AddSliding(key, value, this.slidingLiveTime, this.handleRemoveDelegate);
				}
				else
				{
					this.timeoutCache.AddLimitedSliding(key, value, this.slidingLiveTime, this.absoluteLiveTime, this.handleRemoveDelegate);
				}
				result = true;
			}
			catch (DuplicateKeyException)
			{
				result = false;
			}
			return result;
		}

		protected virtual void AfterCacheHit(K key, T value)
		{
		}

		internal T Remove(K key)
		{
			return this.timeoutCache.Remove(key);
		}

		internal bool Contains(K key)
		{
			return this.timeoutCache.Contains(key);
		}

		internal int Count
		{
			get
			{
				return this.timeoutCache.Count;
			}
		}

		internal T Get(K key)
		{
			T t = default(T);
			key = this.PreprocessKey(key);
			if (this.timeoutCache.TryGetValue(key, out t))
			{
				this.AfterCacheHit(key, t);
				return t;
			}
			bool flag = true;
			t = this.CreateOnCacheMiss(key, ref flag);
			if (flag)
			{
				bool flag2 = false;
				T value = default(T);
				while (!this.TryPerformAdd(key, t))
				{
					T t2;
					if (this.timeoutCache.TryGetValue(key, out t2))
					{
						flag2 = true;
						value = t;
						t = t2;
						break;
					}
				}
				if (flag2)
				{
					this.CleanupValue(key, value);
				}
			}
			return t;
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
			return DisposeTracker.Get<LazyLookupTimeoutCache<K, T>>(this);
		}

		private TimeoutCache<K, T> timeoutCache;

		private readonly TimeoutType timeoutType;

		private readonly TimeSpan slidingLiveTime;

		private readonly TimeSpan absoluteLiveTime;

		private RemoveItemDelegate<K, T> handleRemoveDelegate;
	}
}
