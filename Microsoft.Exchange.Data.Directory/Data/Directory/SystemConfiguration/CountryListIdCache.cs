using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class CountryListIdCache : LazyLookupTimeoutCache<CountryListKey, CountryList>
	{
		private CountryListIdCache() : base(1, 10, false, CacheTimeToLive.GlobalCountryListCacheTimeToLive)
		{
		}

		public static CountryListIdCache Singleton
		{
			get
			{
				return CountryListIdCache.singleton;
			}
		}

		protected override CountryList CreateOnCacheMiss(CountryListKey key, ref bool shouldAdd)
		{
			CountryList countryList = null;
			if (null != key)
			{
				ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, OrganizationId.ForestWideOrgId, OrganizationId.ForestWideOrgId, false);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, sessionSettings, 145, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\CountryListIdCache.cs");
				countryList = tenantOrTopologyConfigurationSession.Read<CountryList>(key.Key);
				if (countryList == null)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_UMCountryListNotFound, key.ToString(), new object[]
					{
						key
					});
				}
			}
			return countryList;
		}

		private static CountryListIdCache singleton = new CountryListIdCache();
	}
}
