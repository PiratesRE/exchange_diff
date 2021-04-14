using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public delegate void SubscriptionEnumerationCallback(NotificationPublishPhase phase, Context transactionContext, NotificationSubscription subscription, NotificationEvent nev);
}
