using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class ProxyHubSelectorPerfCounters
	{
		public static ProxyHubSelectorPerfCountersInstance GetInstance(string instanceName)
		{
			return (ProxyHubSelectorPerfCountersInstance)ProxyHubSelectorPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ProxyHubSelectorPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ProxyHubSelectorPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ProxyHubSelectorPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ProxyHubSelectorPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ProxyHubSelectorPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ProxyHubSelectorPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ProxyHubSelectorPerfCountersInstance(instanceName, (ProxyHubSelectorPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ProxyHubSelectorPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (ProxyHubSelectorPerfCounters.counters == null)
			{
				ProxyHubSelectorPerfCounters.CategoryName = categoryName;
				ProxyHubSelectorPerfCounters.counters = new PerformanceCounterMultipleInstance(ProxyHubSelectorPerfCounters.CategoryName, new CreateInstanceDelegate(ProxyHubSelectorPerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ProxyHubSelectorPerfCounters.counters == null)
			{
				return;
			}
			ProxyHubSelectorPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
