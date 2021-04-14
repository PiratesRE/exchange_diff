using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class LinkedAccountCache : LazyLookupTimeoutCache<ADObjectId, ADUser>
	{
		private LinkedAccountCache() : base(1, LinkedAccountCache.CacheSize.Value, false, LinkedAccountCache.CacheTimeToLive.Value)
		{
		}

		public static LinkedAccountCache Instance
		{
			get
			{
				return LinkedAccountCache.LazyInstance.Value;
			}
		}

		protected override ADUser CreateOnCacheMiss(ADObjectId key, ref bool shouldAdd)
		{
			ADUser result = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				result = this.ResolveLinkedAccount(key);
			});
			if (result == null)
			{
				ExTraceGlobals.OAuthTracer.Information<ADObjectId>((long)this.GetHashCode(), "[LinkedAccountCache::CreateOnCacheMiss] Skip to put null in the cache. Key: {0}", key);
				shouldAdd = false;
			}
			return result;
		}

		private ADUser ResolveLinkedAccount(ADObjectId linkedAccount)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(linkedAccount);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 99, "ResolveLinkedAccount", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\LinkedAccountCache.cs");
			ExTraceGlobals.OAuthTracer.TraceDebug<ADObjectId>(0, 0L, "[LinkedAccountCache::ResolveLinkedAccount] Lookup linkedAccount in AD. LinkedAccount: {0}", linkedAccount);
			return tenantOrRootOrgRecipientSession.FindADUserByObjectId(linkedAccount);
		}

		private static readonly TimeSpanAppSettingsEntry CacheTimeToLive = new TimeSpanAppSettingsEntry("OAuthLinkedAccountCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromHours(24.0), ExTraceGlobals.OAuthTracer);

		private static readonly IntAppSettingsEntry CacheSize = new IntAppSettingsEntry("OAuthLinkedAccountCacheMaxItems", 1000, ExTraceGlobals.OAuthTracer);

		private static readonly Lazy<LinkedAccountCache> LazyInstance = new Lazy<LinkedAccountCache>(() => new LinkedAccountCache());
	}
}
