using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class RmSvcAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RmSvcAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RmSvcAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange RMS Agents";

		public static readonly ExPerformanceCounter CurrentActiveAgents = new ExPerformanceCounter("MSExchange RMS Agents", "Active RMS Licensing Agents", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfSuccessfulActiveRequests = new ExPerformanceCounter("MSExchange RMS Agents", "Active RMS Licensing Agents/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulActiveRequests = new ExPerformanceCounter("MSExchange RMS Agents", "Total active RMS Licensing Agents", string.Empty, null, new ExPerformanceCounter[]
		{
			RmSvcAgentPerfCounters.RateOfSuccessfulActiveRequests
		});

		private static readonly ExPerformanceCounter RateOfUnsuccessfulActiveRequests = new ExPerformanceCounter("MSExchange RMS Agents", "RMS Licensing Agents Failed to Process/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnsuccessfulActiveRequests = new ExPerformanceCounter("MSExchange RMS Agents", "Total RMS Licensing Agents Failed to Process", string.Empty, null, new ExPerformanceCounter[]
		{
			RmSvcAgentPerfCounters.RateOfUnsuccessfulActiveRequests
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RmSvcAgentPerfCounters.CurrentActiveAgents,
			RmSvcAgentPerfCounters.TotalSuccessfulActiveRequests,
			RmSvcAgentPerfCounters.TotalUnsuccessfulActiveRequests
		};
	}
}
