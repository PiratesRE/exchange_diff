using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal class EventNotificationItemWrapper : IEventNotificationItem
	{
		public void Publish(string serviceName, string component, string tag, string notificationReason, ResultSeverityLevel severity, bool throwOnError)
		{
			EventNotificationItem.Publish(serviceName, component, tag, notificationReason, severity, throwOnError);
		}

		public void Publish(string serviceName, string component, string tag, string notificationReason, string stateAttribute1, ResultSeverityLevel severity, bool throwOnError)
		{
			EventNotificationItem.Publish(serviceName, component, tag, notificationReason, stateAttribute1, severity, throwOnError);
		}

		public void PublishPeriodic(string serviceName, string component, string tag, string notificationReason, string periodicKey, TimeSpan period, ResultSeverityLevel severity, bool throwOnError)
		{
			EventNotificationItem.PublishPeriodic(serviceName, component, tag, notificationReason, periodicKey, period, severity, throwOnError);
		}
	}
}
