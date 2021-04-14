using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal static class UMClientAccessCounters
	{
		public static UMClientAccessCountersInstance GetInstance(string instanceName)
		{
			return (UMClientAccessCountersInstance)UMClientAccessCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			UMClientAccessCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return UMClientAccessCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return UMClientAccessCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			UMClientAccessCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			UMClientAccessCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			UMClientAccessCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new UMClientAccessCountersInstance(instanceName, (UMClientAccessCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new UMClientAccessCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (UMClientAccessCounters.counters == null)
			{
				return;
			}
			UMClientAccessCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeUMClientAccess";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeUMClientAccess", new CreateInstanceDelegate(UMClientAccessCounters.CreateInstance));
	}
}
