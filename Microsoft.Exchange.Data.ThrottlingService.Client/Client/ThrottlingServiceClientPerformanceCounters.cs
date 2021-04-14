using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal static class ThrottlingServiceClientPerformanceCounters
	{
		public static ThrottlingServiceClientPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (ThrottlingServiceClientPerformanceCountersInstance)ThrottlingServiceClientPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ThrottlingServiceClientPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ThrottlingServiceClientPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ThrottlingServiceClientPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ThrottlingServiceClientPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ThrottlingServiceClientPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ThrottlingServiceClientPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ThrottlingServiceClientPerformanceCountersInstance(instanceName, (ThrottlingServiceClientPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ThrottlingServiceClientPerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ThrottlingServiceClientPerformanceCounters.counters == null)
			{
				return;
			}
			ThrottlingServiceClientPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Throttling Service Client";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Throttling Service Client", new CreateInstanceDelegate(ThrottlingServiceClientPerformanceCounters.CreateInstance));
	}
}
