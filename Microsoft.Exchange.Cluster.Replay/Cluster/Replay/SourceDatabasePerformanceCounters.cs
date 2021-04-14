using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class SourceDatabasePerformanceCounters
	{
		public static SourceDatabasePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (SourceDatabasePerformanceCountersInstance)SourceDatabasePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SourceDatabasePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SourceDatabasePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SourceDatabasePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SourceDatabasePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SourceDatabasePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SourceDatabasePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SourceDatabasePerformanceCountersInstance(instanceName, (SourceDatabasePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SourceDatabasePerformanceCountersInstance(instanceName);
		}

		public static SourceDatabasePerformanceCountersInstance TotalInstance
		{
			get
			{
				return (SourceDatabasePerformanceCountersInstance)SourceDatabasePerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SourceDatabasePerformanceCounters.counters == null)
			{
				return;
			}
			SourceDatabasePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeRepl Source Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeRepl Source Database", new CreateInstanceDelegate(SourceDatabasePerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(SourceDatabasePerformanceCounters.CreateTotalInstance));
	}
}
