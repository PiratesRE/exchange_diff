using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric
{
	internal static class TaskDistributionFabricPerfCounters
	{
		public static TaskDistributionFabricPerfCountersInstance GetInstance(string instanceName)
		{
			return (TaskDistributionFabricPerfCountersInstance)TaskDistributionFabricPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			TaskDistributionFabricPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return TaskDistributionFabricPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return TaskDistributionFabricPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			TaskDistributionFabricPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			TaskDistributionFabricPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			TaskDistributionFabricPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new TaskDistributionFabricPerfCountersInstance(instanceName, (TaskDistributionFabricPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new TaskDistributionFabricPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (TaskDistributionFabricPerfCounters.counters == null)
			{
				return;
			}
			TaskDistributionFabricPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Task Distribution Fabric";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Task Distribution Fabric", new CreateInstanceDelegate(TaskDistributionFabricPerfCounters.CreateInstance));
	}
}
