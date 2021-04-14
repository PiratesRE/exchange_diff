using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MiddleTierStoragePerformanceCounters
	{
		public static MiddleTierStoragePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (MiddleTierStoragePerformanceCountersInstance)MiddleTierStoragePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MiddleTierStoragePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MiddleTierStoragePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MiddleTierStoragePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MiddleTierStoragePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MiddleTierStoragePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MiddleTierStoragePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MiddleTierStoragePerformanceCountersInstance(instanceName, (MiddleTierStoragePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MiddleTierStoragePerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MiddleTierStoragePerformanceCounters.counters == null)
			{
				return;
			}
			MiddleTierStoragePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Middle-Tier Storage";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Middle-Tier Storage", new CreateInstanceDelegate(MiddleTierStoragePerformanceCounters.CreateInstance));
	}
}
