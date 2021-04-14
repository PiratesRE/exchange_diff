using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Storage
{
	internal static class DatabasePerfCounters
	{
		public static DatabasePerfCountersInstance GetInstance(string instanceName)
		{
			return (DatabasePerfCountersInstance)DatabasePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			DatabasePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return DatabasePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return DatabasePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			DatabasePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			DatabasePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			DatabasePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new DatabasePerfCountersInstance(instanceName, (DatabasePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new DatabasePerfCountersInstance(instanceName);
		}

		public static DatabasePerfCountersInstance TotalInstance
		{
			get
			{
				return (DatabasePerfCountersInstance)DatabasePerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (DatabasePerfCounters.counters == null)
			{
				return;
			}
			DatabasePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTransport Database", new CreateInstanceDelegate(DatabasePerfCounters.CreateInstance), new CreateTotalInstanceDelegate(DatabasePerfCounters.CreateTotalInstance));
	}
}
