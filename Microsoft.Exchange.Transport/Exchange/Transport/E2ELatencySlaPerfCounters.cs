using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class E2ELatencySlaPerfCounters
	{
		public static E2ELatencySlaPerfCountersInstance GetInstance(string instanceName)
		{
			return (E2ELatencySlaPerfCountersInstance)E2ELatencySlaPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			E2ELatencySlaPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return E2ELatencySlaPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return E2ELatencySlaPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			E2ELatencySlaPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			E2ELatencySlaPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			E2ELatencySlaPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new E2ELatencySlaPerfCountersInstance(instanceName, (E2ELatencySlaPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new E2ELatencySlaPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (E2ELatencySlaPerfCounters.counters == null)
			{
				return;
			}
			E2ELatencySlaPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport E2E Latency SLA";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport E2E Latency SLA", new CreateInstanceDelegate(E2ELatencySlaPerfCounters.CreateInstance));
	}
}
