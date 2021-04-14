using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Performance
{
	internal static class StatefulComponentPerformanceCounters
	{
		public static StatefulComponentPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (StatefulComponentPerformanceCountersInstance)StatefulComponentPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			StatefulComponentPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return StatefulComponentPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return StatefulComponentPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			StatefulComponentPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			StatefulComponentPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			StatefulComponentPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new StatefulComponentPerformanceCountersInstance(instanceName, (StatefulComponentPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new StatefulComponentPerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (StatefulComponentPerformanceCounters.counters == null)
			{
				return;
			}
			StatefulComponentPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeInference StatefulComponent";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeInference StatefulComponent", new CreateInstanceDelegate(StatefulComponentPerformanceCounters.CreateInstance));
	}
}
