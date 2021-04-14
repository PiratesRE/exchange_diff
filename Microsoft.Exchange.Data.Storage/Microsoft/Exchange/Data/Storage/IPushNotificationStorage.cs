using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPushNotificationStorage : IDisposable
	{
		string TenantId { get; }

		List<PushNotificationServerSubscription> GetActiveNotificationSubscriptions(IMailboxSession mailboxSession, uint expirationInHours);

		List<StoreObjectId> GetExpiredNotificationSubscriptions(uint expirationInHours);

		List<PushNotificationServerSubscription> GetNotificationSubscriptions(IMailboxSession mailboxSession);

		IPushNotificationSubscriptionItem CreateOrUpdateSubscriptionItem(IMailboxSession mailboxSession, string subscriptionId, PushNotificationServerSubscription subscription);

		void DeleteAllSubscriptions();

		void DeleteExpiredSubscriptions(uint expirationInHours);

		void DeleteSubscription(StoreObjectId itemId);

		void DeleteSubscription(string subscriptionId);
	}
}
