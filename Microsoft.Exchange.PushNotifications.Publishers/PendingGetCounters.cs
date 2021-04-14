using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class PendingGetCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PendingGetCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PendingGetCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Push Notifications Pending Get";

		public static readonly ExPerformanceCounter TryGetConnectionAverageTime = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Try get Connection - Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TryGetConnectionAverageTimeBase = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Try get Connection - Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionCachedAverageTime = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Connection Cached - Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionCachedAverageTimeBase = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Connection Cached - Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AddNewConnectionAverageTime = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Add New Connection - Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AddNewConnectionAverageTimeBase = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Add New Connection - Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PendingGetConnectionCacheCount = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Pending Get Connection Cache - Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PendingGetConnectionCachePeak = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Pending Get Connection Cache - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PendingGetConnectionCacheTotal = new ExPerformanceCounter("MSExchange Push Notifications Pending Get", "Pending Get Connection Cache - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PendingGetCounters.TryGetConnectionAverageTime,
			PendingGetCounters.TryGetConnectionAverageTimeBase,
			PendingGetCounters.ConnectionCachedAverageTime,
			PendingGetCounters.ConnectionCachedAverageTimeBase,
			PendingGetCounters.AddNewConnectionAverageTime,
			PendingGetCounters.AddNewConnectionAverageTimeBase,
			PendingGetCounters.PendingGetConnectionCacheCount,
			PendingGetCounters.PendingGetConnectionCachePeak,
			PendingGetCounters.PendingGetConnectionCacheTotal
		};
	}
}
