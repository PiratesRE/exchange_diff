using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class MSExchangeWorkloadManagement
	{
		public static MSExchangeWorkloadManagementInstance GetInstance(string instanceName)
		{
			return (MSExchangeWorkloadManagementInstance)MSExchangeWorkloadManagement.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeWorkloadManagement.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeWorkloadManagement.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeWorkloadManagement.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeWorkloadManagement.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeWorkloadManagement.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeWorkloadManagement.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeWorkloadManagementInstance(instanceName, (MSExchangeWorkloadManagementInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeWorkloadManagementInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeWorkloadManagement.counters == null)
			{
				return;
			}
			MSExchangeWorkloadManagement.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange WorkloadManagement";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange WorkloadManagement", new CreateInstanceDelegate(MSExchangeWorkloadManagement.CreateInstance));
	}
}
