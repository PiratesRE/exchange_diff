using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal static class PerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Attachment Filtering";

		private static readonly ExPerformanceCounter MsgsFilteredRate = new ExPerformanceCounter("MSExchange Attachment Filtering", "Messages Filtered/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MsgsFiltered = new ExPerformanceCounter("MSExchange Attachment Filtering", "Messages Attachment Filtered", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.MsgsFilteredRate
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerfCounters.MsgsFiltered
		};
	}
}
