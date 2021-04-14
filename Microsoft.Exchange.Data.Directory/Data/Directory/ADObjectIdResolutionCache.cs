using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADObjectIdResolutionCache
	{
		internal static ADObjectIdResolutionCache Default
		{
			get
			{
				return ADObjectIdResolutionCache.defaultInstance;
			}
		}

		internal ADObjectIdResolutionCache(Func<ADObjectId, ADObjectId> resolutionFunc, int capacityLimit)
		{
			this.cache = new ConcurrentDictionary<Guid, ExpiringADObjectIdValue>();
			this.resolutionFunc = resolutionFunc;
			this.capacityLimit = capacityLimit;
		}

		internal int Count
		{
			get
			{
				return this.cache.Count;
			}
		}

		internal ADObjectId GetEntry(ADObjectId obj)
		{
			ExpiringADObjectIdValue expiringADObjectIdValue;
			if (this.cache.TryGetValue(obj.ObjectGuid, out expiringADObjectIdValue) && !expiringADObjectIdValue.Expired)
			{
				return expiringADObjectIdValue.Value;
			}
			ADObjectId adobjectId = this.resolutionFunc(obj);
			this.UpdateEntry(adobjectId);
			return adobjectId;
		}

		internal bool UpdateEntry(ADObjectId obj)
		{
			if (string.IsNullOrEmpty(obj.DistinguishedName))
			{
				return false;
			}
			ExpiringADObjectIdValue expiringValue = new ExpiringADObjectIdValue(obj);
			this.cache.AddOrUpdate(obj.ObjectGuid, expiringValue, (Guid key, ExpiringADObjectIdValue oldValue) => expiringValue);
			this.EvictIfNecessary();
			return true;
		}

		private void EvictIfNecessary()
		{
			if (this.cache.Count <= this.capacityLimit)
			{
				return;
			}
			int num = 2;
			try
			{
				num = Interlocked.CompareExchange(ref this.evictionOwnershipFlag, 1, 0);
				if (num == 0)
				{
					List<Guid> list = this.cache.Keys.ToList<Guid>();
					foreach (Guid key in list)
					{
						ExpiringADObjectIdValue expiringADObjectIdValue;
						if (this.cache.TryGetValue(key, out expiringADObjectIdValue) && expiringADObjectIdValue.Expired)
						{
							this.cache.TryRemove(key, out expiringADObjectIdValue);
						}
					}
					if (this.cache.Count > this.capacityLimit)
					{
						list = this.cache.Keys.ToList<Guid>();
						int num2 = (int)((double)this.capacityLimit * 0.8);
						Random random = new Random();
						foreach (Guid key2 in list)
						{
							if (random.Next(list.Count<Guid>()) > num2)
							{
								ExpiringADObjectIdValue expiringADObjectIdValue2;
								this.cache.TryRemove(key2, out expiringADObjectIdValue2);
							}
						}
					}
				}
			}
			finally
			{
				if (num == 0)
				{
					Interlocked.Exchange(ref this.evictionOwnershipFlag, 0);
				}
			}
		}

		private const int CapacityDefault = 3000;

		private static readonly ADObjectIdResolutionCache defaultInstance = new ADObjectIdResolutionCache(new Func<ADObjectId, ADObjectId>(ADObjectIdResolutionHelper.ResolveADObjectIdWithoutCache), 3000);

		private readonly int capacityLimit;

		private ConcurrentDictionary<Guid, ExpiringADObjectIdValue> cache;

		private int evictionOwnershipFlag;

		private readonly Func<ADObjectId, ADObjectId> resolutionFunc;
	}
}
