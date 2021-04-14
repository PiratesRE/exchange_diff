using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxyPerArrayCounters
	{
		public static HttpProxyPerArrayCountersInstance GetInstance(string instanceName)
		{
			return (HttpProxyPerArrayCountersInstance)HttpProxyPerArrayCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			HttpProxyPerArrayCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return HttpProxyPerArrayCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return HttpProxyPerArrayCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			HttpProxyPerArrayCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			HttpProxyPerArrayCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			HttpProxyPerArrayCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new HttpProxyPerArrayCountersInstance(instanceName, (HttpProxyPerArrayCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new HttpProxyPerArrayCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (HttpProxyPerArrayCounters.counters == null)
			{
				return;
			}
			HttpProxyPerArrayCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange HttpProxy Per Array";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange HttpProxy Per Array", new CreateInstanceDelegate(HttpProxyPerArrayCounters.CreateInstance));
	}
}
