using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal interface INotificationHandler
	{
		void SubscriptionRemoved(BrokerSubscription subscription);
	}
}
