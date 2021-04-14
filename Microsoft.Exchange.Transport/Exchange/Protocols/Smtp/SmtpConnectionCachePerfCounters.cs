using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpConnectionCachePerfCounters
	{
		public static SmtpConnectionCachePerfCountersInstance GetInstance(string instanceName)
		{
			return (SmtpConnectionCachePerfCountersInstance)SmtpConnectionCachePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SmtpConnectionCachePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SmtpConnectionCachePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SmtpConnectionCachePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SmtpConnectionCachePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SmtpConnectionCachePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SmtpConnectionCachePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SmtpConnectionCachePerfCountersInstance(instanceName, (SmtpConnectionCachePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SmtpConnectionCachePerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (SmtpConnectionCachePerfCounters.counters == null)
			{
				SmtpConnectionCachePerfCounters.CategoryName = categoryName;
				SmtpConnectionCachePerfCounters.counters = new PerformanceCounterMultipleInstance(SmtpConnectionCachePerfCounters.CategoryName, new CreateInstanceDelegate(SmtpConnectionCachePerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpConnectionCachePerfCounters.counters == null)
			{
				return;
			}
			SmtpConnectionCachePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
