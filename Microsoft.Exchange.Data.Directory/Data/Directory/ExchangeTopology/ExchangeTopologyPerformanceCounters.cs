using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal static class ExchangeTopologyPerformanceCounters
	{
		public static ExchangeTopologyPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (ExchangeTopologyPerformanceCountersInstance)ExchangeTopologyPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ExchangeTopologyPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ExchangeTopologyPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ExchangeTopologyPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ExchangeTopologyPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ExchangeTopologyPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ExchangeTopologyPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ExchangeTopologyPerformanceCountersInstance(instanceName, (ExchangeTopologyPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ExchangeTopologyPerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ExchangeTopologyPerformanceCounters.counters == null)
			{
				return;
			}
			ExchangeTopologyPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Topology";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Topology", new CreateInstanceDelegate(ExchangeTopologyPerformanceCounters.CreateInstance));
	}
}
