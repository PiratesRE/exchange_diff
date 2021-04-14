using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class SingleKeyCache<TKey, TCached>
	{
		public SingleKeyCache(EvictionPolicy<TKey> evictionPolicy, ICachePerformanceCounters perfCounters)
		{
			this.keyToData = new Dictionary<TKey, TCached>(evictionPolicy.Capacity);
			this.evictionPolicy = evictionPolicy;
			this.performanceCounters = perfCounters;
		}

		public void Insert(TKey key, TCached value)
		{
			this.Insert(key, value, true);
		}

		public void Insert(TKey key, TCached value, bool shouldEvict)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			using (SingleKeyCache<TKey, TCached>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				if (shouldEvict)
				{
					this.EvictionCheckpoint();
				}
				bool flag = this.keyToData.ContainsKey(key);
				this.keyToData[key] = value;
				if (flag)
				{
					this.evictionPolicy.KeyAccess(key);
				}
				else
				{
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheInserts.Increment();
					}
					this.evictionPolicy.Insert(key);
				}
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
					this.performanceCounters.CacheSize.RawValue = (long)this.keyToData.Count;
				}
				criticalConsistencyBlock.Success();
			}
		}

		public TCached Find(TKey key)
		{
			return this.Find(key, true);
		}

		public TCached Find(TKey key, bool shouldEvict)
		{
			TCached result;
			using (SingleKeyCache<TKey, TCached>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				if (shouldEvict)
				{
					this.EvictionCheckpoint();
				}
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheLookups.Increment();
				}
				TCached tcached = default(TCached);
				if (this.keyToData.TryGetValue(key, out tcached))
				{
					this.evictionPolicy.KeyAccess(key);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
					}
					if (!this.evictionPolicy.ContainsKeyToCleanup(key))
					{
						if (this.performanceCounters != null)
						{
							this.performanceCounters.CacheHits.Increment();
						}
					}
					else
					{
						if (this.performanceCounters != null)
						{
							this.performanceCounters.CacheMisses.Increment();
						}
						tcached = default(TCached);
					}
				}
				else if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheMisses.Increment();
				}
				criticalConsistencyBlock.Success();
				result = tcached;
			}
			return result;
		}

		public void Remove(TKey key)
		{
			this.Remove(key, true);
		}

		public void Remove(TKey key, bool shouldEvict)
		{
			using (SingleKeyCache<TKey, TCached>._CriticalConsistencyBlock criticalConsistencyBlock = this.Critical())
			{
				if (shouldEvict)
				{
					this.EvictionCheckpoint();
				}
				if (this.keyToData.Remove(key))
				{
					this.evictionPolicy.Remove(key);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheRemoves.Increment();
						this.performanceCounters.CacheSize.RawValue = (long)this.keyToData.Count;
						this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
					}
				}
				criticalConsistencyBlock.Success();
			}
		}

		public virtual void Reset()
		{
			this.keyToData.Clear();
			this.evictionPolicy.Reset();
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheExpirationQueueLength.RawValue = 0L;
				this.performanceCounters.CacheSize.RawValue = 0L;
			}
		}

		public virtual void EvictionCheckpoint()
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
					this.keyToData.Remove(key);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheRemoves.Increment();
					}
				}
			}
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheSize.RawValue = (long)this.keyToData.Count;
				this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
			}
		}

		private SingleKeyCache<TKey, TCached>._CriticalConsistencyBlock Critical()
		{
			return new SingleKeyCache<TKey, TCached>._CriticalConsistencyBlock
			{
				Cache = this
			};
		}

		protected Dictionary<TKey, TCached> keyToData;

		protected ICachePerformanceCounters performanceCounters;

		protected EvictionPolicy<TKey> evictionPolicy;

		private struct _CriticalConsistencyBlock : IDisposable
		{
			public void Dispose()
			{
				if (this.Cache != null)
				{
					this.Cache.Reset();
				}
			}

			public void Success()
			{
				this.Cache = null;
			}

			public SingleKeyCache<TKey, TCached> Cache;
		}
	}
}
