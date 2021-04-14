using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class MSExchangeADRecipientCache
	{
		public static MSExchangeADRecipientCacheInstance GetInstance(string instanceName)
		{
			return (MSExchangeADRecipientCacheInstance)MSExchangeADRecipientCache.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeADRecipientCache.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeADRecipientCache.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeADRecipientCache.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeADRecipientCache.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeADRecipientCache.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeADRecipientCache.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeADRecipientCacheInstance(instanceName, (MSExchangeADRecipientCacheInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeADRecipientCacheInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeADRecipientCache.counters == null)
			{
				return;
			}
			MSExchangeADRecipientCache.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Recipient Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Recipient Cache", new CreateInstanceDelegate(MSExchangeADRecipientCache.CreateInstance));
	}
}
