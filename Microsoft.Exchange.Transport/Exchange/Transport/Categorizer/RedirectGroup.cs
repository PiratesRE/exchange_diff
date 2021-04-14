using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RedirectGroup : ServerCollectionDeliveryGroup
	{
		public RedirectGroup(RoutedServerCollection routedServerCollection) : base(routedServerCollection)
		{
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return DeliveryType.SmtpRelayToServers;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public override string Name
		{
			get
			{
				return "RedirectGroup";
			}
		}

		public static bool TryGetNextHop(NextHopSolutionKey nextHopKey, RoutingTables routingTables, RoutingContextCore contextCore, out RoutingNextHop nextHop)
		{
			nextHop = null;
			RoutedServerCollection routedServerCollection;
			List<string> list;
			List<RoutingServerInfo> list2;
			if (!routingTables.ServerMap.TryCreateRoutedServerCollection(nextHopKey.NextHopDomain.Split(new char[]
			{
				';'
			}), contextCore, out routedServerCollection, out list, out list2))
			{
				RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] No server information for next hop key <{1}>", routingTables.WhenCreated, nextHopKey);
				return false;
			}
			if (!routedServerCollection.AllServers.All((RoutingServerInfo server) => server.IsHubTransportServer))
			{
				RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] Target server is not a Hub Transport server for next hop key <{1}>", routingTables.WhenCreated, nextHopKey);
				return false;
			}
			nextHop = new RedirectGroup(routedServerCollection);
			return true;
		}

		public override string GetNextHopDomain(RoutingContext context)
		{
			return string.Join(";", from server in base.RoutedServerCollection.AllServers
			select server.Fqdn);
		}

		public override bool Match(RoutingNextHop other)
		{
			throw new NotSupportedException("RedirectGroup is an on-demand-created delivery group that is not expected to be in Routing Tables; hence, it does not support the Match() operation.");
		}
	}
}
