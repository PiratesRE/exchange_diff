using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ConnectorDeliveryHop : RoutingNextHop
	{
		public ConnectorDeliveryHop(SendConnector connector, RoutingContextCore contextCore)
		{
			RoutingUtils.ThrowIfNull(connector, "connector");
			this.connector = connector;
			SmtpSendConnectorConfig smtpSendConnectorConfig = connector as SmtpSendConnectorConfig;
			if (smtpSendConnectorConfig != null)
			{
				if (smtpSendConnectorConfig.DNSRoutingEnabled)
				{
					this.deliveryType = DeliveryType.DnsConnectorDelivery;
					return;
				}
				this.deliveryType = DeliveryType.SmartHostConnectorDelivery;
				this.nextHopDomain = smtpSendConnectorConfig.SmartHostsString;
				this.SetSmartHosts(smtpSendConnectorConfig.SmartHosts, contextCore);
				return;
			}
			else
			{
				if (connector is DeliveryAgentConnector)
				{
					this.deliveryType = DeliveryType.DeliveryAgent;
					return;
				}
				if (connector is ForeignConnector)
				{
					this.deliveryType = DeliveryType.NonSmtpGatewayDelivery;
					this.nextHopDomain = connector.Name;
					return;
				}
				if (connector is RoutingGroupConnector)
				{
					this.deliveryType = DeliveryType.SmtpRelayToTiRg;
					this.nextHopDomain = connector.Name;
					return;
				}
				throw new InvalidOperationException("Unexpected type of local connector: " + connector.GetType());
			}
		}

		public ConnectorDeliveryHop(SmtpSendConnectorConfig connector, IList<SmartHost> expandedSmartHosts, string expandedSmartHostsString, RoutingContextCore contextCore)
		{
			RoutingUtils.ThrowIfNull(connector, "connector");
			RoutingUtils.ThrowIfNullOrEmpty(expandedSmartHostsString, "expandedSmartHostsString");
			RoutingUtils.ThrowIfNullOrEmpty<SmartHost>(expandedSmartHosts, "expandedSmartHosts");
			if (connector.DNSRoutingEnabled)
			{
				throw new ArgumentOutOfRangeException("connector", "Connector must be a smart host connector");
			}
			this.connector = connector;
			this.deliveryType = DeliveryType.SmartHostConnectorDelivery;
			this.nextHopDomain = expandedSmartHostsString;
			this.SetSmartHosts(expandedSmartHosts, contextCore);
		}

		public SendConnector Connector
		{
			get
			{
				return this.connector;
			}
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return this.connector.Guid;
			}
		}

		public override SmtpSendConnectorConfig NextHopConnector
		{
			get
			{
				return this.connector as SmtpSendConnectorConfig;
			}
		}

		public static IList<INextHopServer> GetNextHopServersForDomain(string nextHopDomain)
		{
			List<INextHopServer> routingHostsFromString = RoutingHost.GetRoutingHostsFromString<INextHopServer>(nextHopDomain, (RoutingHost routingHost) => routingHost);
			if (routingHostsFromString.Count == 0)
			{
				routingHostsFromString.Add(new ConnectorDeliveryHop.NextHopFqdn(nextHopDomain));
			}
			RoutingUtils.ShuffleList<INextHopServer>(routingHostsFromString);
			return routingHostsFromString;
		}

		public override string GetNextHopDomain(RoutingContext context)
		{
			if (this.deliveryType != DeliveryType.DnsConnectorDelivery && this.deliveryType != DeliveryType.DeliveryAgent)
			{
				return this.nextHopDomain;
			}
			string text;
			string text2;
			string result;
			if (!ConnectorRoutingDestination.TryGetRecipientAddress(context.CurrentRecipient, context, out text, out text2, out result))
			{
				throw new InvalidOperationException("Invalid recipient address in GetNextHopDomainForRecipient(): " + context.CurrentRecipient.Email);
			}
			return result;
		}

		public override IEnumerable<INextHopServer> GetLoadBalancedNextHopServers(string nextHopDomain)
		{
			if (this.deliveryType == DeliveryType.SmartHostConnectorDelivery || this.deliveryType == DeliveryType.SmtpRelayToTiRg)
			{
				if (this.hosts == null || this.hosts.Count == 0)
				{
					throw new InvalidOperationException("No hosts set for delivery type " + this.deliveryType);
				}
				return this.hosts.LoadBalancedCollection;
			}
			else
			{
				if (this.deliveryType == DeliveryType.DnsConnectorDelivery)
				{
					return ConnectorDeliveryHop.GetNextHopServersForDomain(nextHopDomain);
				}
				throw new NotSupportedException("GetLoadBalancedNextHopServers() is not supported for delivery type " + this.deliveryType);
			}
		}

		public override bool Match(RoutingNextHop other)
		{
			ConnectorDeliveryHop connectorDeliveryHop = other as ConnectorDeliveryHop;
			if (connectorDeliveryHop == null)
			{
				return false;
			}
			if (this.NextHopGuid != connectorDeliveryHop.NextHopGuid || this.deliveryType != connectorDeliveryHop.deliveryType || !RoutingUtils.MatchStrings(this.nextHopDomain, connectorDeliveryHop.nextHopDomain))
			{
				return false;
			}
			return RoutingUtils.MatchLists<INextHopServer>(this.hosts, connectorDeliveryHop.hosts, (INextHopServer host1, INextHopServer host2) => RoutingUtils.MatchNextHopServers(host1, host2));
		}

		public void SetTargetBridgeheads(ListLoadBalancer<INextHopServer> targetBridgeheads)
		{
			RoutingUtils.ThrowIfNullOrEmpty<INextHopServer>(targetBridgeheads, "targetBridgeheads");
			if (this.deliveryType != DeliveryType.SmtpRelayToTiRg)
			{
				throw new InvalidOperationException("Cannot set target bridgeheads for delivery type " + this.deliveryType);
			}
			this.hosts = targetBridgeheads;
		}

		protected override NextHopSolutionKey GetNextHopSolutionKey(RoutingContext context)
		{
			return new NextHopSolutionKey(this.DeliveryType, this.GetNextHopDomain(context), this.NextHopGuid, context.CurrentRecipient.TlsAuthLevel, context.CurrentRecipient.GetTlsDomain(), context.CurrentRecipient.OverrideSource);
		}

		private void SetSmartHosts(IList<SmartHost> smartHosts, RoutingContextCore contextCore)
		{
			this.hosts = new ListLoadBalancer<INextHopServer>(contextCore.Settings.RandomLoadBalancingOffsetEnabled);
			foreach (SmartHost smartHost in smartHosts)
			{
				this.hosts.AddItem(smartHost.InnerRoutingHost);
			}
		}

		private readonly SendConnector connector;

		private readonly DeliveryType deliveryType;

		private readonly string nextHopDomain;

		private ListLoadBalancer<INextHopServer> hosts;

		private class NextHopFqdn : INextHopServer
		{
			public NextHopFqdn(string fqdn)
			{
				this.fqdn = fqdn;
			}

			bool INextHopServer.IsIPAddress
			{
				get
				{
					return false;
				}
			}

			IPAddress INextHopServer.Address
			{
				get
				{
					throw new InvalidOperationException("INextHopServer.Address must not be requested from NextHopFqdn objects");
				}
			}

			string INextHopServer.Fqdn
			{
				get
				{
					return this.fqdn;
				}
			}

			bool INextHopServer.IsFrontendAndHubColocatedServer
			{
				get
				{
					return false;
				}
			}

			private readonly string fqdn;
		}
	}
}
