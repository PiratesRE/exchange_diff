using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class ResourceThrottlingPerfCounters
	{
		public static ResourceThrottlingPerfCountersInstance GetInstance(string instanceName)
		{
			return (ResourceThrottlingPerfCountersInstance)ResourceThrottlingPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ResourceThrottlingPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ResourceThrottlingPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ResourceThrottlingPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ResourceThrottlingPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ResourceThrottlingPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ResourceThrottlingPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ResourceThrottlingPerfCountersInstance(instanceName, (ResourceThrottlingPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ResourceThrottlingPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ResourceThrottlingPerfCounters.counters == null)
			{
				return;
			}
			ResourceThrottlingPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport ResourceThrottling";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport ResourceThrottling", new CreateInstanceDelegate(ResourceThrottlingPerfCounters.CreateInstance));
	}
}
