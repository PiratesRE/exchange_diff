using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class ProxyCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ProxyCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ProxyCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Push Notifications Proxy";

		public static readonly ExPerformanceCounter AveragePublishingTime = new ExPerformanceCounter("MSExchange Push Notifications Proxy", "Publishing - Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragePublishingTimeBase = new ExPerformanceCounter("MSExchange Push Notifications Proxy", "Publishing - Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNotificationBatchSize = new ExPerformanceCounter("MSExchange Push Notifications Proxy", "Average Notification Batch Size - Average Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNotificationBatchSizeBase = new ExPerformanceCounter("MSExchange Push Notifications Proxy", "Average Notification Batch Size - Count Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ProxyCounters.AveragePublishingTime,
			ProxyCounters.AveragePublishingTimeBase,
			ProxyCounters.AverageNotificationBatchSize,
			ProxyCounters.AverageNotificationBatchSizeBase
		};
	}
}
