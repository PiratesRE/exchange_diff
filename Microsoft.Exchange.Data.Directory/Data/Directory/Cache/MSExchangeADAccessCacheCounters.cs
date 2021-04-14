using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal static class MSExchangeADAccessCacheCounters
	{
		public static MSExchangeADAccessCacheCountersInstance GetInstance(string instanceName)
		{
			return (MSExchangeADAccessCacheCountersInstance)MSExchangeADAccessCacheCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeADAccessCacheCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeADAccessCacheCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeADAccessCacheCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeADAccessCacheCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeADAccessCacheCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeADAccessCacheCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeADAccessCacheCountersInstance(instanceName, (MSExchangeADAccessCacheCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeADAccessCacheCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeADAccessCacheCounters.counters == null)
			{
				return;
			}
			MSExchangeADAccessCacheCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange ADAccess Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange ADAccess Cache", new CreateInstanceDelegate(MSExchangeADAccessCacheCounters.CreateInstance));
	}
}
