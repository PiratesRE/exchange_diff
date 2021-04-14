using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class OrganizationCache : LazyLookupTimeoutCache<OrganizationId, Organization>
	{
		private OrganizationCache() : base(1, 1000, false, TimeSpan.FromSeconds((double)Global.OrganizationWideAccessPolicyTimeoutInSeconds), TimeSpan.FromSeconds((double)Global.OrganizationWideAccessPolicyTimeoutInSeconds))
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
			if (Global.OrganizationWideAccessPolicyTimeoutInSeconds == 0)
			{
				shouldAdd = false;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 63, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\OrganizationCache.cs");
			Organization orgContainer = tenantOrTopologyConfigurationSession.GetOrgContainer();
			shouldAdd = orgContainer.IsValid;
			return orgContainer;
		}

		private const int OrganizationCacheBuckets = 1;

		private const int OrganizationCacheBucketSize = 1000;

		private static OrganizationCache singleton = new OrganizationCache();
	}
}
