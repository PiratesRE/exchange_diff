using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal static class ActiveDatabasePerformanceCounters
	{
		public static ActiveDatabasePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (ActiveDatabasePerformanceCountersInstance)ActiveDatabasePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ActiveDatabasePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ActiveDatabasePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ActiveDatabasePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ActiveDatabasePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ActiveDatabasePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ActiveDatabasePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ActiveDatabasePerformanceCountersInstance(instanceName, (ActiveDatabasePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ActiveDatabasePerformanceCountersInstance(instanceName);
		}

		public static ActiveDatabasePerformanceCountersInstance TotalInstance
		{
			get
			{
				return (ActiveDatabasePerformanceCountersInstance)ActiveDatabasePerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveDatabasePerformanceCounters.counters == null)
			{
				return;
			}
			ActiveDatabasePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeIS HA Active Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeIS HA Active Database", new CreateInstanceDelegate(ActiveDatabasePerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(ActiveDatabasePerformanceCounters.CreateTotalInstance));
	}
}
