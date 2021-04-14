using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal interface IEventNotificationItem
	{
		void Publish(string serviceName, string component, string tag, string notificationReason, ResultSeverityLevel severity, bool throwOnError);

		void Publish(string serviceName, string component, string tag, string notificationReason, string stateAttribute1, ResultSeverityLevel severity, bool throwOnError);

		void PublishPeriodic(string serviceName, string component, string tag, string notificationReason, string periodicKey, TimeSpan period, ResultSeverityLevel severity, bool throwOnError);
	}
}
