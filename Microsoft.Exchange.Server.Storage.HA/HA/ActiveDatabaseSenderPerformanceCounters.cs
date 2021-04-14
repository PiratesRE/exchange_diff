using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal static class ActiveDatabaseSenderPerformanceCounters
	{
		public static ActiveDatabaseSenderPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (ActiveDatabaseSenderPerformanceCountersInstance)ActiveDatabaseSenderPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ActiveDatabaseSenderPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ActiveDatabaseSenderPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ActiveDatabaseSenderPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ActiveDatabaseSenderPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ActiveDatabaseSenderPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ActiveDatabaseSenderPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ActiveDatabaseSenderPerformanceCountersInstance(instanceName, (ActiveDatabaseSenderPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ActiveDatabaseSenderPerformanceCountersInstance(instanceName);
		}

		public static ActiveDatabaseSenderPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (ActiveDatabaseSenderPerformanceCountersInstance)ActiveDatabaseSenderPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveDatabaseSenderPerformanceCounters.counters == null)
			{
				return;
			}
			ActiveDatabaseSenderPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeIS HA Active Database Sender";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeIS HA Active Database Sender", new CreateInstanceDelegate(ActiveDatabaseSenderPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(ActiveDatabaseSenderPerformanceCounters.CreateTotalInstance));
	}
}
