using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class EventNotificationItem
	{
		public static void Publish(string serviceName, string component, string tag, string notificationReason, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			EventNotificationItem.Publish(serviceName, component, tag, notificationReason, (ResultSeverityLevel)severity, throwOnError);
		}
	}
}
