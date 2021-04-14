using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ConnectorRoutingDestination : RoutingDestination
	{
		public ConnectorRoutingDestination(MailGateway connector, IList<AddressSpace> addressSpaces, RouteInfo routeInfo) : base(routeInfo)
		{
			RoutingUtils.ThrowIfNull(connector, "connector");
			RoutingUtils.ThrowIfNullOrEmpty<AddressSpace>(addressSpaces, "addressSpaces");
			this.connector = connector;
			this.addressSpaces = addressSpaces;
		}

		public IList<AddressSpace> AddressSpaces
		{
			get
			{
				return this.addressSpaces;
			}
		}

		public override MailRecipientType DestinationType
		{
			get
			{
				return MailRecipientType.External;
			}
		}

		public override string StringIdentity
		{
			get
			{
				return this.connector.DistinguishedName;
			}
		}

		public long MaxMessageSize
		{
			get
			{
				return (long)this.connector.AbsoluteMaxMessageSize;
			}
		}

		public string ConnectorName
		{
			get
			{
				return this.connector.Name;
			}
		}

		public Guid ConnectorGuid
		{
			get
			{
				return this.connector.Guid;
			}
		}

		private bool DnsRoutingEnabled
		{
			get
			{
				SmtpSendConnectorConfig smtpSendConnectorConfig = this.connector as SmtpSendConnectorConfig;
				return smtpSendConnectorConfig != null && smtpSendConnectorConfig.DNSRoutingEnabled;
			}
		}

		public static RoutingDestination GetRoutingDestination(MailRecipient recipient, RoutingContext context)
		{
			string addressType;
			string address;
			string text;
			if (!ConnectorRoutingDestination.TryGetRecipientAddress(recipient, context, out addressType, out address, out text))
			{
				return ConnectorRoutingDestination.invalidAddressNdr;
			}
			ConnectorRoutingDestination connectorRoutingDestination = null;
			switch (context.RoutingTables.ConnectorMap.TryFindBestConnector(addressType, address, context.MessageSize, out connectorRoutingDestination))
			{
			case ConnectorMatchResult.InvalidSmtpAddress:
				return ConnectorRoutingDestination.invalidAddressNdr;
			case ConnectorMatchResult.InvalidX400Address:
				return ConnectorRoutingDestination.invalidX400AddressNdr;
			case ConnectorMatchResult.MaxMessageSizeExceeded:
				return ConnectorRoutingDestination.maxMessageSizeExceededNdr;
			case ConnectorMatchResult.NoAddressMatch:
				if (!RoutingUtils.IsSmtpAddressType(addressType))
				{
					return ConnectorRoutingDestination.noMatchingConnectorNdr;
				}
				return ConnectorRoutingDestination.noMatchingConnectorUnreachable;
			}
			if (connectorRoutingDestination.DnsRoutingEnabled && text.IndexOf(RoutingDomain.Separator) != -1)
			{
				return ConnectorRoutingDestination.dnsDomainUnreachable;
			}
			return connectorRoutingDestination;
		}

		public static bool TryGetRecipientAddress(MailRecipient recipient, RoutingContext context, out string addressType, out string address, out string dnsDomain)
		{
			addressType = null;
			address = null;
			dnsDomain = null;
			if (recipient.RoutingOverride != null)
			{
				RoutingDomain routingDomain = recipient.RoutingOverride.RoutingDomain;
				address = routingDomain.Domain;
				addressType = routingDomain.Type;
				switch (recipient.RoutingOverride.DeliveryQueueDomain)
				{
				case DeliveryQueueDomain.UseOverrideDomain:
					if (routingDomain.IsSmtp())
					{
						dnsDomain = address;
					}
					else
					{
						dnsDomain = routingDomain.ToString();
					}
					break;
				case DeliveryQueueDomain.UseRecipientDomain:
					dnsDomain = recipient.Email.DomainPart;
					break;
				case DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts:
					dnsDomain = recipient.RoutingOverride.AlternateDeliveryRoutingHostsString;
					break;
				default:
					throw new InvalidOperationException("Unexpected DeliveryQueueDomain value: " + recipient.RoutingOverride.DeliveryQueueDomain);
				}
			}
			else
			{
				if (!context.Core.IsEdgeMode)
				{
					ProxyAddress proxyAddress;
					if (context.Core.Dependencies.TryDeencapsulate(recipient.Email, out proxyAddress))
					{
						addressType = proxyAddress.Prefix.PrimaryPrefix;
						address = proxyAddress.AddressString;
						dnsDomain = recipient.Email.DomainPart;
						RoutingDiag.Tracer.TraceDebug(0L, "[{0}] De-encapsulated address '{1}' into '{2}:{3}'", new object[]
						{
							context.Timestamp,
							recipient.Email.ToString(),
							addressType,
							address
						});
					}
					else if (proxyAddress is InvalidProxyAddress)
					{
						RoutingDiag.Tracer.TraceError<DateTime, string, ProxyAddress>(0L, "[{0}] Cannot route to encapsulated recipient address {1} because inner address <{2}> is invalid", context.Timestamp, recipient.Email.ToString(), proxyAddress);
						return false;
					}
				}
				if (string.IsNullOrEmpty(address))
				{
					addressType = "smtp";
					address = recipient.Email.DomainPart;
					dnsDomain = address;
				}
			}
			if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(dnsDomain))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>(0L, "[{0}] Cannot route to recipient with null or empty domain {1}", context.Timestamp, recipient.Email.ToString());
				return false;
			}
			return true;
		}

		public static bool TryGetNextHop(NextHopSolutionKey nextHopKey, RoutingTables routingTables, out RoutingNextHop nextHop)
		{
			if (!routingTables.ConnectorMap.TryGetConnectorNextHop(nextHopKey.NextHopConnector, out nextHop))
			{
				RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] No connector next hop for next hop key <{1}>", routingTables.WhenCreated, nextHopKey);
				return false;
			}
			if (nextHopKey.NextHopType.DeliveryType != nextHop.DeliveryType)
			{
				RoutingDiag.Tracer.TraceError<DateTime, DeliveryType, NextHopSolutionKey>(0L, "[{0}] Delivery Type mismatch between connector {1} and next hop key <{2}>", routingTables.WhenCreated, nextHop.DeliveryType, nextHopKey);
				nextHop = null;
				return false;
			}
			return true;
		}

		public bool Match(ConnectorRoutingDestination other)
		{
			return this.ConnectorGuid == other.ConnectorGuid && this.MaxMessageSize == other.MaxMessageSize && base.RouteInfo.Match(other.RouteInfo, NextHopMatch.Full);
		}

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination noMatchingConnectorUnreachable = new ConnectorRoutingDestination.NoMatchingConnectorDestination(UnreachableNextHop.NoMatchingConnector);

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination noMatchingConnectorNdr = new ConnectorRoutingDestination.NoMatchingConnectorDestination(NdrNextHop.NoConnectorForAddressType);

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination invalidAddressNdr = new ConnectorRoutingDestination.NoMatchingConnectorDestination(NdrNextHop.InvalidAddressForRouting);

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination invalidX400AddressNdr = new ConnectorRoutingDestination.NoMatchingConnectorDestination(NdrNextHop.InvalidX400AddressForRouting);

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination maxMessageSizeExceededNdr = new ConnectorRoutingDestination.NoMatchingConnectorDestination(NdrNextHop.MessageTooLargeForRoute);

		private static ConnectorRoutingDestination.NoMatchingConnectorDestination dnsDomainUnreachable = new ConnectorRoutingDestination.NoMatchingConnectorDestination(UnreachableNextHop.IncompatibleDeliveryDomain);

		private MailGateway connector;

		private IList<AddressSpace> addressSpaces;

		private class NoMatchingConnectorDestination : UnroutableDestination
		{
			public NoMatchingConnectorDestination(RoutingNextHop nextHop) : base(MailRecipientType.External, "<No Matching Connector>", nextHop)
			{
			}
		}
	}
}
