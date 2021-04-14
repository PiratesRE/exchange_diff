using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ReplayServicePerfmon
	{
		public static ReplayServicePerfmonInstance GetInstance(string instanceName)
		{
			return (ReplayServicePerfmonInstance)ReplayServicePerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ReplayServicePerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ReplayServicePerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ReplayServicePerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ReplayServicePerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ReplayServicePerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ReplayServicePerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ReplayServicePerfmonInstance(instanceName, (ReplayServicePerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ReplayServicePerfmonInstance(instanceName);
		}

		public static ReplayServicePerfmonInstance TotalInstance
		{
			get
			{
				return (ReplayServicePerfmonInstance)ReplayServicePerfmon.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ReplayServicePerfmon.counters == null)
			{
				return;
			}
			ReplayServicePerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Replication";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Replication", new CreateInstanceDelegate(ReplayServicePerfmon.CreateInstance), new CreateTotalInstanceDelegate(ReplayServicePerfmon.CreateTotalInstance));
	}
}
