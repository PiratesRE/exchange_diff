using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class TimeoutCache<K, T> : IDisposable
	{
		internal TimeoutCache(int numberOfBuckets, int maxSizeForBuckets, bool shouldCallbackOnDispose, PreprocessKeyDelegate<K> handlePreprocessKey, ShouldRemoveDelegate<K, T> shouldRemoveDelegate, HandleBeforeAdd<K, T> handleBeforeAddDelegate)
		{
			if (numberOfBuckets < 1 || numberOfBuckets > 20)
			{
				throw new ArgumentException(string.Format("numberOfBuckets must be between {0}-{1} inclusive.", 1, 20), "numberOfBuckets");
			}
			if (maxSizeForBuckets < 1)
			{
				throw new ArgumentException(string.Format("maxSizeForBuckets must be {0} or greater.", 1));
			}
			this.handlePreprocessKey = handlePreprocessKey;
			this.handleBeforeAddDelegate = handleBeforeAddDelegate;
			this.buckets = new TimeoutCacheBucket<K, T>[numberOfBuckets];
			for (int i = 0; i < this.buckets.Length; i++)
			{
				this.buckets[i] = new TimeoutCacheBucket<K, T>(shouldRemoveDelegate, maxSizeForBuckets, shouldCallbackOnDispose);
			}
		}

		internal TimeoutCache(int numberOfBuckets, int maxSizeForBuckets, bool shouldCallbackOnDispose) : this(numberOfBuckets, maxSizeForBuckets, shouldCallbackOnDispose, null, null, null)
		{
		}

		private K PreProcessKey(K key)
		{
			if (this.handlePreprocessKey != null)
			{
				return this.handlePreprocessKey(key);
			}
			return key;
		}

		internal T Get(K key)
		{
			K key2 = this.PreProcessKey(key);
			return this.GetBucket(key2).Get(key2);
		}

		internal bool TryGetValue(K key, out T value)
		{
			K key2 = this.PreProcessKey(key);
			return this.GetBucket(key2).TryGetValue(key2, out value);
		}

		internal void AddAbsolute(K key, T value, TimeSpan expiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.AddAbsolute(key2, value, expiration, callback);
			}
		}

		internal void AddAbsolute(K key, T value, DateTime absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.AddAbsolute(key2, value, absoluteExpiration, callback);
			}
		}

		internal void AddSliding(K key, T value, TimeSpan slidingExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.AddSliding(key2, value, slidingExpiration, callback);
			}
		}

		internal void AddLimitedSliding(K key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.AddLimitedSliding(key2, value, absoluteExpiration, slidingExpiration, callback);
			}
		}

		internal void InsertAbsolute(K key, T value, TimeSpan expiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.InsertAbsolute(key2, value, expiration, callback);
			}
		}

		internal void InsertAbsolute(K key, T value, DateTime absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.InsertAbsolute(key2, value, absoluteExpiration, callback);
			}
		}

		internal void InsertSliding(K key, T value, TimeSpan slidingExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.InsertSliding(key2, value, slidingExpiration, callback);
			}
		}

		internal void InsertLimitedSliding(K key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, RemoveItemDelegate<K, T> callback)
		{
			K key2 = this.PreProcessKey(key);
			TimeoutCacheBucket<K, T> bucket = this.GetBucket(key2);
			if (this.BeforeAdd(key2, value, bucket))
			{
				bucket.InsertLimitedSliding(key2, value, absoluteExpiration, slidingExpiration, callback);
			}
		}

		internal T Remove(K key)
		{
			K key2 = this.PreProcessKey(key);
			return this.GetBucket(key2).Remove(key2);
		}

		internal bool Contains(K key)
		{
			K key2 = this.PreProcessKey(key);
			return this.GetBucket(key2).Contains(key2);
		}

		internal int Count
		{
			get
			{
				int num = 0;
				foreach (TimeoutCacheBucket<K, T> timeoutCacheBucket in this.buckets)
				{
					num += timeoutCacheBucket.Count;
				}
				return num;
			}
		}

		protected virtual bool BeforeAdd(K key, T value, TimeoutCacheBucket<K, T> bucket)
		{
			return this.handleBeforeAddDelegate == null || this.handleBeforeAddDelegate(key, value, bucket);
		}

		private TimeoutCacheBucket<K, T> GetBucket(K key)
		{
			return this.buckets[this.ComputeIndex(key)];
		}

		private int ComputeIndex(K key)
		{
			return Math.Abs(key.GetHashCode()) % this.buckets.Length;
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (this.buckets != null)
			{
				foreach (TimeoutCacheBucket<K, T> timeoutCacheBucket in this.buckets)
				{
					timeoutCacheBucket.Dispose();
				}
			}
			this.disposed = true;
			GC.SuppressFinalize(this);
		}

		internal void Clear()
		{
			if (this.buckets != null)
			{
				foreach (TimeoutCacheBucket<K, T> timeoutCacheBucket in this.buckets)
				{
					timeoutCacheBucket.Clear();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private const int minBuckets = 1;

		private const int maxBuckets = 20;

		private const int minMaxSize = 1;

		private HandleBeforeAdd<K, T> handleBeforeAddDelegate;

		private PreprocessKeyDelegate<K> handlePreprocessKey;

		private TimeoutCacheBucket<K, T>[] buckets;

		private bool disposed;
	}
}
