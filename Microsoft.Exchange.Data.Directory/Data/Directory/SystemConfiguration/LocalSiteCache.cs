using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class LocalSiteCache
	{
		public static ADSite LocalSite
		{
			get
			{
				LocalSiteCache.InitializeIfNeeded();
				return LocalSiteCache.localSite;
			}
		}

		private static void InitializeIfNeeded()
		{
			if (LocalSiteCache.localSite == null || LocalSiteCache.nextRefresh < DateTime.UtcNow)
			{
				LocalSiteCache.ReadLocalSite();
				LocalSiteCache.nextRefresh = DateTime.UtcNow + LocalSiteCache.RefreshInterval;
			}
		}

		private static void ReadLocalSite()
		{
			LocalSiteCache.Tracer.TraceDebug(0L, "LocalSiteCache: reading local site object");
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 70, "ReadLocalSite", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\LocalSiteCache.cs");
			LocalSiteCache.localSite = topologyConfigurationSession.GetLocalSite();
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1.0);

		private static DateTime nextRefresh;

		private static ADSite localSite;
	}
}
