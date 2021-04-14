using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IOutlookServiceSubscriptionStorage : IDisposable
	{
		string TenantId { get; }

		List<OutlookServiceNotificationSubscription> GetActiveNotificationSubscriptions(string appId, uint deactivationInHours = 72U);

		List<OutlookServiceNotificationSubscription> GetActiveNotificationSubscriptionsForContext(string notificationConext);

		List<string> GetDeactivatedNotificationSubscriptions(string appId, uint deactivationInHours = 72U);

		List<string> GetExpiredNotificationSubscriptions(string appId);

		List<OutlookServiceNotificationSubscription> GetNotificationSubscriptions(string appId);

		OutlookServiceNotificationSubscription CreateOrUpdateSubscriptionItem(OutlookServiceNotificationSubscription subscription);

		void DeleteAllSubscriptions(string appId);

		void DeleteExpiredSubscriptions(string appId);

		void DeleteSubscription(string subscriptionId);
	}
}
