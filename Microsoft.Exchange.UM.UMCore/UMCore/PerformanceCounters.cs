using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class PerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeUMPerformance";

		public static readonly ExPerformanceCounter OperationsUnderTwoSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations under Two Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsBetweenTwoAndThreeSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations Between Two and Three Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsBetweenThreeAndFourSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations Between Three and Four Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsBetweenFourAndFiveSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations Between Four and Five Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsBetweenFiveAndSixSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations Between Five and Six Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsOverSixSeconds = new ExPerformanceCounter("MSExchangeUMPerformance", "Operations over Six Seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerformanceCounters.OperationsUnderTwoSeconds,
			PerformanceCounters.OperationsBetweenTwoAndThreeSeconds,
			PerformanceCounters.OperationsBetweenThreeAndFourSeconds,
			PerformanceCounters.OperationsBetweenFourAndFiveSeconds,
			PerformanceCounters.OperationsBetweenFiveAndSixSeconds,
			PerformanceCounters.OperationsOverSixSeconds
		};
	}
}
