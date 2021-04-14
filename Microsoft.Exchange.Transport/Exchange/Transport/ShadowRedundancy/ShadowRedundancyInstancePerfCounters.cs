using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal static class ShadowRedundancyInstancePerfCounters
	{
		public static ShadowRedundancyInstancePerfCountersInstance GetInstance(string instanceName)
		{
			return (ShadowRedundancyInstancePerfCountersInstance)ShadowRedundancyInstancePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ShadowRedundancyInstancePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ShadowRedundancyInstancePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ShadowRedundancyInstancePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ShadowRedundancyInstancePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ShadowRedundancyInstancePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ShadowRedundancyInstancePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ShadowRedundancyInstancePerfCountersInstance(instanceName, (ShadowRedundancyInstancePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ShadowRedundancyInstancePerfCountersInstance(instanceName);
		}

		public static ShadowRedundancyInstancePerfCountersInstance TotalInstance
		{
			get
			{
				return (ShadowRedundancyInstancePerfCountersInstance)ShadowRedundancyInstancePerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ShadowRedundancyInstancePerfCounters.counters == null)
			{
				return;
			}
			ShadowRedundancyInstancePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Shadow Redundancy Host Info";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTransport Shadow Redundancy Host Info", new CreateInstanceDelegate(ShadowRedundancyInstancePerfCounters.CreateInstance), new CreateTotalInstanceDelegate(ShadowRedundancyInstancePerfCounters.CreateTotalInstance));
	}
}
