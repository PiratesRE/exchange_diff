using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PendingGetChannel : PushNotificationChannel<PendingGetNotification>
	{
		public PendingGetChannel(string appId, IPendingGetConnectionCache connectionCache, ITracer tracer) : base(appId, tracer)
		{
			this.connectionCache = connectionCache;
		}

		public override void Send(PendingGetNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			IPendingGetConnection pendingGetConnection = null;
			if (!this.connectionCache.TryGetConnection(notification.SubscriptionId, out pendingGetConnection))
			{
				PushNotificationsCrimsonEvents.PendingGetConnectionUnavailable.Log<string, string>(notification.Identifier, notification.SubscriptionId);
				base.Tracer.TraceDebug<PendingGetNotification>((long)this.GetHashCode(), "[Send] Skip to send notification '{0}' because no connection was found in the cache.", notification);
				return;
			}
			if (pendingGetConnection.FireUnseenEmailNotification(notification.Payload.EmailCount.Value, notification.SequenceNumber))
			{
				base.Tracer.TraceDebug<PendingGetNotification>((long)this.GetHashCode(), "[Send] Successfully sent notification '{0}'.", notification);
				PushNotificationTracker.ReportSent(notification, PushNotificationPlatform.None);
				return;
			}
			base.Tracer.TraceDebug<PendingGetNotification>((long)this.GetHashCode(), "[Send] Skip to send notification '{0}' because the pending get connection is not available yet.", notification);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PendingGetChannel>(this);
		}

		private IPendingGetConnectionCache connectionCache;
	}
}
