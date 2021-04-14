using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EdgeRoutingTopology : RoutingTopologyBase
	{
		public override TopologyServer LocalServer
		{
			get
			{
				throw new NotSupportedException("LocalServer property is not supported");
			}
		}

		public override IEnumerable<MiniDatabase> GetDatabases(bool forcedReload)
		{
			throw new NotSupportedException("Databases property is not supported");
		}

		public override IEnumerable<TopologyServer> Servers
		{
			get
			{
				throw new NotSupportedException("Servers property is not supported");
			}
		}

		public override IList<TopologySite> Sites
		{
			get
			{
				throw new NotSupportedException("Sites property is not supported");
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
				throw new NotSupportedException("PublicFolderTrees property is not supported");
			}
		}

		public override IList<RoutingGroup> RoutingGroups
		{
			get
			{
				throw new NotSupportedException("RoutingGroups property is not supported");
			}
		}

		public override IList<RoutingGroupConnector> RoutingGroupConnectors
		{
			get
			{
				throw new NotSupportedException("RoutingGroupConnectors property is not supported");
			}
		}

		public override IList<Server> HubServersOnEdge
		{
			get
			{
				return this.hubServersOnEdge;
			}
		}

		public override void LogData(RoutingTableLogger logger)
		{
			logger.WriteStartElement("EdgeRoutingTopology");
			base.LogSendConnectors(logger);
			this.LogHubServers(logger);
			logger.WriteEndElement();
		}

		protected override void PreLoadInternal()
		{
			this.sendConnectors = base.LoadAll<MailGateway>();
			this.hubServersOnEdge = base.LoadAll<Server>(delegate(Server server)
			{
				if (!server.IsHubTransportServer)
				{
					return false;
				}
				if (string.IsNullOrEmpty(server.Fqdn))
				{
					RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No FQDN for Server object with DN: {1}. Skipping it.", base.WhenCreated, server.DistinguishedName);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoServerFqdn, null, new object[]
					{
						server.DistinguishedName,
						base.WhenCreated
					});
					return false;
				}
				return true;
			});
		}

		protected override void RegisterForADNotifications(ADNotificationCallback callback, IList<ADNotificationRequestCookie> cookies)
		{
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForMailGatewayNotifications(base.RootId, callback));
			cookies.Add(TransportADNotificationAdapter.Instance.RegisterForExchangeServerNotifications(base.RootId, callback));
		}

		protected override void Validate()
		{
		}

		private void LogHubServers(RoutingTableLogger logger)
		{
			logger.WriteStartElement("SyncedServers");
			foreach (Server server in this.hubServersOnEdge)
			{
				logger.WriteServer(server);
			}
			logger.WriteEndElement();
		}

		private IList<MailGateway> sendConnectors;

		private IList<Server> hubServersOnEdge;
	}
}
