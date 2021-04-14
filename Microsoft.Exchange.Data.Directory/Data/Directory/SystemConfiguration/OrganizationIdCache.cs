using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OrganizationIdCache : LazyLookupTimeoutCache<OrganizationId, OrganizationIdCacheValue>
	{
		private OrganizationIdCache() : base(1, 1000, false, CacheTimeToLive.FederatedCacheTimeToLive)
		{
		}

		public static OrganizationIdCache Singleton
		{
			get
			{
				return OrganizationIdCache.singleton;
			}
		}

		protected override OrganizationIdCacheValue CreateOnCacheMiss(OrganizationId key, ref bool shouldAdd)
		{
			return new OrganizationIdCacheValue(key);
		}

		private static OrganizationIdCache singleton = new OrganizationIdCache();
	}
}
