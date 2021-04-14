using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Collections
{
	internal sealed class MruDictionaryListCache<TKey, TValue> : IDisposable
	{
		public MruDictionaryListCache(int capacity, int expireTimeInMinutes, Func<TValue, TKey> getKeyFromValue)
		{
			this.getKeyFromValue = getKeyFromValue;
			this.lookup = new Dictionary<TKey, IList<Guid>>();
			this.cache = new MruDictionaryCache<Guid, TValue>(capacity, capacity, expireTimeInMinutes, new Action<Guid, TValue>(this.OnEntryExpired));
			this.cache.OnExpirationStart = delegate()
			{
				Monitor.Enter(this.lockObject);
			};
			this.cache.OnExpirationComplete = delegate()
			{
				Monitor.Exit(this.lockObject);
			};
		}

		public bool TryGetAndRemoveValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			lock (this.lockObject)
			{
				IList<Guid> list;
				if (this.lookup.TryGetValue(key, out list))
				{
					Guid token = list[0];
					list.RemoveAt(0);
					if (list.Count == 0)
					{
						this.lookup.Remove(key);
					}
					return this.cache.TryGetAndRemoveValue(token, out value);
				}
			}
			value = default(TValue);
			return false;
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue data = default(TValue);
			try
			{
				lock (this.lockObject)
				{
					Guid guid = Guid.NewGuid();
					IList<Guid> list;
					if (!this.lookup.TryGetValue(key, out list))
					{
						list = new List<Guid>();
						this.lookup.Add(key, list);
					}
					list.Insert(0, guid);
					data = this.cache.AddAndReturnOriginalData(guid, value);
				}
			}
			finally
			{
				MruDictionaryCache<Guid, TValue>.DisposeData(data);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					lock (this.lockObject)
					{
						this.cache.Dispose();
						this.cache = null;
						this.lookup.Clear();
						this.lookup = null;
					}
				}
				this.disposed = true;
			}
		}

		private void OnEntryExpired(Guid cacheId, TValue value)
		{
			TKey tkey = this.getKeyFromValue(value);
			IList<Guid> list;
			if (tkey != null && this.lookup.TryGetValue(tkey, out list))
			{
				list.Remove(cacheId);
				if (list.Count == 0)
				{
					this.lookup.Remove(tkey);
				}
			}
		}

		private MruDictionaryCache<Guid, TValue> cache;

		private Dictionary<TKey, IList<Guid>> lookup;

		private Func<TValue, TKey> getKeyFromValue;

		private bool disposed;

		private object lockObject = new object();
	}
}
