using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ConnectionDroppedNotificationHandler : MapiNotificationHandlerBase
	{
		public ConnectionDroppedNotificationHandler(IMailboxContext userContext) : base(userContext, false)
		{
			ConnectionDroppedNotifier connectionDroppedNotifier = new ConnectionDroppedNotifier(userContext);
			connectionDroppedNotifier.RegisterWithPendingRequestNotifier();
		}

		internal event ConnectionDroppedNotificationHandler.ConnectionDroppedEventHandler OnConnectionDropped;

		internal override void HandleNotificationInternal(Notification notification, MapiNotificationsLogEvent logEvent, object context)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.HandleNotification Begin.");
			lock (base.SyncRoot)
			{
				base.NeedToReinitSubscriptions = true;
			}
			if (this.OnConnectionDropped != null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.HandleNotification Call OnConnectionDropped.");
				this.OnConnectionDropped(notification);
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			bool flag = false;
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.HandlePendingGetTimerCallback Begin.");
			lock (base.SyncRoot)
			{
				flag = base.NeedToReinitSubscriptions;
			}
			if (flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.HandlePendingGetTimerCallback Need to reinit true.");
				try
				{
					lock (base.SyncRoot)
					{
						if (base.NeedToReinitSubscriptions)
						{
							base.InitSubscription();
							ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.HandlePendingGetTimerCallback Successful reinit .");
						}
					}
				}
				catch (Exception)
				{
					base.NeedToReinitSubscriptions = true;
					throw;
				}
			}
		}

		protected override void InitSubscriptionInternal()
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.InitSubscriptionInternal Begin.");
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			lock (base.SyncRoot)
			{
				if (base.Subscription != null)
				{
					throw new InvalidOperationException("There is an existing undisposed subscription. Dispose it before creating a new one");
				}
				base.Subscription = Subscription.CreateMailboxSubscription(base.UserContext.MailboxSession, new NotificationHandler(base.HandleNotification), NotificationType.ConnectionDropped);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ConnectionDroppedEventHandler.InitSubscriptionInternal Subscription created.");
			}
		}

		internal delegate void ConnectionDroppedEventHandler(Notification notification);
	}
}
