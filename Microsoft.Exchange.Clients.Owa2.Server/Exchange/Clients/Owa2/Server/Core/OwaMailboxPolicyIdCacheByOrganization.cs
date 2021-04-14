using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaMailboxPolicyIdCacheByOrganization : LazyLookupTimeoutCache<OrganizationId, ADObjectId>
	{
		private OwaMailboxPolicyIdCacheByOrganization() : base(5, 1000, false, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(60.0))
		{
		}

		internal static OwaMailboxPolicyIdCacheByOrganization Instance
		{
			get
			{
				return OwaMailboxPolicyIdCacheByOrganization.instance;
			}
		}

		protected override ADObjectId CreateOnCacheMiss(OrganizationId key, ref bool shouldAdd)
		{
			shouldAdd = true;
			return this.GetPolicyIdFromAD(key);
		}

		private ADObjectId GetPolicyIdFromAD(OrganizationId key)
		{
			OwaMailboxPolicy defaultOwaMailboxPolicy = OwaSegmentationSettings.GetDefaultOwaMailboxPolicy(key);
			if (defaultOwaMailboxPolicy == null)
			{
				return null;
			}
			return defaultOwaMailboxPolicy.Id;
		}

		private const int OwaMailboxPolicyIdCacheByOrganizationBucketSize = 1000;

		private const int OwaMailboxPolicyIdCacheByOrganizationBuckets = 5;

		private static OwaMailboxPolicyIdCacheByOrganization instance = new OwaMailboxPolicyIdCacheByOrganization();
	}
}
