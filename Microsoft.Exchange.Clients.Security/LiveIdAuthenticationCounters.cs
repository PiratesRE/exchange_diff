using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Security
{
	internal static class LiveIdAuthenticationCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (LiveIdAuthenticationCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in LiveIdAuthenticationCounters.AllCounters)
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

		public const string CategoryName = "MSExchange LiveIdAuthentication";

		public static readonly ExPerformanceCounter TotalRetrievalsfromCache = new ExPerformanceCounter("MSExchange LiveIdAuthentication", "Total Retrievals from Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedLookupsfromCache = new ExPerformanceCounter("MSExchange LiveIdAuthentication", "Total Failed Lookups from Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSessionDataPreloadRequestsSent = new ExPerformanceCounter("MSExchange LiveIdAuthentication", "Total session data preload requests sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSessionDataPreloadRequestsFailed = new ExPerformanceCounter("MSExchange LiveIdAuthentication", "Total session data preload requests that failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			LiveIdAuthenticationCounters.TotalRetrievalsfromCache,
			LiveIdAuthenticationCounters.TotalFailedLookupsfromCache,
			LiveIdAuthenticationCounters.TotalSessionDataPreloadRequestsSent,
			LiveIdAuthenticationCounters.TotalSessionDataPreloadRequestsFailed
		};
	}
}
