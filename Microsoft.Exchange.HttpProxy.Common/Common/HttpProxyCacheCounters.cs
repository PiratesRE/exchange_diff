using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxyCacheCounters
	{
		public static HttpProxyCacheCountersInstance GetInstance(string instanceName)
		{
			return (HttpProxyCacheCountersInstance)HttpProxyCacheCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			HttpProxyCacheCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return HttpProxyCacheCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return HttpProxyCacheCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			HttpProxyCacheCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			HttpProxyCacheCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			HttpProxyCacheCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new HttpProxyCacheCountersInstance(instanceName, (HttpProxyCacheCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new HttpProxyCacheCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (HttpProxyCacheCounters.counters == null)
			{
				return;
			}
			HttpProxyCacheCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange HttpProxy Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange HttpProxy Cache", new CreateInstanceDelegate(HttpProxyCacheCounters.CreateInstance));
	}
}
