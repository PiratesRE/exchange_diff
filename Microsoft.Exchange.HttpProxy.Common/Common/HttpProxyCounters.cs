using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxyCounters
	{
		public static HttpProxyCountersInstance GetInstance(string instanceName)
		{
			return (HttpProxyCountersInstance)HttpProxyCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			HttpProxyCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return HttpProxyCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return HttpProxyCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			HttpProxyCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			HttpProxyCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			HttpProxyCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new HttpProxyCountersInstance(instanceName, (HttpProxyCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new HttpProxyCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (HttpProxyCounters.counters == null)
			{
				return;
			}
			HttpProxyCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange HttpProxy";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange HttpProxy", new CreateInstanceDelegate(HttpProxyCounters.CreateInstance));
	}
}
