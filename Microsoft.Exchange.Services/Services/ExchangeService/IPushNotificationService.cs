using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal interface IPushNotificationService : IDisposable
	{
		void EnqueuePendingOutlookPushNotification(ExchangeServiceTraceDelegate traceDelegate, IOutlookPushNotificationSubscriptionCache subscriptionCache, IOutlookPushNotificationSerializer serializer, string notificationContext, PendingOutlookPushNotification notification);
	}
}
