using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class OutlookPushNotificationService : DisposeTrackableBase, IPushNotificationService, IDisposable
	{
		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OutlookPushNotificationService>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		public void EnqueuePendingOutlookPushNotification(ExchangeServiceTraceDelegate traceDelegate, IOutlookPushNotificationSubscriptionCache subscriptionCache, IOutlookPushNotificationSerializer serializer, string notificationContext, PendingOutlookPushNotification notification)
		{
			OutlookPushNotificationBatchManager.GetInstance(traceDelegate, subscriptionCache, serializer).Add(notificationContext, notification);
		}
	}
}
