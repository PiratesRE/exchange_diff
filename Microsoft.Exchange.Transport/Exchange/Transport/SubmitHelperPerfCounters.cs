using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class SubmitHelperPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (SubmitHelperPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in SubmitHelperPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeTransport Submit Helper";

		public static readonly ExPerformanceCounter AgentSubmitted = new ExPerformanceCounter("MSExchangeTransport Submit Helper", "Agent Messages Submitted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			SubmitHelperPerfCounters.AgentSubmitted
		};
	}
}
