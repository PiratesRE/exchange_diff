using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class AutodiscoverPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (AutodiscoverPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in AutodiscoverPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeAutodiscover";

		private static readonly ExPerformanceCounter TotalRequestsPerSecond = new ExPerformanceCounter("MSExchangeAutodiscover", "Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequests = new ExPerformanceCounter("MSExchangeAutodiscover", "Total Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			AutodiscoverPerformanceCounters.TotalRequestsPerSecond
		});

		private static readonly ExPerformanceCounter ErrorResponsesPerSecond = new ExPerformanceCounter("MSExchangeAutodiscover", "Error Responses/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalErrorResponses = new ExPerformanceCounter("MSExchangeAutodiscover", "Error Responses", string.Empty, null, new ExPerformanceCounter[]
		{
			AutodiscoverPerformanceCounters.ErrorResponsesPerSecond
		});

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchangeAutodiscover", "Process ID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			AutodiscoverPerformanceCounters.TotalRequests,
			AutodiscoverPerformanceCounters.TotalErrorResponses,
			AutodiscoverPerformanceCounters.PID
		};
	}
}
