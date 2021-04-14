using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class OabCache
	{
		public OabCache()
		{
			Func<OfflineAddressBookCacheKey, OfflineAddressBookCacheEntry> loadItem = delegate(OfflineAddressBookCacheKey cacheKey)
			{
				switch (cacheKey.FilterType)
				{
				case FilterType.OabId:
					return OabCache.FindOab(null, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, cacheKey.Key), this.GetSession(cacheKey.OrganizationId));
				case FilterType.ConfigUnitId:
					return OabCache.FindOab(cacheKey.Key, new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "msExchOAB"),
						new ComparisonFilter(ComparisonOperator.Equal, OfflineAddressBookSchema.IsDefault, true)
					}), this.GetSession(cacheKey.OrganizationId));
				default:
					throw new InvalidOperationException("Unknown type");
				}
			};
			Func<OfflineAddressBookCacheEntry, IEnumerable<OfflineAddressBookCacheKey>> loadExtraKeys = delegate(OfflineAddressBookCacheEntry entry)
			{
				List<OfflineAddressBookCacheKey> list = new List<OfflineAddressBookCacheKey>(2);
				if (entry != null)
				{
					if (entry.Id != null)
					{
						list.Add(new OfflineAddressBookCacheKey(entry.Id, FilterType.OabId));
					}
					if (entry.IsDefault)
					{
						list.Add(new OfflineAddressBookCacheKey(entry.ConfigurationUnitId, FilterType.ConfigUnitId));
					}
					entry.NullConfigurationUnit();
				}
				return list;
			};
			Predicate<OfflineAddressBookCacheEntry> forceReload = (OfflineAddressBookCacheEntry entry) => entry.ElapsedTimeSinceCreated > OabCache.CacheStaleTime;
			this.lruCache = new OabLruCache<OfflineAddressBookCacheKey, OfflineAddressBookCacheEntry>(100000, loadItem, loadExtraKeys, forceReload);
		}

		public OfflineAddressBookCacheEntry GetOabById(OrganizationId orgId, ADObjectId oabId)
		{
			OfflineAddressBookCacheEntry offlineAddressBookCacheEntry = this.lruCache.Get(new OfflineAddressBookCacheKey(orgId, oabId, FilterType.OabId));
			if (!offlineAddressBookCacheEntry.HasValue)
			{
				return null;
			}
			return offlineAddressBookCacheEntry;
		}

		public OfflineAddressBookCacheEntry GetOabByOrganizationId(OrganizationId organizationId)
		{
			OfflineAddressBookCacheEntry offlineAddressBookCacheEntry = this.lruCache.Get(new OfflineAddressBookCacheKey(organizationId, organizationId.ConfigurationUnit, FilterType.ConfigUnitId));
			if (!offlineAddressBookCacheEntry.HasValue)
			{
				return null;
			}
			return offlineAddressBookCacheEntry;
		}

		private static OfflineAddressBookCacheEntry FindOab(ADObjectId rootId, QueryFilter filter, IConfigurationSession session)
		{
			IEnumerable<OfflineAddressBook> enumerable = session.FindPaged<OfflineAddressBook>(rootId, QueryScope.SubTree, filter, null, 0);
			OfflineAddressBook oab = null;
			using (IEnumerator<OfflineAddressBook> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					OfflineAddressBook offlineAddressBook = enumerator.Current;
					oab = offlineAddressBook;
				}
			}
			return OfflineAddressBookCacheEntry.Create(oab);
		}

		private IConfigurationSession GetSession(OrganizationId orgId)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 152, "GetSession", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationCache\\OabCache.cs");
		}

		private const int CacheSize = 100000;

		private static readonly TimeSpan CacheStaleTime = TimeSpan.FromHours(12.0);

		private readonly OabLruCache<OfflineAddressBookCacheKey, OfflineAddressBookCacheEntry> lruCache;
	}
}
