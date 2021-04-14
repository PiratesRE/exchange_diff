using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxyPerSiteCounters
	{
		public static HttpProxyPerSiteCountersInstance GetInstance(string instanceName)
		{
			return (HttpProxyPerSiteCountersInstance)HttpProxyPerSiteCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			HttpProxyPerSiteCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return HttpProxyPerSiteCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return HttpProxyPerSiteCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			HttpProxyPerSiteCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			HttpProxyPerSiteCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			HttpProxyPerSiteCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new HttpProxyPerSiteCountersInstance(instanceName, (HttpProxyPerSiteCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new HttpProxyPerSiteCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (HttpProxyPerSiteCounters.counters == null)
			{
				return;
			}
			HttpProxyPerSiteCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange HttpProxy Per Site";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange HttpProxy Per Site", new CreateInstanceDelegate(HttpProxyPerSiteCounters.CreateInstance));
	}
}
