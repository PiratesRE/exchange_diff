using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Data.Directory.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingTopology : RoutingTopologyBase
	{
		public RoutingTopology(DatabaseLoader databaseLoader, RoutingContextCore context)
		{
			this.databaseLoader = databaseLoader;
			this.routingTopologyCacheEnabled = context.Settings.RoutingTopologyCacheEnabled;
		}

		public override TopologyServer LocalServer
		{
			get
			{
				return this.siteTopology.LocalServer;
			}
		}

		public override IEnumerable<MiniDatabase> GetDatabases(bool forcedReload)
		{
			if (this.databases == null)
			{
				this.databases = this.databaseLoader.GetDatabases(base.WhenCreated, forcedReload);
			}
			return this.databases;
		}

		public override IEnumerable<TopologyServer> Servers
		{
			get
			{
				return this.siteTopology.AllTopologyServers;
			}
		}

		public override IList<TopologySite> Sites
		{
			get
			{
				return this.siteTopology.AllTopologySites;
			}
		}

		public override IList<MailGateway> SendConnectors
		{
			get
			{
				return this.sendConnectors;
			}
		}

		public override IList<PublicFolderTree> PublicFolderTrees
		{
			get
			{
				return this.publicFolderTrees;
			}
		}

		public override IList<RoutingGroup> RoutingGroups
		{
			get
			{
				return this.routingGroups;
			}
		}

		public override IList<RoutingGroupConnector> RoutingGroupConnectors
		{
			get
			{
				return this.routingGroupConnectors;
			}
		}

		public override IList<Server> HubServersOnEdge
		{
			get
			{
				throw new NotSupportedException("HubServersOnEdge property is not supported");
			}
		}

		public override void LogData(RoutingTableLogger logger)
		{
			logger.WriteStartElement("RoutingTopology");
			this.LogServers(logger);
			this.LogSiteTopology(logger);
			this.LogRoutingGroupTopology(logger);
			base.LogSendConnectors(logger);
			this.LogDatabases(logger);
			this.LogPublicFolderTrees(logger);
			logger.WriteEndElement();
		}

		protected override void PreLoadInternal()
		{
			RoutingDiag.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[{0}] Discovering topology", base.WhenCreated);
			if (this.routingTopologyCacheEnabled)
			{
				RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Getting routing topology from Service Cache.");
				using (TopologyServiceClient topologyServiceClient = TopologyServiceClient.CreateClient("localhost"))
				{
					byte[][] exchangeTopology = topologyServiceClient.GetExchangeTopology(DateTime.MinValue, ExchangeTopologyScope.ServerAndSiteTopology, false);
					ExchangeTopologyDiscovery.Simple topology = ExchangeTopologyDiscovery.Simple.Deserialize(exchangeTopology);
					ExchangeTopologyDiscovery topologyDiscovery = ExchangeTopologyDiscovery.Simple.CreateFrom(topology);
					this.siteTopology = ExchangeTopologyDiscovery.Populate(topologyDiscovery);
					goto IL_A1;
				}
			}
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Getting routing topology from AD.");
			this.siteTopology = ExchangeTopology.Discover(base.Session, ExchangeTopologyScope.ServerAndSiteTopology);
			IL_A1:
			this.sendConnectors = base.LoadAll<MailGateway>();
			this.publicFolderTrees = base.LoadAll<PublicFolderTree>();
			this.routingGroups = base.LoadAll<RoutingGroup>();
			this.routingGroupConnectors = base.LoadAll<RoutingGroupConnector>();
		}

		protected override void RegisterForADNotifications(ADNotificationCallback callback, IList<ADNotificationRequestCookie> cookies)
		{
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForMailGatewayNotifications(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForDatabaseNotifications(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.RegisterForNonDeletedNotifications<PublicFolderTree>(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.RegisterForNonDeletedNotifications<RoutingGroup>(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.RegisterForNonDeletedNotifications<RoutingGroupConnector>(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.RegisterForNonDeletedNotifications<AdministrativeGroup>(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.RegisterForNonDeletedNotifications<StorageGroup>(base.RootId, callback));
			ADObjectId childId = ADSession.GetConfigurationNamingContextForLocalForest().GetChildId("CN", "Sites");
			ADObjectId childId2 = childId.GetChildId("CN", "Inter-Site Transports").GetChildId("CN", "IP");
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForADSiteNotifications(childId, callback));
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForADSiteLinkNotifications(childId2, callback));
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForExchangeServerNotifications(null, callback));
		}

		protected override void Validate()
		{
			if (this.siteTopology.LocalServer == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime>((long)this.GetHashCode(), "[{0}] Topology Discovery returned null LocalServer", base.WhenCreated);
				throw new TransientRoutingException(Strings.RoutingNoLocalServer);
			}
			if (this.siteTopology.AllTopologySites.Count == 0)
			{
				RoutingDiag.Tracer.TraceError<DateTime>(0L, "[{0}] No AD sites found", base.WhenCreated);
				throw new TransientRoutingException(Strings.RoutingNoAdSites);
			}
			if (this.siteTopology.LocalServer.TopologySite == null)
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>(0L, "[{0}] Unable to determine local AD site from local server {1}", base.WhenCreated, this.siteTopology.LocalServer.Fqdn);
				throw new TransientRoutingException(Strings.RoutingNoLocalAdSite);
			}
			RoutingGroupRelayMap.ValidateTopologyConfig(this);
		}

		private void LogServers(RoutingTableLogger logger)
		{
			logger.WriteStartElement("ExchangeServers");
			foreach (TopologyServer server in this.siteTopology.AllTopologyServers)
			{
				logger.WriteTopologyServer(server);
			}
			logger.WriteEndElement();
			logger.WriteADReference("LocalServerId", this.LocalServer.Id);
		}

		private void LogSiteTopology(RoutingTableLogger logger)
		{
			logger.WriteStartElement("TopologySites");
			foreach (TopologySite site in this.Sites)
			{
				logger.WriteADSite(site);
			}
			logger.WriteEndElement();
			logger.WriteStartElement("TopologySiteLinks");
			foreach (TopologySiteLink link in this.siteTopology.AllTopologySiteLinks)
			{
				logger.WriteADSiteLink(link);
			}
			logger.WriteEndElement();
		}

		private void LogRoutingGroupTopology(RoutingTableLogger logger)
		{
			logger.WriteStartElement("RoutingGroups");
			foreach (RoutingGroup routingGroup in this.RoutingGroups)
			{
				logger.WriteRoutingGroup(routingGroup);
			}
			logger.WriteEndElement();
			logger.WriteStartElement("RoutingGroupConnectors");
			foreach (RoutingGroupConnector rgc in this.RoutingGroupConnectors)
			{
				logger.WriteRoutingGroupConnector(rgc);
			}
			logger.WriteEndElement();
		}

		private void LogDatabases(RoutingTableLogger logger)
		{
			if (this.databases == null)
			{
				throw new InvalidOperationException("Databases have not been read from AD yet");
			}
			logger.WriteStartElement("Databases");
			foreach (MiniDatabase database in this.databases)
			{
				logger.WriteDatabase(database);
			}
			logger.WriteEndElement();
		}

		private void LogPublicFolderTrees(RoutingTableLogger logger)
		{
			logger.WriteStartElement("PublicFolderTrees");
			foreach (PublicFolderTree publicFolderTree in this.publicFolderTrees)
			{
				logger.WritePublicFolderTree(publicFolderTree);
			}
			logger.WriteEndElement();
		}

		private ExchangeTopology siteTopology;

		private IList<MailGateway> sendConnectors;

		private IList<PublicFolderTree> publicFolderTrees;

		private IList<RoutingGroup> routingGroups;

		private IList<RoutingGroupConnector> routingGroupConnectors;

		private IEnumerable<MiniDatabase> databases;

		private readonly DatabaseLoader databaseLoader;

		private readonly bool routingTopologyCacheEnabled;
	}
}
