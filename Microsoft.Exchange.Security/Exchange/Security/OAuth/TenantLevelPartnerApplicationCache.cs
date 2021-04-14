using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class TenantLevelPartnerApplicationCache : LazyLookupTimeoutCache<TenantLevelPartnerApplicationCache.CacheKey, PartnerApplication>
	{
		private TenantLevelPartnerApplicationCache() : base(2, TenantLevelPartnerApplicationCache.cacheSize.Value, false, TenantLevelPartnerApplicationCache.cacheTimeToLive.Value)
		{
		}

		public static TenantLevelPartnerApplicationCache Singleton
		{
			get
			{
				if (TenantLevelPartnerApplicationCache.singleton == null)
				{
					lock (TenantLevelPartnerApplicationCache.lockObj)
					{
						if (TenantLevelPartnerApplicationCache.singleton == null)
						{
							TenantLevelPartnerApplicationCache.singleton = new TenantLevelPartnerApplicationCache();
						}
					}
				}
				return TenantLevelPartnerApplicationCache.singleton;
			}
		}

		public PartnerApplication Get(OrganizationId organizationId, string applicationId)
		{
			return base.Get(new TenantLevelPartnerApplicationCache.CacheKey(organizationId, applicationId));
		}

		protected override PartnerApplication CreateOnCacheMiss(TenantLevelPartnerApplicationCache.CacheKey key, ref bool shouldAdd)
		{
			ExTraceGlobals.OAuthTracer.TraceFunction(0L, "[TenantLevelPartnerApplicationCache::CreateOnCacheMiss] Entering");
			shouldAdd = true;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key.OrganizationId);
			IConfigurationSession configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 103, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\TenantLevelPartnerApplicationCache.cs");
			PartnerApplication[] results = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				results = configSession.Find<PartnerApplication>(PartnerApplication.GetContainerId(configSession), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, PartnerApplicationSchema.ApplicationIdentifier, key.ApplicationId), null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize);
			});
			switch (results.Length)
			{
			case 0:
				return null;
			case 1:
				return results[0];
			default:
				ExTraceGlobals.OAuthTracer.TraceError<int, string, OrganizationId>(0L, "[TenantLevelPartnerApplicationCache::CreateOnCacheMiss] found {0} partner application for given pid {1} in org {2}", results.Length, key.ApplicationId, key.OrganizationId);
				return null;
			}
		}

		private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("TenantLevelPartnerApplicationCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromHours(30.0), ExTraceGlobals.OAuthTracer);

		private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("TenantLevelPartnerApplicationCacheMaxItems", 500, ExTraceGlobals.OAuthTracer);

		private static readonly object lockObj = new object();

		private static TenantLevelPartnerApplicationCache singleton = null;

		internal sealed class CacheKey
		{
			public CacheKey(OrganizationId organizationId, string applicationId)
			{
				if (organizationId == OrganizationId.ForestWideOrgId)
				{
					throw new ArgumentException("organizationId must not come from first org");
				}
				this.organizationId = organizationId;
				this.applicationId = applicationId;
			}

			public OrganizationId OrganizationId
			{
				get
				{
					return this.organizationId;
				}
			}

			public string ApplicationId
			{
				get
				{
					return this.applicationId;
				}
			}

			public override bool Equals(object obj)
			{
				TenantLevelPartnerApplicationCache.CacheKey cacheKey = obj as TenantLevelPartnerApplicationCache.CacheKey;
				return cacheKey != null && this.organizationId.Equals(cacheKey.organizationId) && this.applicationId == cacheKey.applicationId;
			}

			public override int GetHashCode()
			{
				return this.organizationId.GetHashCode() ^ this.applicationId.GetHashCode();
			}

			private readonly OrganizationId organizationId;

			private readonly string applicationId;
		}
	}
}
