using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RedirectDestination : RoutingDestination
	{
		private RedirectDestination(RouteInfo primaryRoute, RoutedServerCollection routedServerCollection) : base(primaryRoute)
		{
			RoutingUtils.ThrowIfNull(routedServerCollection, "routedServerCollection");
			this.redirectGroup = new RedirectGroup(routedServerCollection);
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			string format = "Redirect={0}";
			object[] array = new object[1];
			array[0] = string.Join(";", from server in routedServerCollection.AllServers
			select server.Fqdn);
			this.stringIdentity = string.Format(invariantCulture, format, array);
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
				return this.stringIdentity;
			}
		}

		public static RoutingDestination GetRoutingDestination(List<string> targetHostFqdns, RoutingContext context)
		{
			RoutingUtils.ThrowIfNull(targetHostFqdns, "targetHostFqdns");
			RoutedServerCollection routedServerCollection;
			List<string> list;
			List<RoutingServerInfo> list2;
			bool flag = context.RoutingTables.ServerMap.TryCreateRoutedServerCollection(targetHostFqdns, context.Core, out routedServerCollection, out list, out list2);
			if (list != null && list.Count > 0)
			{
				RoutingDiag.Tracer.TraceWarning<DateTime, string, string>(0L, "[{0}] Unknown servers specified as redirect hosts <{1}> for recipient {2}", context.Timestamp, string.Join(", ", targetHostFqdns), context.CurrentRecipient.Email.ToString());
			}
			if (list2 != null && list2.Count > 0)
			{
				RoutingDiag.Tracer.TraceWarning<DateTime, string, string>(0L, "[{0}] Unreachable servers specified as redirect hosts <{1}> for recipient {2}", context.Timestamp, string.Join<RoutingServerInfo>(", ", list2), context.CurrentRecipient.Email.ToString());
			}
			if (flag)
			{
				return new RedirectDestination(routedServerCollection.PrimaryRoute, routedServerCollection);
			}
			RoutingDiag.Tracer.TraceError<DateTime, string, string>(0L, "[{0}] No server information for redirect hosts <{1}> for recipient {2}; the message will get into the unreachable queue.", context.Timestamp, string.Join(", ", targetHostFqdns), context.CurrentRecipient.Email.ToString());
			if (list2 == null || list2.Count <= 0)
			{
				return RedirectDestination.UnknownServer;
			}
			return RedirectDestination.NoRouteToServer;
		}

		public override RoutingNextHop GetNextHop(RoutingContext context)
		{
			return this.redirectGroup;
		}

		private static readonly UnroutableDestination NoRouteToServer = new UnroutableDestination(MailRecipientType.DistributionGroup, "<No Route to Redirection Server>", UnreachableNextHop.NoRouteToServer);

		private static readonly UnroutableDestination UnknownServer = new UnroutableDestination(MailRecipientType.DistributionGroup, "<Unknown Redirection Server>", UnreachableNextHop.NonHubExpansionServer);

		private readonly RedirectGroup redirectGroup;

		private readonly string stringIdentity;
	}
}
