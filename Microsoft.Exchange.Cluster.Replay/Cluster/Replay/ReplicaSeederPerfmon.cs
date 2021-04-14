using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ReplicaSeederPerfmon
	{
		public static ReplicaSeederPerfmonInstance GetInstance(string instanceName)
		{
			return (ReplicaSeederPerfmonInstance)ReplicaSeederPerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ReplicaSeederPerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ReplicaSeederPerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ReplicaSeederPerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ReplicaSeederPerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ReplicaSeederPerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ReplicaSeederPerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ReplicaSeederPerfmonInstance(instanceName, (ReplicaSeederPerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ReplicaSeederPerfmonInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ReplicaSeederPerfmon.counters == null)
			{
				return;
			}
			ReplicaSeederPerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Replica Seeder";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Replica Seeder", new CreateInstanceDelegate(ReplicaSeederPerfmon.CreateInstance));
	}
}
