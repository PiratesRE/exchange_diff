using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class MultiKeyCache<TCached, TKey>
	{
		public MultiKeyCache(EvictionPolicy<TKey> evictionPolicy, ICachePerformanceCounters perfCounters)
		{
			this.data = new Dictionary<TKey, MultiKeyCache<TCached, TKey>._CacheEntry<TCached, TKey>>(evictionPolicy.Capacity);
			this.secondaryKeys = new Dictionary<TKey, TKey>(evictionPolicy.Capacity);
			this.evictionPolicy = evictionPolicy;
			this.performanceCounters = perfCounters;
		}

		public void Insert(TCached value, TKey primaryKey, TKey lookupKey)
		{
			using (MultiKeyCache<TCached, TKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				this.EvictionCheckpoint();
				if (this.secondaryKeys.ContainsKey(lookupKey))
				{
					TKey tkey = this.secondaryKeys[lookupKey];
					if (!primaryKey.Equals(tkey))
					{
						criticalConsistencyBlock.Success();
					}
					else
					{
						this.data[primaryKey].Value = value;
						this.evictionPolicy.KeyAccess(primaryKey);
						if (this.performanceCounters != null)
						{
							this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
						}
						criticalConsistencyBlock.Success();
					}
				}
				else if (this.data.ContainsKey(primaryKey))
				{
					this.data[primaryKey].Value = value;
					this.data[primaryKey].Keys.Add(lookupKey);
					this.secondaryKeys[lookupKey] = primaryKey;
					this.evictionPolicy.KeyAccess(primaryKey);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
					}
					criticalConsistencyBlock.Success();
				}
				else
				{
					this.data[primaryKey] = new MultiKeyCache<TCached, TKey>._CacheEntry<TCached, TKey>();
					this.data[primaryKey].Value = value;
					this.data[primaryKey].Keys.Add(lookupKey);
					this.secondaryKeys[lookupKey] = primaryKey;
					this.evictionPolicy.Insert(primaryKey);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
						this.performanceCounters.CacheInserts.Increment();
						this.performanceCounters.CacheSize.RawValue = (long)this.data.Count;
					}
					criticalConsistencyBlock.Success();
				}
			}
		}

		public void Remove(TKey key)
		{
			using (MultiKeyCache<TCached, TKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				this.EvictionCheckpoint();
				TKey key2 = default(TKey);
				if (this.secondaryKeys.ContainsKey(key))
				{
					key2 = this.secondaryKeys[key];
				}
				else
				{
					key2 = key;
				}
				if (this.data.ContainsKey(key2))
				{
					foreach (TKey key3 in this.data[key2].Keys)
					{
						this.secondaryKeys.Remove(key3);
					}
					this.data.Remove(key2);
					this.evictionPolicy.Remove(key2);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
						this.performanceCounters.CacheRemoves.Increment();
						this.performanceCounters.CacheSize.RawValue = (long)this.data.Count;
					}
				}
				criticalConsistencyBlock.Success();
			}
		}

		public TCached Find(TKey key)
		{
			TCached result = default(TCached);
			using (MultiKeyCache<TCached, TKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				this.EvictionCheckpoint();
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheLookups.Increment();
				}
				TKey key2 = default(TKey);
				if (this.secondaryKeys.ContainsKey(key))
				{
					key2 = this.secondaryKeys[key];
				}
				else
				{
					key2 = key;
				}
				if (this.data.ContainsKey(key2))
				{
					this.evictionPolicy.KeyAccess(key2);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
					}
					if (!this.evictionPolicy.ContainsKeyToCleanup(key2))
					{
						if (this.performanceCounters != null)
						{
							this.performanceCounters.CacheHits.Increment();
						}
						result = this.data[key2].Value;
					}
					else if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheMisses.Increment();
					}
				}
				else if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheMisses.Increment();
				}
				criticalConsistencyBlock.Success();
			}
			return result;
		}

		public void ResetCache()
		{
			this.data.Clear();
			this.secondaryKeys.Clear();
			this.evictionPolicy.Reset();
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheExpirationQueueLength.RawValue = 0L;
				this.performanceCounters.CacheSize.RawValue = 0L;
			}
		}

		private MultiKeyCache<TCached, TKey>._CriticalConsistencyBlock Critical()
		{
			return new MultiKeyCache<TCached, TKey>._CriticalConsistencyBlock
			{
				Cache = this
			};
		}

		protected void EvictionCheckpoint()
		{
			this.evictionPolicy.EvictionCheckpoint();
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
			}
			if (this.evictionPolicy.CountOfKeysToCleanup > 0)
			{
				foreach (TKey key in this.evictionPolicy.GetKeysToCleanup(true))
				{
					foreach (TKey key2 in this.data[key].Keys)
					{
						this.secondaryKeys.Remove(key2);
					}
					this.data.Remove(key);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheRemoves.Increment();
					}
				}
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheSize.RawValue = (long)this.data.Count;
					this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
				}
			}
		}

		private Dictionary<TKey, MultiKeyCache<TCached, TKey>._CacheEntry<TCached, TKey>> data;

		private Dictionary<TKey, TKey> secondaryKeys;

		private EvictionPolicy<TKey> evictionPolicy;

		private ICachePerformanceCounters performanceCounters;

		private class _CacheEntry<_TCached, _TKey>
		{
			public _CacheEntry()
			{
				this.value = default(_TCached);
				this.keys = new List<_TKey>();
			}

			public _TCached Value
			{
				get
				{
					return this.value;
				}
				set
				{
					this.value = value;
				}
			}

			public List<_TKey> Keys
			{
				get
				{
					return this.keys;
				}
			}

			private _TCached value;

			private List<_TKey> keys;
		}

		private struct _CriticalConsistencyBlock : IDisposable
		{
			public void Dispose()
			{
				if (this.Cache != null)
				{
					this.Cache.ResetCache();
				}
			}

			public void Success()
			{
				this.Cache = null;
			}

			public MultiKeyCache<TCached, TKey> Cache;
		}
	}
}
