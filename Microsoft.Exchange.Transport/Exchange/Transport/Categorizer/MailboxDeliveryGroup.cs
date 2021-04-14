using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailboxDeliveryGroup : MailboxDeliveryGroupBase
	{
		public MailboxDeliveryGroup(MailboxDeliveryGroupId id, RouteInfo siteRouteInfo, RoutingServerInfo serverInfo, bool isLocalDeliveryGroup, RoutingContextCore contextCore) : base(new RoutedServerCollection(siteRouteInfo, serverInfo, contextCore), DeliveryType.SmtpRelayToMailboxDeliveryGroup, id.ToString(), Guid.Empty, serverInfo.MajorVersion, isLocalDeliveryGroup)
		{
		}

		public void AddHubServer(RoutingServerInfo server, RoutingContextCore contextCore)
		{
			base.AddServerInternal(base.RoutedServerCollection.PrimaryRoute, server, contextCore);
		}
	}
}
