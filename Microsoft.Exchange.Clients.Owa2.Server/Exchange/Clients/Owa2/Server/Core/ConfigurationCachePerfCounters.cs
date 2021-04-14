using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class ConfigurationCachePerfCounters
	{
		public static ConfigurationCachePerfCountersInstance GetInstance(string instanceName)
		{
			return (ConfigurationCachePerfCountersInstance)ConfigurationCachePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ConfigurationCachePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ConfigurationCachePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ConfigurationCachePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ConfigurationCachePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ConfigurationCachePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ConfigurationCachePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ConfigurationCachePerfCountersInstance(instanceName, (ConfigurationCachePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ConfigurationCachePerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ConfigurationCachePerfCounters.counters == null)
			{
				return;
			}
			ConfigurationCachePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Owa Configuration Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Owa Configuration Cache", new CreateInstanceDelegate(ConfigurationCachePerfCounters.CreateInstance));
	}
}
