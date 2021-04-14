using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class CallRouterAvailabilityCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (CallRouterAvailabilityCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in CallRouterAvailabilityCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeUMCallRouterAvailability";

		public static readonly ExPerformanceCounter RecentMissedCallNotificationProxyFailed = new ExPerformanceCounter("MSExchangeUMCallRouterAvailability", "% of Missed Call Notification Proxy Failed at UM Call Router Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMCallRouterCallsRejected = new ExPerformanceCounter("MSExchangeUMCallRouterAvailability", "Total Inbound Calls Rejected by the UM Call Router", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMCallRouterCallsReceived = new ExPerformanceCounter("MSExchangeUMCallRouterAvailability", "Total Inbound Calls Received by the UM Call Router", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecentUMCallRouterCallsRejected = new ExPerformanceCounter("MSExchangeUMCallRouterAvailability", "% of Inbound Calls Rejected by the UM Call Router Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			CallRouterAvailabilityCounters.RecentMissedCallNotificationProxyFailed,
			CallRouterAvailabilityCounters.UMCallRouterCallsRejected,
			CallRouterAvailabilityCounters.UMCallRouterCallsReceived,
			CallRouterAvailabilityCounters.RecentUMCallRouterCallsRejected
		};
	}
}
