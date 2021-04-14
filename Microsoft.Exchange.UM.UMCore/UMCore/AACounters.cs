using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class AACounters
	{
		public static AACountersInstance GetInstance(string instanceName)
		{
			return (AACountersInstance)AACounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			AACounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return AACounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return AACounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			AACounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			AACounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			AACounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new AACountersInstance(instanceName, (AACountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new AACountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (AACounters.counters == null)
			{
				return;
			}
			AACounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeUMAutoAttendant";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeUMAutoAttendant", new CreateInstanceDelegate(AACounters.CreateInstance));
	}
}
