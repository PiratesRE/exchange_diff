using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal static class PerformanceCountersPerDatabase
	{
		public static PerformanceCountersPerDatabaseInstance GetInstance(string instanceName)
		{
			return (PerformanceCountersPerDatabaseInstance)PerformanceCountersPerDatabase.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PerformanceCountersPerDatabase.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PerformanceCountersPerDatabase.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PerformanceCountersPerDatabase.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PerformanceCountersPerDatabase.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PerformanceCountersPerDatabase.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PerformanceCountersPerDatabase.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PerformanceCountersPerDatabaseInstance(instanceName, (PerformanceCountersPerDatabaseInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PerformanceCountersPerDatabaseInstance(instanceName);
		}

		public static PerformanceCountersPerDatabaseInstance TotalInstance
		{
			get
			{
				return (PerformanceCountersPerDatabaseInstance)PerformanceCountersPerDatabase.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerformanceCountersPerDatabase.counters == null)
			{
				return;
			}
			PerformanceCountersPerDatabase.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Assistants - Per Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Assistants - Per Database", new CreateInstanceDelegate(PerformanceCountersPerDatabase.CreateInstance), new CreateTotalInstanceDelegate(PerformanceCountersPerDatabase.CreateTotalInstance));
	}
}
