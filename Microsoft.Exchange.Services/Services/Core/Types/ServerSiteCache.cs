using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServerSiteCache : LazyLookupTimeoutCache<string, ADObjectId>
	{
		private ServerSiteCache() : base(2, 1000, false, TimeSpan.FromDays(1.0))
		{
		}

		protected override ADObjectId CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			Server server = ServerSiteCache.session.FindServerByFqdn(key);
			if (server != null)
			{
				return server.ServerSite;
			}
			return null;
		}

		protected override string PreprocessKey(string key)
		{
			return key.ToLowerInvariant();
		}

		public static ServerSiteCache Singleton
		{
			get
			{
				return ServerSiteCache.singleton;
			}
		}

		private static readonly ServerSiteCache singleton = new ServerSiteCache();

		private static readonly ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 21, "session", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\RequestProxying\\ServerSiteCache.cs");
	}
}
