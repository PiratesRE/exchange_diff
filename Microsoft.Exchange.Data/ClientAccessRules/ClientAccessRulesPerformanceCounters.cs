using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal static class ClientAccessRulesPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ClientAccessRulesPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ClientAccessRulesPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeCAR";

		public static readonly ExPerformanceCounter TotalClientAccessRulesEvaluationCalls = new ExPerformanceCounter("MSExchangeCAR", "ClientAccessRules Evaluation Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConnectionsBlockedByClientAccessRules = new ExPerformanceCounter("MSExchangeCAR", "ClientAccessRules Evaluation Blocks", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalClientAccessRulesEvaluationCallsOver10ms = new ExPerformanceCounter("MSExchangeCAR", "ClientAccessRules Evaluation Calls that took over 10ms", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalClientAccessRulesEvaluationCallsOver50ms = new ExPerformanceCounter("MSExchangeCAR", "ClientAccessRules Evaluation Calls that took over 50ms", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCalls,
			ClientAccessRulesPerformanceCounters.TotalConnectionsBlockedByClientAccessRules,
			ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCallsOver10ms,
			ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCallsOver50ms
		};
	}
}
