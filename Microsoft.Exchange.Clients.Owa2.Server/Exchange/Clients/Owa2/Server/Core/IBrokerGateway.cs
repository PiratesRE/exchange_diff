using System;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IBrokerGateway
	{
		void Subscribe(BrokerSubscription brokerSubscription, BrokerHandler handler);

		void Unsubscribe(BrokerSubscription brokerSubscription);
	}
}
