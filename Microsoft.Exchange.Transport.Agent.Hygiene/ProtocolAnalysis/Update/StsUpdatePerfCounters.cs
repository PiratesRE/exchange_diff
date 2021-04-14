using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Update
{
	internal static class StsUpdatePerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (StsUpdatePerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in StsUpdatePerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Update Agent";

		public static readonly ExPerformanceCounter TotalUpdate = new ExPerformanceCounter("MSExchange Update Agent", "Total Updates", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSrlUpdate = new ExPerformanceCounter("MSExchange Update Agent", "Total SRL Parameter Updates", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			StsUpdatePerfCounters.TotalUpdate,
			StsUpdatePerfCounters.TotalSrlUpdate
		};
	}
}
