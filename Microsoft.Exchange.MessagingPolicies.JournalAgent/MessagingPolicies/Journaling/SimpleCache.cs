using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class SimpleCache<TKey, TValue>
	{
		internal bool TryGetValue(TKey key, out TValue value)
		{
			bool result;
			try
			{
				this.rwLock.AcquireReaderLock(-1);
				this.FlushIfNeeded();
				result = this.cacheDictionary.TryGetValue(key, out value);
			}
			finally
			{
				this.rwLock.ReleaseReaderLock();
			}
			return result;
		}

		internal void AddIgnoringDups(TKey key, TValue value)
		{
			try
			{
				this.rwLock.AcquireWriterLock(-1);
				if (this.cacheDictionary.Count == 0)
				{
					this.nextCacheFlushTime = DateTime.UtcNow.Add(SimpleCache<TKey, TValue>.CacheFlushInterval);
				}
				if (!this.cacheDictionary.ContainsKey(key))
				{
					this.cacheDictionary.Add(key, value);
				}
			}
			finally
			{
				this.rwLock.ReleaseWriterLock();
			}
		}

		private void FlushIfNeeded()
		{
			if (DateTime.UtcNow > this.nextCacheFlushTime)
			{
				this.nextCacheFlushTime = DateTime.UtcNow.Add(SimpleCache<TKey, TValue>.CacheFlushInterval);
				LockCookie lockCookie = this.rwLock.UpgradeToWriterLock(-1);
				this.cacheDictionary.Clear();
				this.rwLock.DowngradeFromWriterLock(ref lockCookie);
			}
		}

		private static readonly TimeSpan CacheFlushInterval = new TimeSpan(0, 10, 0);

		private Dictionary<TKey, TValue> cacheDictionary = new Dictionary<TKey, TValue>();

		private ReaderWriterLock rwLock = new ReaderWriterLock();

		private DateTime nextCacheFlushTime = DateTime.UtcNow;
	}
}
