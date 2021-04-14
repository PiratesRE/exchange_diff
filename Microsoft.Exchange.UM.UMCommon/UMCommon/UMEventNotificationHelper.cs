using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMEventNotificationHelper
	{
		public static void PublishUMSuccessEventNotificationItem(Component exchangeComponent, string notificationEvent)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(exchangeComponent.Name, notificationEvent, string.Empty, string.Empty, ResultSeverityLevel.Informational);
			eventNotificationItem.Publish(false);
		}

		public static void PublishUMFailureEventNotificationItem(Component exchangeComponent, string notificationEvent)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(exchangeComponent.Name, notificationEvent, string.Empty, ResultSeverityLevel.Error);
			eventNotificationItem.Publish(false);
		}
	}
}
