using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class FaxCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (FaxCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in FaxCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeUMFax";

		public static readonly ExPerformanceCounter TotalNumberOfInvalidFaxCalls = new ExPerformanceCounter("MSExchangeUMFax", "Total Invalid Fax Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNumberOfValidFaxCalls = new ExPerformanceCounter("MSExchangeUMFax", "Total Valid Fax Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNumberOfSuccessfulValidFaxCalls = new ExPerformanceCounter("MSExchangeUMFax", "Total Succesful Valid Fax Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageSuccessfulValidFaxCalls = new ExPerformanceCounter("MSExchangeUMFax", "Percentage of Successful Valid Fax Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageSuccessfulValidFaxCalls_Base = new ExPerformanceCounter("MSExchangeUMFax", "Base for Percentage Successful Valid Fax Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			FaxCounters.TotalNumberOfInvalidFaxCalls,
			FaxCounters.TotalNumberOfValidFaxCalls,
			FaxCounters.TotalNumberOfSuccessfulValidFaxCalls,
			FaxCounters.PercentageSuccessfulValidFaxCalls,
			FaxCounters.PercentageSuccessfulValidFaxCalls_Base
		};
	}
}
