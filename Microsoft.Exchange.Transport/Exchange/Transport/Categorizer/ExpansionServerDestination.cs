using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ExpansionServerDestination : RoutingDestination
	{
		private ExpansionServerDestination(ADObjectId serverId, RouteInfo routeInfo) : base(routeInfo)
		{
			this.serverId = serverId;
		}

		public override MailRecipientType DestinationType
		{
			get
			{
				return MailRecipientType.DistributionGroup;
			}
		}

		public override string StringIdentity
		{
			get
			{
				return this.serverId.DistinguishedName;
			}
		}

		public static RoutingDestination GetRoutingDestination(ADObjectId serverId, RoutingContext context)
		{
			if (serverId == null)
			{
				throw new ArgumentNullException("serverId", "Expansion Server cannot be null for Distribution Group recipients that reach Routing");
			}
			RoutingUtils.ThrowIfNullOrEmpty(serverId.DistinguishedName, "serverId.DistinguishedName");
			RoutingServerInfo routingServerInfo;
			RouteInfo routeInfo;
			if (!context.RoutingTables.ServerMap.TryGetServerRouteByDN(serverId.DistinguishedName, out routingServerInfo, out routeInfo))
			{
				if (routingServerInfo == null)
				{
					RoutingDiag.Tracer.TraceError<DateTime, string, string>(0L, "[{0}] No server information for DG expansion server <{1}> for recipient {2}; the recipient will be placed in Unreachable queue", context.Timestamp, serverId.DistinguishedName, context.CurrentRecipient.Email.ToString());
				}
				else
				{
					RoutingDiag.Tracer.TraceError<DateTime, string, string>(0L, "[{0}] No route to DG expansion server <{1}> for recipient {2}; the recipient will be placed in Unreachable queue", context.Timestamp, serverId.DistinguishedName, context.CurrentRecipient.Email.ToString());
				}
				return ExpansionServerDestination.NoRouteToServer;
			}
			if (routingServerInfo.IsExchange2007OrLater && !routingServerInfo.IsHubTransportServer)
			{
				RoutingDiag.Tracer.TraceError<DateTime, string, string>(0L, "[{0}] DG expansion server <{1}> for recipient {2} is not a Hub Transport server; the recipient will be placed in Unreachable queue", context.Timestamp, serverId.DistinguishedName, context.CurrentRecipient.Email.ToString());
				return ExpansionServerDestination.NonHubExpansionServer;
			}
			if (routeInfo.DestinationProximity == Proximity.LocalServer)
			{
				throw new InvalidOperationException("DG with local expansion server reached routing: " + context.CurrentRecipient.Email.ToString());
			}
			if (!routeInfo.HasMandatoryTopologyHop && (routeInfo.DestinationProximity != Proximity.RemoteADSite || context.Core.Settings.DestinationRoutingToRemoteSitesEnabled))
			{
				routeInfo = routeInfo.ReplaceNextHop(new RedirectGroup(new RoutedServerCollection(routeInfo, routingServerInfo, context.Core)), context.CurrentRecipient.Email.ToString());
			}
			return new ExpansionServerDestination(serverId, routeInfo);
		}

		private static readonly UnroutableDestination NoRouteToServer = new UnroutableDestination(MailRecipientType.DistributionGroup, "<No Route To Expansion Server>", UnreachableNextHop.NoRouteToServer);

		private static readonly UnroutableDestination NonHubExpansionServer = new UnroutableDestination(MailRecipientType.DistributionGroup, "<Non-Hub Expansion Server>", UnreachableNextHop.NonHubExpansionServer);

		private ADObjectId serverId;
	}
}
