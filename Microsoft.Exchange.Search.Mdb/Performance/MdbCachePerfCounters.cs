using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Performance
{
	internal static class MdbCachePerfCounters
	{
		public static MdbCachePerfCountersInstance GetInstance(string instanceName)
		{
			return (MdbCachePerfCountersInstance)MdbCachePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MdbCachePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MdbCachePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MdbCachePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MdbCachePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MdbCachePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MdbCachePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MdbCachePerfCountersInstance(instanceName, (MdbCachePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MdbCachePerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MdbCachePerfCounters.counters == null)
			{
				return;
			}
			MdbCachePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeSearch MailboxSession Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeSearch MailboxSession Cache", new CreateInstanceDelegate(MdbCachePerfCounters.CreateInstance));
	}
}
