using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class MSExchangeWorkloadManagementWorkload
	{
		public static MSExchangeWorkloadManagementWorkloadInstance GetInstance(string instanceName)
		{
			return (MSExchangeWorkloadManagementWorkloadInstance)MSExchangeWorkloadManagementWorkload.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeWorkloadManagementWorkload.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeWorkloadManagementWorkload.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeWorkloadManagementWorkload.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeWorkloadManagementWorkload.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeWorkloadManagementWorkload.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeWorkloadManagementWorkload.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeWorkloadManagementWorkloadInstance(instanceName, (MSExchangeWorkloadManagementWorkloadInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeWorkloadManagementWorkloadInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeWorkloadManagementWorkload.counters == null)
			{
				return;
			}
			MSExchangeWorkloadManagementWorkload.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange WorkloadManagement Workloads";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange WorkloadManagement Workloads", new CreateInstanceDelegate(MSExchangeWorkloadManagementWorkload.CreateInstance));
	}
}
