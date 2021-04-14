using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class NetworkManagerPerfmon
	{
		public static NetworkManagerPerfmonInstance GetInstance(string instanceName)
		{
			return (NetworkManagerPerfmonInstance)NetworkManagerPerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			NetworkManagerPerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return NetworkManagerPerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return NetworkManagerPerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			NetworkManagerPerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			NetworkManagerPerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			NetworkManagerPerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new NetworkManagerPerfmonInstance(instanceName, (NetworkManagerPerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new NetworkManagerPerfmonInstance(instanceName);
		}

		public static NetworkManagerPerfmonInstance TotalInstance
		{
			get
			{
				return (NetworkManagerPerfmonInstance)NetworkManagerPerfmon.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (NetworkManagerPerfmon.counters == null)
			{
				return;
			}
			NetworkManagerPerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Network Manager";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Network Manager", new CreateInstanceDelegate(NetworkManagerPerfmon.CreateInstance), new CreateTotalInstanceDelegate(NetworkManagerPerfmon.CreateTotalInstance));
	}
}
