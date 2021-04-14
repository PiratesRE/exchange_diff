using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal static class MSExchangeProvisioningCache
	{
		public static MSExchangeProvisioningCacheInstance GetInstance(string instanceName)
		{
			return (MSExchangeProvisioningCacheInstance)MSExchangeProvisioningCache.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeProvisioningCache.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeProvisioningCache.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeProvisioningCache.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeProvisioningCache.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeProvisioningCache.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeProvisioningCache.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeProvisioningCacheInstance(instanceName, (MSExchangeProvisioningCacheInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeProvisioningCacheInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeProvisioningCache.counters == null)
			{
				return;
			}
			MSExchangeProvisioningCache.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Provisioning Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Provisioning Cache", new CreateInstanceDelegate(MSExchangeProvisioningCache.CreateInstance));
	}
}
