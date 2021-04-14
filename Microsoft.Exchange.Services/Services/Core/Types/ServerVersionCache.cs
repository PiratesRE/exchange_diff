using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServerVersionCache : LazyLookupTimeoutCache<string, int>
	{
		private ServerVersionCache() : base(2, 1000, false, TimeSpan.FromDays(1.0))
		{
		}

		protected override int CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			Server server = ServerVersionCache.session.FindServerByFqdn(key);
			if (server != null)
			{
				return server.VersionNumber;
			}
			return 0;
		}

		protected override string PreprocessKey(string key)
		{
			return key.ToLowerInvariant();
		}

		public static ServerVersionCache Singleton
		{
			get
			{
				return ServerVersionCache.singleton;
			}
		}

		private static readonly ServerVersionCache singleton = new ServerVersionCache();

		private static readonly ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 21, "session", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\ServerVersionCache.cs");
	}
}
