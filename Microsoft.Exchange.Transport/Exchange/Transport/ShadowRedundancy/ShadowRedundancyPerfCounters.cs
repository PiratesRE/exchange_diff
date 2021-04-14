using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal static class ShadowRedundancyPerfCounters
	{
		public static ShadowRedundancyPerfCountersInstance GetInstance(string instanceName)
		{
			return (ShadowRedundancyPerfCountersInstance)ShadowRedundancyPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ShadowRedundancyPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ShadowRedundancyPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ShadowRedundancyPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ShadowRedundancyPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ShadowRedundancyPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ShadowRedundancyPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ShadowRedundancyPerfCountersInstance(instanceName, (ShadowRedundancyPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ShadowRedundancyPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ShadowRedundancyPerfCounters.counters == null)
			{
				return;
			}
			ShadowRedundancyPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Shadow Redundancy";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport Shadow Redundancy", new CreateInstanceDelegate(ShadowRedundancyPerfCounters.CreateInstance));
	}
}
