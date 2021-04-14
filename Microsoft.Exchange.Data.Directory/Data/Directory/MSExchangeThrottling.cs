using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class MSExchangeThrottling
	{
		public static MSExchangeThrottlingInstance GetInstance(string instanceName)
		{
			return (MSExchangeThrottlingInstance)MSExchangeThrottling.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeThrottling.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeThrottling.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeThrottling.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeThrottling.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeThrottling.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeThrottling.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeThrottlingInstance(instanceName, (MSExchangeThrottlingInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeThrottlingInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeThrottling.counters == null)
			{
				return;
			}
			MSExchangeThrottling.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Throttling";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Throttling", new CreateInstanceDelegate(MSExchangeThrottling.CreateInstance));
	}
}
