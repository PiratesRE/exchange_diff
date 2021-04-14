using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class AutodiscoverDatacenterPerformanceCounters
	{
		public static AutodiscoverDatacenterPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (AutodiscoverDatacenterPerformanceCountersInstance)AutodiscoverDatacenterPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			AutodiscoverDatacenterPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return AutodiscoverDatacenterPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return AutodiscoverDatacenterPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			AutodiscoverDatacenterPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			AutodiscoverDatacenterPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			AutodiscoverDatacenterPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new AutodiscoverDatacenterPerformanceCountersInstance(instanceName, (AutodiscoverDatacenterPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new AutodiscoverDatacenterPerformanceCountersInstance(instanceName);
		}

		public static AutodiscoverDatacenterPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (AutodiscoverDatacenterPerformanceCountersInstance)AutodiscoverDatacenterPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (AutodiscoverDatacenterPerformanceCounters.counters == null)
			{
				return;
			}
			AutodiscoverDatacenterPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeAutodiscover:Datacenter";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeAutodiscover:Datacenter", new CreateInstanceDelegate(AutodiscoverDatacenterPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(AutodiscoverDatacenterPerformanceCounters.CreateTotalInstance));
	}
}
