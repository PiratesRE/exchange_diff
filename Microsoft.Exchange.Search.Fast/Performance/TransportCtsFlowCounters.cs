using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Performance
{
	internal static class TransportCtsFlowCounters
	{
		public static TransportCtsFlowCountersInstance GetInstance(string instanceName)
		{
			return (TransportCtsFlowCountersInstance)TransportCtsFlowCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			TransportCtsFlowCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return TransportCtsFlowCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return TransportCtsFlowCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			TransportCtsFlowCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			TransportCtsFlowCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			TransportCtsFlowCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new TransportCtsFlowCountersInstance(instanceName, (TransportCtsFlowCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new TransportCtsFlowCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (TransportCtsFlowCounters.counters == null)
			{
				return;
			}
			TransportCtsFlowCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeSearch Transport CTS Flow";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeSearch Transport CTS Flow", new CreateInstanceDelegate(TransportCtsFlowCounters.CreateInstance));
	}
}
