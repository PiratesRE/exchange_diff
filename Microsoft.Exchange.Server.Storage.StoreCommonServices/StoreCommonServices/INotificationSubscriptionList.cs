using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface INotificationSubscriptionList
	{
		void RegisterSubscription(NotificationSubscription subscription);

		void UnregisterSubscription(NotificationSubscription subscription);

		void EnumerateSubscriptionsForEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev, SubscriptionEnumerationCallback callback);
	}
}
