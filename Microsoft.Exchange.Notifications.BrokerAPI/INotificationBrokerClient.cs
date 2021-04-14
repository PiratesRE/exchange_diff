using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal interface INotificationBrokerClient : IDisposable
	{
		void Subscribe(BrokerSubscription subscription);

		void Unsubscribe(BrokerSubscription subscription);

		void StartNotificationCallbacks(Action<BrokerNotification> notificationCallback);

		void StopNotificationCallbacks();
	}
}
