using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class MSExchangeUserThrottling
	{
		public static MSExchangeUserThrottlingInstance GetInstance(string instanceName)
		{
			return (MSExchangeUserThrottlingInstance)MSExchangeUserThrottling.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeUserThrottling.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeUserThrottling.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeUserThrottling.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeUserThrottling.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeUserThrottling.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeUserThrottling.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeUserThrottlingInstance(instanceName, (MSExchangeUserThrottlingInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeUserThrottlingInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeUserThrottling.counters == null)
			{
				return;
			}
			MSExchangeUserThrottling.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange User Throttling";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange User Throttling", new CreateInstanceDelegate(MSExchangeUserThrottling.CreateInstance));
	}
}
