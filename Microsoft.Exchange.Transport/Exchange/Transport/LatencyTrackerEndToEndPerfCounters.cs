using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class LatencyTrackerEndToEndPerfCounters
	{
		public static LatencyTrackerEndToEndPerfCountersInstance GetInstance(string instanceName)
		{
			return (LatencyTrackerEndToEndPerfCountersInstance)LatencyTrackerEndToEndPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			LatencyTrackerEndToEndPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return LatencyTrackerEndToEndPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return LatencyTrackerEndToEndPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			LatencyTrackerEndToEndPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			LatencyTrackerEndToEndPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			LatencyTrackerEndToEndPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new LatencyTrackerEndToEndPerfCountersInstance(instanceName, (LatencyTrackerEndToEndPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new LatencyTrackerEndToEndPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (LatencyTrackerEndToEndPerfCounters.counters == null)
			{
				LatencyTrackerEndToEndPerfCounters.CategoryName = categoryName;
				LatencyTrackerEndToEndPerfCounters.counters = new PerformanceCounterMultipleInstance(LatencyTrackerEndToEndPerfCounters.CategoryName, new CreateInstanceDelegate(LatencyTrackerEndToEndPerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (LatencyTrackerEndToEndPerfCounters.counters == null)
			{
				return;
			}
			LatencyTrackerEndToEndPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
