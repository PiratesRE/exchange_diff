using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class EventNotificationItem : NotificationItem
	{
		public EventNotificationItem(string serviceName, string component, string notificationReason, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, notificationReason, severity)
		{
		}

		public EventNotificationItem(string serviceName, string component, string notificationReason, string message, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, message, severity)
		{
		}

		public EventNotificationItem(string serviceName, string component, string notificationReason, string message, string stateAttribute1, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, message, stateAttribute1, severity)
		{
		}

		public static void Publish(string serviceName, string component, string tag, string notificationReason, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(component))
			{
				throw new ArgumentException("serviceName and component must have non-null/non-empty values.");
			}
			NotificationItem notificationItem = new EventNotificationItem(serviceName, component, tag, notificationReason, severity);
			notificationItem.Publish(throwOnError);
		}

		public static void Publish(string serviceName, string component, string tag, string notificationReason, string stateAttribute1, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(component))
			{
				throw new ArgumentException("serviceName and component must have non-null/non-empty values.");
			}
			NotificationItem notificationItem = new EventNotificationItem(serviceName, component, tag, notificationReason, stateAttribute1, severity);
			notificationItem.Publish(throwOnError);
		}

		public static void PublishPeriodic(string serviceName, string component, string tag, string notificationReason, string periodicKey, TimeSpan period, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (string.IsNullOrEmpty(periodicKey))
			{
				throw new ArgumentException("periodicKey must have non-null/non-empty value.");
			}
			if (EventNotificationItem.CanPublishPeriodic(periodicKey, period))
			{
				EventNotificationItem.Publish(serviceName, component, tag, notificationReason, severity, throwOnError);
			}
		}

		private static bool CanPublishPeriodic(string periodicKey, TimeSpan period)
		{
			bool result;
			lock (EventNotificationItem.periodicEventDictionary)
			{
				DateTime d;
				if (!EventNotificationItem.periodicEventDictionary.TryGetValue(periodicKey, out d) || DateTime.UtcNow > d + period)
				{
					EventNotificationItem.periodicEventDictionary[periodicKey] = DateTime.UtcNow;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static readonly Dictionary<string, DateTime> periodicEventDictionary = new Dictionary<string, DateTime>();
	}
}
