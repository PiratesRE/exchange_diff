using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ActiveManagerDagNamePerfmon
	{
		public static ActiveManagerDagNamePerfmonInstance GetInstance(string instanceName)
		{
			return (ActiveManagerDagNamePerfmonInstance)ActiveManagerDagNamePerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ActiveManagerDagNamePerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ActiveManagerDagNamePerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ActiveManagerDagNamePerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ActiveManagerDagNamePerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ActiveManagerDagNamePerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ActiveManagerDagNamePerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ActiveManagerDagNamePerfmonInstance(instanceName, (ActiveManagerDagNamePerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ActiveManagerDagNamePerfmonInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveManagerDagNamePerfmon.counters == null)
			{
				return;
			}
			ActiveManagerDagNamePerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Active Manager Dag Name Instance";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Active Manager Dag Name Instance", new CreateInstanceDelegate(ActiveManagerDagNamePerfmon.CreateInstance));
	}
}
