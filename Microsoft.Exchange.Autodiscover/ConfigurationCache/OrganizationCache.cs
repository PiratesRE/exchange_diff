using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal sealed class OrganizationCache : LazyLookupTimeoutCache<OrganizationId, Organization>
	{
		private OrganizationCache() : base(1, 1000, false, TimeSpan.FromSeconds(10800.0), TimeSpan.FromSeconds(10800.0))
		{
		}

		internal static OrganizationCache Singleton
		{
			get
			{
				return OrganizationCache.singleton;
			}
		}

		protected override Organization CreateOnCacheMiss(OrganizationId orgId, ref bool shouldAdd)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 56, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationCache\\OrganizationCache.cs");
			return tenantOrTopologyConfigurationSession.GetOrgContainer();
		}

		private const int OrganizationCacheBuckets = 1;

		private const int OrganizationCacheBucketSize = 1000;

		private const int CacheTimeoutInSeconds = 10800;

		private static OrganizationCache singleton = new OrganizationCache();
	}
}
