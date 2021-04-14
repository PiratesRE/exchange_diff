using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal struct ServerInfo
	{
		internal string Key
		{
			get
			{
				return this.lowerCaseFqdn;
			}
		}

		internal ServerStatus Status
		{
			get
			{
				return this.status;
			}
		}

		internal ADObjectId ServerSiteId
		{
			get
			{
				return this.serverSiteId;
			}
		}

		internal ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return this.databaseAvailabilityGroup;
			}
		}

		internal bool IsSearchable
		{
			get
			{
				return ServerStatus.Searchable == this.status;
			}
		}

		internal ulong Roles
		{
			get
			{
				return this.roles;
			}
		}

		internal ServerVersion AdminDisplayVersion
		{
			get
			{
				return this.adminDisplayVersion;
			}
		}

		internal ServerInfo(string lowerCaseFqdn, ServerStatus status, ADObjectId serverSiteId, ADObjectId databaseAvailabilityGroup, ServerVersion version, ulong roles)
		{
			this.lowerCaseFqdn = lowerCaseFqdn;
			this.status = status;
			this.serverSiteId = serverSiteId;
			this.databaseAvailabilityGroup = databaseAvailabilityGroup;
			this.adminDisplayVersion = version;
			this.roles = roles;
		}

		public override string ToString()
		{
			return this.status.ToString() + ":" + (this.lowerCaseFqdn ?? "null");
		}

		internal static int GetHubServersInSite(IConfigurationSession session, ADObjectId site, out List<string> serverFqdns)
		{
			serverFqdns = new List<string>(0);
			IEnumerable<ServerInfo> serversInSiteByRole = ServerInfo.GetServersInSiteByRole(session, ServerInfo.HubRoleFilter, site);
			foreach (ServerInfo serverInfo in serversInSiteByRole)
			{
				serverFqdns.Add(serverInfo.lowerCaseFqdn);
			}
			return serverFqdns.Count;
		}

		internal static int GetCASServersInSite(IConfigurationSession session, ADObjectId site, out List<ServerInfo> serverInfoList)
		{
			IEnumerable<ServerInfo> serversInSiteByRole = ServerInfo.GetServersInSiteByRole(session, ServerInfo.CASRoleFilter, site);
			serverInfoList = new List<ServerInfo>(serversInSiteByRole);
			return serverInfoList.Count;
		}

		internal static ServerInfo GetServerByName(string serverName, ulong roleMask, ITopologyConfigurationSession session)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "null/empty servername search, returning not-found", new object[0]);
				return ServerInfo.NotFound;
			}
			Server server;
			if (serverName.Contains("."))
			{
				server = session.FindServerByFqdn(serverName);
			}
			else
			{
				server = session.FindServerByName(serverName);
			}
			if (server == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(0, "Server name: {0} is not found", serverName);
				return ServerInfo.NotFound;
			}
			if (((long)server.CurrentServerRole & (long)roleMask) != 0L)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, ulong>(0, "Found server: {0} with roles: {1}", server.Fqdn, (ulong)((long)server.CurrentServerRole));
				return ServerInfo.CreateServerInfoFromServer(server);
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, ulong, ulong>(0, "Server name: {0} roles do not match. Searched for: {1}, actual: {2}", serverName, roleMask, (ulong)((long)server.CurrentServerRole));
			if (!server.IsE14OrLater)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Server is legacy Exchange Server", new object[0]);
				return ServerInfo.LegacyExchangeServer;
			}
			return ServerInfo.NotFound;
		}

		private static IEnumerable<ServerInfo> GetServersInSiteByRole(IConfigurationSession session, QueryFilter roleFilter, ADObjectId site)
		{
			ComparisonFilter userSiteFilter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, site);
			AndFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				userSiteFilter,
				roleFilter
			});
			ADPagedReader<Server> servers = session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 0);
			foreach (Server server in servers)
			{
				ServerInfo serverInfo = ServerInfo.CreateServerInfoFromServer(server);
				if (!serverInfo.IsSearchable)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<string>(0, "Server: {0}, not searchable", serverInfo.Key);
				}
				else if (string.IsNullOrEmpty(server.Fqdn))
				{
					TraceWrapper.SearchLibraryTracer.TraceError(0, "Null/empty server-name, skipping", new object[0]);
				}
				else
				{
					yield return serverInfo;
				}
			}
			yield break;
		}

		private static ServerInfo CreateServerInfoFromServer(Server server)
		{
			if (string.IsNullOrEmpty(server.Fqdn))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Null or Empty FQDN", new object[0]);
				return ServerInfo.NotFound;
			}
			if (!server.IsE14OrLater)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Pre-E14 server {0}", server.Fqdn);
				return ServerInfo.LegacyExchangeServer;
			}
			ServerVersion serverVersion = server.AdminDisplayVersion;
			if (serverVersion == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Could not get build for server {0}", server.Fqdn);
				return ServerInfo.NotExchangeServer;
			}
			string text = server.Fqdn.ToLowerInvariant();
			return new ServerInfo(text, ServerStatus.Searchable, server.ServerSite, server.DatabaseAvailabilityGroup, serverVersion, (ulong)((long)server.CurrentServerRole));
		}

		private static readonly BitMaskAndFilter HubRoleFilter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL);

		private static readonly BitMaskAndFilter CASRoleFilter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 4UL);

		internal static readonly ServerInfo NotFound = new ServerInfo(null, ServerStatus.NotFound, null, null, null, 0UL);

		internal static readonly ServerInfo NotExchangeServer = new ServerInfo(null, ServerStatus.NotExchangeServer, null, null, null, 0UL);

		internal static readonly ServerInfo LegacyExchangeServer = new ServerInfo(null, ServerStatus.LegacyExchangeServer, null, null, null, 0UL);

		private string lowerCaseFqdn;

		private ServerStatus status;

		private ADObjectId serverSiteId;

		private ADObjectId databaseAvailabilityGroup;

		private ulong roles;

		private ServerVersion adminDisplayVersion;
	}
}
