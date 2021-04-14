using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeServiceCalendarSubscription : ExchangeServiceSubscription
	{
		internal ExchangeServiceCalendarSubscription(string subscriptionId) : base(subscriptionId)
		{
		}

		internal Subscription Subscription { get; set; }

		internal Action<CalendarChangeNotificationType> Callback { get; set; }

		internal QueryResult QueryResult { get; set; }

		internal override void HandleNotification(Notification notification)
		{
			if (notification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Received a null notification for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			if (notification is ConnectionDroppedNotification)
			{
				this.Callback(CalendarChangeNotificationType.ConnectionLost);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Connection dropped, returning notification for reload");
				return;
			}
			QueryNotification queryNotification = notification as QueryNotification;
			if (queryNotification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Received a notification of an unknown type for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Received a {0} notification for subscriptionId: {1}", queryNotification.EventType.ToString(), base.SubscriptionId);
			switch (queryNotification.EventType)
			{
			case QueryNotificationType.RowAdded:
			case QueryNotificationType.RowDeleted:
			case QueryNotificationType.RowModified:
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Calling notification callback for calendar");
				this.Callback(CalendarChangeNotificationType.CalendarChanged);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Returned from callback");
				return;
			default:
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceCalendarSubscription.HandleNotification: Unknown notification event type");
				return;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.Subscription != null)
			{
				this.Subscription.Dispose();
				this.Subscription = null;
			}
			if (this.QueryResult != null)
			{
				this.QueryResult.Dispose();
				this.QueryResult = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeServiceCalendarSubscription>(this);
		}
	}
}
