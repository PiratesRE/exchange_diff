using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal static class ServiceProxyPoolCounters
	{
		public static ServiceProxyPoolCountersInstance GetInstance(string instanceName)
		{
			return (ServiceProxyPoolCountersInstance)ServiceProxyPoolCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ServiceProxyPoolCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ServiceProxyPoolCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ServiceProxyPoolCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ServiceProxyPoolCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ServiceProxyPoolCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ServiceProxyPoolCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ServiceProxyPoolCountersInstance(instanceName, (ServiceProxyPoolCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ServiceProxyPoolCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ServiceProxyPoolCounters.counters == null)
			{
				return;
			}
			ServiceProxyPoolCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange ServiceProxyPool";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange ServiceProxyPool", new CreateInstanceDelegate(ServiceProxyPoolCounters.CreateInstance));
	}
}
