using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class MSExchangeWorkloadManagementClassification
	{
		public static MSExchangeWorkloadManagementClassificationInstance GetInstance(string instanceName)
		{
			return (MSExchangeWorkloadManagementClassificationInstance)MSExchangeWorkloadManagementClassification.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeWorkloadManagementClassification.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeWorkloadManagementClassification.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeWorkloadManagementClassification.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeWorkloadManagementClassification.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeWorkloadManagementClassification.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeWorkloadManagementClassification.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeWorkloadManagementClassificationInstance(instanceName, (MSExchangeWorkloadManagementClassificationInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeWorkloadManagementClassificationInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeWorkloadManagementClassification.counters == null)
			{
				return;
			}
			MSExchangeWorkloadManagementClassification.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange WorkloadManagement Classification";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange WorkloadManagement Classification", new CreateInstanceDelegate(MSExchangeWorkloadManagementClassification.CreateInstance));
	}
}
