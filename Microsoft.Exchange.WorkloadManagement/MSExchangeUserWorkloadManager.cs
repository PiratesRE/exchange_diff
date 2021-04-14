using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class MSExchangeUserWorkloadManager
	{
		public static MSExchangeUserWorkloadManagerInstance GetInstance(string instanceName)
		{
			return (MSExchangeUserWorkloadManagerInstance)MSExchangeUserWorkloadManager.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeUserWorkloadManager.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeUserWorkloadManager.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeUserWorkloadManager.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeUserWorkloadManager.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeUserWorkloadManager.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeUserWorkloadManager.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeUserWorkloadManagerInstance(instanceName, (MSExchangeUserWorkloadManagerInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeUserWorkloadManagerInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeUserWorkloadManager.counters == null)
			{
				return;
			}
			MSExchangeUserWorkloadManager.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange User WorkloadManager";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange User WorkloadManager", new CreateInstanceDelegate(MSExchangeUserWorkloadManager.CreateInstance));
	}
}
