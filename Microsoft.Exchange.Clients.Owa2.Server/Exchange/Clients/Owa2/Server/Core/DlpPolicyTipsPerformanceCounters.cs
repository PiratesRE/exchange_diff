using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class DlpPolicyTipsPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (DlpPolicyTipsPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in DlpPolicyTipsPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange DlpPolicyTips";

		public static readonly ExPerformanceCounter DlpPolicyTipsTotalRequests = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Total DlpPolicyTips requests processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsPendingRequests = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Current Pending DlpPolicyTips requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsSuccessfulRequests = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Successful DlpPolicyTips requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsAverageRequestLatency = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Average latency of the DlpPolicyTips requests within the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsHighLatencyRequests = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Number of DlpPolicyTips requests with latency more than 1 minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsPercentHighLatency = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Percent of requests with Latency more than 1 minute within the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsSkippedRequestsInputError = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Skipped DlpPolicyTips requests InputError", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsAllServerFailedRequests = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Total server Failed DlpPolicyTips requests received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsPercentServerFailures = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Percent of all server Failed requests within the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsUnknownError = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests UnknownError", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsFips = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Fips exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsFipsTimeOut = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Fips TimeOut exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsEtr = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Etr exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsAd = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Ad exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsXso = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Xso exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DlpPolicyTipsFailedRequestsOws = new ExPerformanceCounter("MSExchange DlpPolicyTips", "Failed DlpPolicyTips requests Ows exceptions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsTotalRequests,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPendingRequests,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsSuccessfulRequests,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsAverageRequestLatency,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsHighLatencyRequests,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPercentHighLatency,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsSkippedRequestsInputError,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsAllServerFailedRequests,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPercentServerFailures,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsUnknownError,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsFips,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsFipsTimeOut,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsEtr,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsAd,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsXso,
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsOws
		};
	}
}
