using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class StorePerDatabasePerformanceCounters
	{
		public static StorePerDatabasePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (StorePerDatabasePerformanceCountersInstance)StorePerDatabasePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			StorePerDatabasePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return StorePerDatabasePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return StorePerDatabasePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			StorePerDatabasePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			StorePerDatabasePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			StorePerDatabasePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new StorePerDatabasePerformanceCountersInstance(instanceName, (StorePerDatabasePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new StorePerDatabasePerformanceCountersInstance(instanceName);
		}

		public static StorePerDatabasePerformanceCountersInstance TotalInstance
		{
			get
			{
				return (StorePerDatabasePerformanceCountersInstance)StorePerDatabasePerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (StorePerDatabasePerformanceCounters.counters == null)
			{
				return;
			}
			StorePerDatabasePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeIS Store";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeIS Store", new CreateInstanceDelegate(StorePerDatabasePerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(StorePerDatabasePerformanceCounters.CreateTotalInstance));
	}
}
