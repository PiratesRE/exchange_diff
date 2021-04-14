using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal static class MSExchangeTenantMonitoring
	{
		public static MSExchangeTenantMonitoringInstance GetInstance(string instanceName)
		{
			return (MSExchangeTenantMonitoringInstance)MSExchangeTenantMonitoring.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeTenantMonitoring.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeTenantMonitoring.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeTenantMonitoring.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeTenantMonitoring.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeTenantMonitoring.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeTenantMonitoring.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeTenantMonitoringInstance(instanceName, (MSExchangeTenantMonitoringInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeTenantMonitoringInstance(instanceName);
		}

		public static MSExchangeTenantMonitoringInstance TotalInstance
		{
			get
			{
				return (MSExchangeTenantMonitoringInstance)MSExchangeTenantMonitoring.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeTenantMonitoring.counters == null)
			{
				return;
			}
			MSExchangeTenantMonitoring.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTenantMonitoring";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTenantMonitoring", new CreateInstanceDelegate(MSExchangeTenantMonitoring.CreateInstance), new CreateTotalInstanceDelegate(MSExchangeTenantMonitoring.CreateTotalInstance));
	}
}
