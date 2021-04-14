using System;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientAccessRulesCache : TenantConfigurationCache<ClientAccessRuleCollectionCacheableItem>
	{
		private static TimeSpan CacheTimeToLive
		{
			get
			{
				return ClientAccessRulesCache.CacheTimeToLiveData.Value;
			}
		}

		public static ClientAccessRulesCache Instance
		{
			get
			{
				return ClientAccessRulesCache.instance;
			}
		}

		public ClientAccessRuleCollection GetCollection(OrganizationId orgId)
		{
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return this.GetValue(orgId).ClientAccessRuleCollection;
			}
			ClientAccessRuleCollection clientAccessRuleCollection = new ClientAccessRuleCollection(orgId.ToString());
			clientAccessRuleCollection.AddClientAccessRuleCollection(this.GetValue(OrganizationId.ForestWideOrgId).ClientAccessRuleCollection);
			clientAccessRuleCollection.AddClientAccessRuleCollection(this.GetValue(orgId).ClientAccessRuleCollection);
			return clientAccessRuleCollection;
		}

		private ClientAccessRulesCache() : base(ClientAccessRulesCache.CacheSizeInBytes, ClientAccessRulesCache.CacheTimeToLive, TimeSpan.Zero, null, null)
		{
		}

		private static readonly long CacheSizeInBytes = (long)ByteQuantifiedSize.FromMB(1UL).ToBytes();

		private static readonly TimeSpanAppSettingsEntry CacheTimeToLiveData = new TimeSpanAppSettingsEntry("ClientAccessRulesCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes((double)VariantConfiguration.InvariantNoFlightingSnapshot.ClientAccessRulesCommon.ClientAccessRulesCacheExpiryTime.Value), ExTraceGlobals.SystemConfigurationCacheTracer);

		private static ClientAccessRulesCache instance = new ClientAccessRulesCache();
	}
}
