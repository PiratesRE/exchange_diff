using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class SiteConfigCache : LazyLookupTimeoutCacheWithDiagnostics<ADObjectId, SiteConfigCache.Item>
	{
		public SiteConfigCache() : base(2, 20, false, TimeSpan.FromHours(1.0))
		{
		}

		protected override SiteConfigCache.Item Create(ADObjectId key, ref bool shouldAdd)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<ADObjectId>(this.GetHashCode(), "SiteConfigCache miss, searching for {0}", key);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false), 116, "Create", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\Caching\\SiteConfigCache.cs");
			shouldAdd = true;
			return SiteConfigCache.GetSiteConfiguration(tenantOrTopologyConfigurationSession, key);
		}

		public static SiteConfigCache.Item GetSiteConfiguration(IConfigurationSession globalConfigSession, ADObjectId siteId)
		{
			List<ServerInfo> casServerInfos;
			int arg = ServerInfo.GetCASServersInSite(globalConfigSession, siteId, out casServerInfos);
			TraceWrapper.SearchLibraryTracer.TraceDebug<int>(0, "Added {0} CAS servers for site", arg);
			List<string> hubServerFqdns;
			arg = ServerInfo.GetHubServersInSite(globalConfigSession, siteId, out hubServerFqdns);
			TraceWrapper.SearchLibraryTracer.TraceDebug<int>(0, "Added {0} HUB servers for site", arg);
			return new SiteConfigCache.Item(casServerInfos, hubServerFqdns);
		}

		internal sealed class Item
		{
			public Item(List<ServerInfo> casServerInfos, List<string> hubServerFqdns)
			{
				if (casServerInfos == null)
				{
					throw new NullReferenceException("casServerInfos must not be null");
				}
				if (hubServerFqdns == null)
				{
					throw new NullReferenceException("hubServerFqdns must not be null");
				}
				this.CasServerInfos = casServerInfos;
				this.HubServerFqdns = hubServerFqdns;
				this.HubServerTable = new HashSet<string>();
				foreach (string text in this.HubServerFqdns)
				{
					this.HubServerTable.Add(text);
					int num = text.IndexOf('.');
					string text2 = null;
					if (num != -1)
					{
						text2 = text.Substring(0, num);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						this.HubServerTable.Add(text2);
					}
				}
			}

			public List<ServerInfo> CasServerInfos { get; private set; }

			public List<string> HubServerFqdns { get; private set; }

			public HashSet<string> HubServerTable { get; private set; }
		}
	}
}
