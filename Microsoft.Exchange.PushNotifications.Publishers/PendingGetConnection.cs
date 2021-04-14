using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PendingGetConnection : IPendingGetConnection
	{
		public PendingGetConnection(string connectionId)
		{
			this.connectionId = connectionId;
			this.latestUnseenEmailNotification = new PendingGetConnection.UnseenEmailNotification();
		}

		public void SubscribeToUnseenEmailNotification(PendingGetContext pendingGetContext, long timeoutInMilliseconds, int latestUnseenEmailNotificationId)
		{
			PendingGetConnection.UnseenEmailNotification unseenEmailNotification = this.latestUnseenEmailNotification;
			if (unseenEmailNotification.NotificationId > 0 && latestUnseenEmailNotificationId != unseenEmailNotification.NotificationId)
			{
				ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[SubscribeToUnseenEmailNotification] The latest Unseen Email notification did not deliver to client for PendingGet channel - '{0}'. Send it again as the client reconnects. The latest notification id in client is '{1}', the latest notification id in server is '{2}'", this.connectionId, latestUnseenEmailNotificationId, unseenEmailNotification.NotificationId);
				PushNotificationsCrimsonEvents.PendingGetPickupLostNotification.Log<string, int, int>(this.connectionId, latestUnseenEmailNotificationId, unseenEmailNotification.NotificationId);
				pendingGetContext.Response.Write(unseenEmailNotification.GetPayload(true));
				pendingGetContext.AsyncResult.CompletedSynchronously = true;
				pendingGetContext.AsyncResult.IsCompleted = true;
				return;
			}
			PendingGetContext pendingGetContext2 = Interlocked.Exchange<PendingGetContext>(ref this.context, pendingGetContext);
			if (pendingGetContext2 != null)
			{
				ExTraceGlobals.PendingGetPublisherTracer.TraceWarning<string>((long)this.GetHashCode(), "[SubscribeToUnseenEmailNotification] Client reconnects again while there is still connection on server for PendingGet channel - '{0}'. This means previous connection was dropped without server's awareness. Finish the previous request to cleanup.", this.connectionId);
				PushNotificationsCrimsonEvents.PendingGetCloseLostConnection.Log<string>(this.connectionId);
				pendingGetContext2.AsyncResult.IsCompleted = true;
			}
			ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<long, string>((long)this.GetHashCode(), "[SubscribeToUnseenEmailNotification] Latest Unseen Email notification id matches. Setup a timer in {0} milliseconds to timeout the connection for PendingGet channel - '{1}'.", timeoutInMilliseconds, this.connectionId);
			this.SetupTimer(timeoutInMilliseconds);
		}

		public bool FireUnseenEmailNotification(int unseenCount, int notificationId)
		{
			this.DisableTimer();
			ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[FireUnseenEmailNotification] PendingGet channel - '{0}' receives Unseen Email notification - unseenCount = '{1}', notificationId = '{2}'.", this.connectionId, unseenCount, notificationId);
			PendingGetConnection.UnseenEmailNotification unseenEmailNotification = new PendingGetConnection.UnseenEmailNotification
			{
				NotificationId = notificationId,
				UnseenEmailCount = unseenCount
			};
			this.latestUnseenEmailNotification = unseenEmailNotification;
			PendingGetContext pendingGetContext = Interlocked.Exchange<PendingGetContext>(ref this.context, null);
			if (pendingGetContext != null)
			{
				ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[FireUnseenEmailNotification] Found the connection for PendingGet channel '{0}' and writes the notification to the client - unseenCount = '{1}', notificationId = '{2}'.", this.connectionId, unseenCount, notificationId);
				pendingGetContext.Response.Write(unseenEmailNotification.GetPayload(false));
				pendingGetContext.AsyncResult.IsCompleted = true;
				return true;
			}
			ExTraceGlobals.PendingGetPublisherTracer.TraceWarning<string, int, int>((long)this.GetHashCode(), "[FireUnseenEmailNotification] PendingGet connection is not available for for channel '{0}'. Will try to deliver the notification (unseenCount = '{1}', notificationId = '{2}') once client connects again.", this.connectionId, unseenCount, notificationId);
			PushNotificationsCrimsonEvents.PendingGetArchiveNotifiction.Log<string, int, int>(this.connectionId, unseenCount, notificationId);
			return false;
		}

		private void ElapsedConnectionTimeout(object state)
		{
			this.DisableTimer();
			PendingGetContext pendingGetContext = Interlocked.Exchange<PendingGetContext>(ref this.context, null);
			if (pendingGetContext != null)
			{
				ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string>((long)this.GetHashCode(), "[ElapsedConnectionTimeout] PendingGet connection times out in server. Complete the request for PendingGet channel '{0}'.", this.connectionId);
				pendingGetContext.AsyncResult.IsCompleted = true;
			}
		}

		private void SetupTimer(long timeoutInMilliseconds)
		{
			Timer value = new Timer(new TimerCallback(this.ElapsedConnectionTimeout), null, timeoutInMilliseconds, timeoutInMilliseconds);
			Interlocked.Exchange<Timer>(ref this.pendingRequestTimeoutTimer, value);
		}

		private void DisableTimer()
		{
			Timer timer = Interlocked.Exchange<Timer>(ref this.pendingRequestTimeoutTimer, null);
			if (timer != null)
			{
				timer.Change(-1, -1);
			}
		}

		private readonly string connectionId;

		private PendingGetContext context;

		private PendingGetConnection.UnseenEmailNotification latestUnseenEmailNotification;

		private Timer pendingRequestTimeoutTimer;

		private class UnseenEmailNotification
		{
			public int NotificationId { get; set; }

			public int UnseenEmailCount { get; set; }

			public string GetPayload(bool isCached)
			{
				return string.Format("{{\"type\":\"UnseenEmail\",\"id\":{0},\"UnseenCount\":{1},\"IsCached\":{2}}}", this.NotificationId, this.UnseenEmailCount, isCached);
			}

			private const string UnseenEmailPayloadTemplate = "{{\"type\":\"UnseenEmail\",\"id\":{0},\"UnseenCount\":{1},\"IsCached\":{2}}}";
		}
	}
}
