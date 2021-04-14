using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class TransportServerConfigCache : LazyLookupTimeoutCacheWithDiagnostics<string, ServerInfo>
	{
		public TransportServerConfigCache() : base(2, 100, false, TimeSpan.FromHours(5.0))
		{
		}

		protected override string PreprocessKey(string key)
		{
			return key.ToUpperInvariant();
		}

		protected override ServerInfo Create(string key, ref bool shouldAdd)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "TransportServerConfigCache miss, searching for {0}", key);
			ITopologyConfigurationSession globalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false), 74, "Create", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\Caching\\TransportServerConfigCache.cs");
			shouldAdd = true;
			return TransportServerConfigCache.FindServer(globalConfigSession, key, 34UL);
		}

		public ServerInfo FindServer(string serverNameOrFqdn, ulong roleMask)
		{
			ServerInfo result = base.Get(serverNameOrFqdn);
			if ((roleMask & result.Roles) != 0UL)
			{
				return result;
			}
			if (result.Status == ServerStatus.LegacyExchangeServer)
			{
				return ServerInfo.LegacyExchangeServer;
			}
			return ServerInfo.NotFound;
		}

		public static ServerInfo FindServer(ITopologyConfigurationSession globalConfigSession, string serverNameOrFqdn, ulong roleMask)
		{
			return ServerInfo.GetServerByName(serverNameOrFqdn, roleMask, globalConfigSession);
		}

		private const ulong MailboxAndHubRoleMask = 34UL;
	}
}
