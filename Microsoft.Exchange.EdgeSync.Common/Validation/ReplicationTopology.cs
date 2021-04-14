using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Common.Internal;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal sealed class ReplicationTopology
	{
		public Server LocalHub
		{
			get
			{
				return this.localHub;
			}
		}

		public IConfigurationSession ConfigSession
		{
			get
			{
				return this.configSession;
			}
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		public List<Server> SiteEdgeServers
		{
			get
			{
				return this.siteEdgeServers;
			}
		}

		public List<Server> SiteHubServers
		{
			get
			{
				return this.siteHubServers;
			}
		}

		public EdgeSyncServiceConfig EdgeSyncServiceConfig
		{
			get
			{
				return this.edgeSyncServiceConfig;
			}
		}

		public ADSite LocalSite
		{
			get
			{
				return this.localSite;
			}
		}

		private ReplicationTopology(IConfigurationSession configSession, Server localHub, ADSite localSite, EdgeSyncServiceConfig edgeSyncServiceConfig)
		{
			this.configSession = configSession;
			this.localHub = localHub;
			this.localSite = localSite;
			this.edgeSyncServiceConfig = edgeSyncServiceConfig;
		}

		public static bool TryLoadLocalSiteTopology(string domainController, out ReplicationTopology topology, out Exception exception)
		{
			topology = null;
			exception = null;
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 154, "TryLoadLocalSiteTopology", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Common\\Validation\\ReplicationTopology.cs");
			ADSite localSite = null;
			EdgeSyncServiceConfig edgeSyncServiceConfig = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localSite = session.GetLocalSite();
				if (localSite == null)
				{
					throw new ADTransientException(Strings.CannotGetLocalSite);
				}
				edgeSyncServiceConfig = session.Read<EdgeSyncServiceConfig>(localSite.Id.GetChildId("EdgeSyncService"));
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				exception = adoperationResult.Exception;
				return false;
			}
			if (edgeSyncServiceConfig == null)
			{
				topology = new ReplicationTopology(session, null, localSite, null);
				return true;
			}
			ReplicationTopology resultTopology = null;
			adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				Server server = session.FindLocalServer();
				resultTopology = new ReplicationTopology(session, server, localSite, edgeSyncServiceConfig);
				QueryFilter filter = Util.BuildServerFilterForSite(localSite.Id);
				ADPagedReader<Server> adpagedReader = session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				resultTopology.siteEdgeServers.Clear();
				resultTopology.siteHubServers.Clear();
				foreach (Server server2 in adpagedReader)
				{
					if (server2.IsEdgeServer)
					{
						resultTopology.siteEdgeServers.Add(server2);
					}
					if (server2.IsHubTransportServer)
					{
						resultTopology.siteHubServers.Add(server2);
					}
				}
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				exception = adoperationResult.Exception;
				return false;
			}
			topology = resultTopology;
			return true;
		}

		private readonly Server localHub;

		private readonly ADSite localSite;

		private readonly EdgeSyncServiceConfig edgeSyncServiceConfig;

		private readonly List<Server> siteEdgeServers = new List<Server>();

		private readonly List<Server> siteHubServers = new List<Server>();

		private readonly IConfigurationSession configSession;

		private readonly IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 66, "recipientSession", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Common\\Validation\\ReplicationTopology.cs");
	}
}
