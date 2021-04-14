using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal static class GlsPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (GlsPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in GlsPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Global Locator";

		public static readonly ExPerformanceCounter AverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator", "Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator", "Base for Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReadLatency = new ExPerformanceCounter("MSExchange Global Locator", "Average Read Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReadLatencyBase = new ExPerformanceCounter("MSExchange Global Locator", "Base for Average Read Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageWriteLatency = new ExPerformanceCounter("MSExchange Global Locator", "Average Write Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageWriteLatencyBase = new ExPerformanceCounter("MSExchange Global Locator", "Base for Average Write Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			GlsPerformanceCounters.AverageOverallLatency,
			GlsPerformanceCounters.AverageOverallLatencyBase,
			GlsPerformanceCounters.AverageReadLatency,
			GlsPerformanceCounters.AverageReadLatencyBase,
			GlsPerformanceCounters.AverageWriteLatency,
			GlsPerformanceCounters.AverageWriteLatencyBase
		};
	}
}
