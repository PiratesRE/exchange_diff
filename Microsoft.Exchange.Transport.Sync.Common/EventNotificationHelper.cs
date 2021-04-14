using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EventNotificationHelper
	{
		public static void PublishTransportEventNotificationItem(string notificationEvent)
		{
			EventNotificationHelper.PublishEventNotificationItem(notificationEvent, ExchangeComponent.Transport.Name, null);
		}

		public static void PublishTransportEventNotificationItem(string notificationEvent, Exception exception)
		{
			EventNotificationHelper.PublishEventNotificationItem(notificationEvent, ExchangeComponent.Transport.Name, exception);
		}

		public static void PublishTransportSyncEventNotificationItem(string notificationEvent)
		{
			EventNotificationHelper.PublishEventNotificationItem(notificationEvent, ExchangeComponent.MailboxMigration.Name, null);
		}

		public static void PublishTransportSyncEventNotificationItem(string notificationEvent, Exception exception)
		{
			EventNotificationHelper.PublishEventNotificationItem(notificationEvent, ExchangeComponent.MailboxMigration.Name, exception);
		}

		private static void PublishEventNotificationItem(string notificationEvent, string componentName, Exception exception)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(componentName, notificationEvent, string.Empty, ResultSeverityLevel.Error);
			if (exception != null)
			{
				eventNotificationItem.AddCustomProperty("Exception", exception.ToString());
			}
			eventNotificationItem.Publish(false);
		}
	}
}
