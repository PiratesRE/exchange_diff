using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class RoutingPerfCounters
	{
		public static RoutingPerfCountersInstance GetInstance(string instanceName)
		{
			return (RoutingPerfCountersInstance)RoutingPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			RoutingPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return RoutingPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return RoutingPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			RoutingPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			RoutingPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			RoutingPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new RoutingPerfCountersInstance(instanceName, (RoutingPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new RoutingPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (RoutingPerfCounters.counters == null)
			{
				RoutingPerfCounters.CategoryName = categoryName;
				RoutingPerfCounters.counters = new PerformanceCounterMultipleInstance(RoutingPerfCounters.CategoryName, new CreateInstanceDelegate(RoutingPerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (RoutingPerfCounters.counters == null)
			{
				return;
			}
			RoutingPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
