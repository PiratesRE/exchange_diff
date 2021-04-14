using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal static class TenantMonitor
	{
		public static void LogActivity(CounterType counterType, string organizationName)
		{
			if (TenantMonitor.counterCategoryExist)
			{
				IntervalCounterInstanceCache.IncrementIntervalCounter(organizationName ?? "First Organization", counterType);
			}
		}

		private static MSExchangeTenantMonitoringInstance GetInstance(string instanceName)
		{
			return MSExchangeTenantMonitoring.GetInstance(instanceName);
		}

		public const string TenantMonitoringRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange Tenant Monitoring";

		public const string TenantMonitoringCounterCategory = "MSExchangeTenantMonitoring";

		public const string DefaultTenantName = "First Organization";

		private static bool counterCategoryExist = PerformanceCounterCategory.Exists("MSExchangeTenantMonitoring");
	}
}
