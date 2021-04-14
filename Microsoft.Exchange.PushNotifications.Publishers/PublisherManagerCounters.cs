using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class PublisherManagerCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PublisherManagerCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PublisherManagerCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Push Notifications Publisher Manager";

		public static readonly ExPerformanceCounter TotalPushNotificationRequests = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total PushNotification Requests - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInvalidPushNotifications = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Invalid PushNotifications - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDiscardedPushNotifications = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Discarded PushNotifications - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNotificationRequests = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Notification Requests - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInvalidNotifications = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Invalid Notifications - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDiscardedNotifications = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Discarded Notifications - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMulticastNotificationRequests = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Multicast Notification Requests - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInvalidMulticastNotifications = new ExPerformanceCounter("MSExchange Push Notifications Publisher Manager", "Total Invalid Multicast Notifications - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PublisherManagerCounters.TotalPushNotificationRequests,
			PublisherManagerCounters.TotalInvalidPushNotifications,
			PublisherManagerCounters.TotalDiscardedPushNotifications,
			PublisherManagerCounters.TotalNotificationRequests,
			PublisherManagerCounters.TotalInvalidNotifications,
			PublisherManagerCounters.TotalDiscardedNotifications,
			PublisherManagerCounters.TotalMulticastNotificationRequests,
			PublisherManagerCounters.TotalInvalidMulticastNotifications
		};
	}
}
