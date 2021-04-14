using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ActiveManagerPerfmon
	{
		public static ActiveManagerPerfmonInstance GetInstance(string instanceName)
		{
			return (ActiveManagerPerfmonInstance)ActiveManagerPerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ActiveManagerPerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ActiveManagerPerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ActiveManagerPerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ActiveManagerPerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ActiveManagerPerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ActiveManagerPerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ActiveManagerPerfmonInstance(instanceName, (ActiveManagerPerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ActiveManagerPerfmonInstance(instanceName);
		}

		public static ActiveManagerPerfmonInstance TotalInstance
		{
			get
			{
				return (ActiveManagerPerfmonInstance)ActiveManagerPerfmon.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveManagerPerfmon.counters == null)
			{
				return;
			}
			ActiveManagerPerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Active Manager";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Active Manager", new CreateInstanceDelegate(ActiveManagerPerfmon.CreateInstance), new CreateTotalInstanceDelegate(ActiveManagerPerfmon.CreateTotalInstance));
	}
}
