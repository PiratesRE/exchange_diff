using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaMailboxPolicyCache : LazyLookupTimeoutCache<ADObjectId, PolicyConfiguration>
	{
		private OwaMailboxPolicyCache() : base(5, 1000, false, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(60.0))
		{
		}

		protected override PolicyConfiguration CreateOnCacheMiss(ADObjectId key, ref bool shouldAdd)
		{
			shouldAdd = true;
			return this.GetPolicyFromAD(key);
		}

		private PolicyConfiguration GetPolicyFromAD(ADObjectId key)
		{
			IConfigurationSession configurationSession = Utilities.CreateConfigurationSessionScoped(true, ConsistencyMode.FullyConsistent, key);
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			return PolicyConfiguration.GetPolicyConfigurationFromAD(configurationSession, key);
		}

		internal static OwaMailboxPolicyCache Instance
		{
			get
			{
				return OwaMailboxPolicyCache.instance;
			}
		}

		private const int OwaMailboxPolicyCacheBucketSize = 1000;

		private const int OwaMailboxPolicyCacheBuckets = 5;

		private static OwaMailboxPolicyCache instance = new OwaMailboxPolicyCache();
	}
}
