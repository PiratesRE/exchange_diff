using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class TypedMultiKeyCache<TCached, TPrimaryKey>
	{
		protected void RegisterKeyDefinition(TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition keyDefinition)
		{
			this.keyDefinitions.Add(keyDefinition);
		}

		public TypedMultiKeyCache(EvictionPolicy<TPrimaryKey> evictionPolicy, ICachePerformanceCounters perfCounters)
		{
			this.primaryKeyToData = new Dictionary<TPrimaryKey, TCached>(evictionPolicy.Capacity);
			this.keyDefinitions = new List<TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition>();
			this.evictionPolicy = evictionPolicy;
			this.performanceCounters = perfCounters;
		}

		protected void Remove(TPrimaryKey primaryKey)
		{
			this.RemoveDataAndSecondaryKeys(primaryKey);
			this.evictionPolicy.Remove(primaryKey);
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheRemoves.Increment();
				this.performanceCounters.CacheSize.RawValue = (long)this.primaryKeyToData.Count;
				this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
			}
		}

		protected void RemoveDataAndSecondaryKeys(TPrimaryKey primaryKey)
		{
			this.primaryKeyToData.Remove(primaryKey);
			foreach (TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition keyDefinition in this.keyDefinitions)
			{
				keyDefinition.Remove(primaryKey);
			}
		}

		protected void Reset()
		{
			this.primaryKeyToData.Clear();
			foreach (TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition keyDefinition in this.keyDefinitions)
			{
				keyDefinition.ResetCalledFromCache();
			}
			this.evictionPolicy.Reset();
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheExpirationQueueLength.RawValue = 0L;
				this.performanceCounters.CacheSize.RawValue = 0L;
			}
		}

		protected TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock Critical()
		{
			return new TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock
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
				foreach (TPrimaryKey primaryKey in this.evictionPolicy.GetKeysToCleanup(true))
				{
					this.RemoveDataAndSecondaryKeys(primaryKey);
					if (this.performanceCounters != null)
					{
						this.performanceCounters.CacheRemoves.Increment();
					}
				}
			}
			if (this.performanceCounters != null)
			{
				this.performanceCounters.CacheSize.RawValue = (long)this.primaryKeyToData.Count;
				this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
			}
		}

		private Dictionary<TPrimaryKey, TCached> primaryKeyToData;

		private List<TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition> keyDefinitions;

		private ICachePerformanceCounters performanceCounters;

		protected EvictionPolicy<TPrimaryKey> evictionPolicy;

		protected interface IKeyDefinition
		{
			void Remove(TPrimaryKey primaryKey);

			void Reset();

			void ResetCalledFromCache();
		}

		public class KeyDefinition<TSecondaryKey> : TypedMultiKeyCache<TCached, TPrimaryKey>.IKeyDefinition
		{
			void TypedMultiKeyCache<!0, !1>.IKeyDefinition.Remove(TPrimaryKey primaryKey)
			{
				List<TSecondaryKey> list;
				if (this.primaryKeyToSecondaryKeys.TryGetValue(primaryKey, out list))
				{
					foreach (TSecondaryKey key in list)
					{
						this.secondaryKeyToPrimaryKey.Remove(key);
					}
					this.primaryKeyToSecondaryKeys.Remove(primaryKey);
				}
			}

			void TypedMultiKeyCache<!0, !1>.IKeyDefinition.Reset()
			{
				this.cache.Reset();
			}

			void TypedMultiKeyCache<!0, !1>.IKeyDefinition.ResetCalledFromCache()
			{
				this.primaryKeyToSecondaryKeys.Clear();
				this.secondaryKeyToPrimaryKey.Clear();
			}

			public KeyDefinition(TypedMultiKeyCache<TCached, TPrimaryKey> cache, int capacity)
			{
				this.secondaryKeyToPrimaryKey = new Dictionary<TSecondaryKey, TPrimaryKey>(capacity);
				this.primaryKeyToSecondaryKeys = new Dictionary<TPrimaryKey, List<TSecondaryKey>>(capacity);
				this.cache = cache;
			}

			public IEnumerable<TSecondaryKey> GetKeys()
			{
				IList<TSecondaryKey> list;
				using (TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.cache.Critical())
				{
					list = new List<TSecondaryKey>(this.primaryKeyToSecondaryKeys.Count);
					foreach (TPrimaryKey key in this.primaryKeyToSecondaryKeys.Keys)
					{
						bool flag = this.primaryKeyToSecondaryKeys[key].Count > 0;
						if (flag)
						{
							list.Add(this.primaryKeyToSecondaryKeys[key][0]);
						}
					}
					criticalConsistencyBlock.Success();
				}
				return list;
			}

			public void Insert(TCached value, TPrimaryKey primaryKey, TSecondaryKey secondaryKey)
			{
				if (primaryKey == null)
				{
					throw new ArgumentNullException("primaryKey");
				}
				if (secondaryKey == null)
				{
					throw new ArgumentNullException("secondary key");
				}
				using (TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.cache.Critical())
				{
					this.cache.EvictionCheckpoint();
					TPrimaryKey tprimaryKey;
					if (this.secondaryKeyToPrimaryKey.TryGetValue(secondaryKey, out tprimaryKey))
					{
						if (!primaryKey.Equals(tprimaryKey))
						{
							criticalConsistencyBlock.Success();
							return;
						}
						this.cache.primaryKeyToData[primaryKey] = value;
						this.cache.evictionPolicy.KeyAccess(primaryKey);
						if (this.cache.performanceCounters != null)
						{
							this.cache.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.cache.evictionPolicy.CountOfKeysToCleanup;
						}
					}
					else
					{
						this.secondaryKeyToPrimaryKey[secondaryKey] = primaryKey;
						if (!this.primaryKeyToSecondaryKeys.ContainsKey(primaryKey))
						{
							this.primaryKeyToSecondaryKeys[primaryKey] = new List<TSecondaryKey>(1);
						}
						this.primaryKeyToSecondaryKeys[primaryKey].Add(secondaryKey);
						bool flag;
						if (this.cache.primaryKeyToData.ContainsKey(primaryKey))
						{
							this.cache.evictionPolicy.KeyAccess(primaryKey);
							flag = false;
						}
						else
						{
							this.cache.evictionPolicy.Insert(primaryKey);
							flag = true;
						}
						this.cache.primaryKeyToData[primaryKey] = value;
						if (this.cache.performanceCounters != null && flag)
						{
							this.cache.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.cache.evictionPolicy.CountOfKeysToCleanup;
							this.cache.performanceCounters.CacheInserts.Increment();
							this.cache.performanceCounters.CacheSize.RawValue = (long)this.cache.primaryKeyToData.Count;
						}
					}
					criticalConsistencyBlock.Success();
				}
			}

			public TCached Find(TSecondaryKey secondaryKey)
			{
				TCached result;
				using (TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.cache.Critical())
				{
					this.cache.EvictionCheckpoint();
					if (this.cache.performanceCounters != null)
					{
						this.cache.performanceCounters.CacheLookups.Increment();
					}
					TCached tcached = default(TCached);
					TPrimaryKey key;
					if (this.secondaryKeyToPrimaryKey.TryGetValue(secondaryKey, out key))
					{
						this.cache.evictionPolicy.KeyAccess(key);
						if (this.cache.performanceCounters != null)
						{
							this.cache.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.cache.evictionPolicy.CountOfKeysToCleanup;
						}
						if (!this.cache.evictionPolicy.ContainsKeyToCleanup(key))
						{
							tcached = this.cache.primaryKeyToData[key];
							if (this.cache.performanceCounters != null)
							{
								this.cache.performanceCounters.CacheHits.Increment();
							}
						}
						else if (this.cache.performanceCounters != null)
						{
							this.cache.performanceCounters.CacheMisses.Increment();
						}
					}
					else if (this.cache.performanceCounters != null)
					{
						this.cache.performanceCounters.CacheMisses.Increment();
					}
					criticalConsistencyBlock.Success();
					result = tcached;
				}
				return result;
			}

			public void Remove(TSecondaryKey secondaryKey)
			{
				using (TypedMultiKeyCache<TCached, TPrimaryKey>._CriticalConsistencyBlock criticalConsistencyBlock = this.cache.Critical())
				{
					this.cache.EvictionCheckpoint();
					TPrimaryKey primaryKey;
					if (this.secondaryKeyToPrimaryKey.TryGetValue(secondaryKey, out primaryKey))
					{
						this.cache.Remove(primaryKey);
					}
					criticalConsistencyBlock.Success();
				}
			}

			private Dictionary<TSecondaryKey, TPrimaryKey> secondaryKeyToPrimaryKey;

			private Dictionary<TPrimaryKey, List<TSecondaryKey>> primaryKeyToSecondaryKeys;

			private TypedMultiKeyCache<TCached, TPrimaryKey> cache;
		}

		protected struct _CriticalConsistencyBlock : IDisposable
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

			public TypedMultiKeyCache<TCached, TPrimaryKey> Cache;
		}
	}
}
