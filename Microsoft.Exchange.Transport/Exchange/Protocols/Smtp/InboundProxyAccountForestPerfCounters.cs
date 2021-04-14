using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class InboundProxyAccountForestPerfCounters
	{
		public static InboundProxyAccountForestPerfCountersInstance GetInstance(string instanceName)
		{
			return (InboundProxyAccountForestPerfCountersInstance)InboundProxyAccountForestPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			InboundProxyAccountForestPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return InboundProxyAccountForestPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return InboundProxyAccountForestPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			InboundProxyAccountForestPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			InboundProxyAccountForestPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			InboundProxyAccountForestPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new InboundProxyAccountForestPerfCountersInstance(instanceName, (InboundProxyAccountForestPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new InboundProxyAccountForestPerfCountersInstance(instanceName);
		}

		public static InboundProxyAccountForestPerfCountersInstance TotalInstance
		{
			get
			{
				return (InboundProxyAccountForestPerfCountersInstance)InboundProxyAccountForestPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (InboundProxyAccountForestPerfCounters.counters == null)
			{
				return;
			}
			InboundProxyAccountForestPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeFrontendTransport InboundProxyEXOAccountForests";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeFrontendTransport InboundProxyEXOAccountForests", new CreateInstanceDelegate(InboundProxyAccountForestPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(InboundProxyAccountForestPerfCounters.CreateTotalInstance));
	}
}
