using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaMailboxPolicyCache : LazyLookupTimeoutCache<OrgIdADObjectWrapper, PolicyConfiguration>
	{
		private OwaMailboxPolicyCache() : base(5, 1000, false, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(60.0))
		{
		}

		internal static OwaMailboxPolicyCache Instance
		{
			get
			{
				return OwaMailboxPolicyCache.instance;
			}
		}

		protected override PolicyConfiguration CreateOnCacheMiss(OrgIdADObjectWrapper key, ref bool shouldAdd)
		{
			shouldAdd = true;
			return this.GetPolicyFromAD(key);
		}

		private PolicyConfiguration GetPolicyFromAD(OrgIdADObjectWrapper key)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key.OrgId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, sessionSettings, 89, "GetPolicyFromAD", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\configuration\\OwaMailboxPolicyCache.cs");
			return PolicyConfiguration.GetPolicyConfigurationFromAD(tenantOrTopologyConfigurationSession, key.AdObject);
		}

		internal static PolicyConfiguration GetPolicyConfiguration(ADObjectId owaMailboxPolicyId, OrganizationId organizationId)
		{
			PolicyConfiguration policyConfiguration = null;
			if (owaMailboxPolicyId != null)
			{
				policyConfiguration = OwaMailboxPolicyCache.Instance.Get(new OrgIdADObjectWrapper(owaMailboxPolicyId, organizationId));
			}
			if (policyConfiguration == null)
			{
				ADObjectId adobjectId = OwaMailboxPolicyIdCacheByOrganization.Instance.Get(organizationId);
				if (adobjectId != null)
				{
					policyConfiguration = OwaMailboxPolicyCache.Instance.Get(new OrgIdADObjectWrapper(adobjectId, organizationId));
				}
			}
			return policyConfiguration;
		}

		private const int OwaMailboxPolicyCacheBucketSize = 1000;

		private const int OwaMailboxPolicyCacheBuckets = 5;

		private static OwaMailboxPolicyCache instance = new OwaMailboxPolicyCache();
	}
}
