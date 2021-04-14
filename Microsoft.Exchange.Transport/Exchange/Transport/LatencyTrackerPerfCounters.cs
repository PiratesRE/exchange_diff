using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class LatencyTrackerPerfCounters
	{
		public static LatencyTrackerPerfCountersInstance GetInstance(string instanceName)
		{
			return (LatencyTrackerPerfCountersInstance)LatencyTrackerPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			LatencyTrackerPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return LatencyTrackerPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return LatencyTrackerPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			LatencyTrackerPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			LatencyTrackerPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			LatencyTrackerPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new LatencyTrackerPerfCountersInstance(instanceName, (LatencyTrackerPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new LatencyTrackerPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (LatencyTrackerPerfCounters.counters == null)
			{
				LatencyTrackerPerfCounters.CategoryName = categoryName;
				LatencyTrackerPerfCounters.counters = new PerformanceCounterMultipleInstance(LatencyTrackerPerfCounters.CategoryName, new CreateInstanceDelegate(LatencyTrackerPerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (LatencyTrackerPerfCounters.counters == null)
			{
				return;
			}
			LatencyTrackerPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
