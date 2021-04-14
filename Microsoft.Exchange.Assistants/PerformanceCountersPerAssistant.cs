using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal static class PerformanceCountersPerAssistant
	{
		public static PerformanceCountersPerAssistantInstance GetInstance(string instanceName)
		{
			return (PerformanceCountersPerAssistantInstance)PerformanceCountersPerAssistant.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PerformanceCountersPerAssistant.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PerformanceCountersPerAssistant.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PerformanceCountersPerAssistant.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PerformanceCountersPerAssistant.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PerformanceCountersPerAssistant.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PerformanceCountersPerAssistant.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PerformanceCountersPerAssistantInstance(instanceName, (PerformanceCountersPerAssistantInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PerformanceCountersPerAssistantInstance(instanceName);
		}

		public static PerformanceCountersPerAssistantInstance TotalInstance
		{
			get
			{
				return (PerformanceCountersPerAssistantInstance)PerformanceCountersPerAssistant.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerformanceCountersPerAssistant.counters == null)
			{
				return;
			}
			PerformanceCountersPerAssistant.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Assistants - Per Assistant";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Assistants - Per Assistant", new CreateInstanceDelegate(PerformanceCountersPerAssistant.CreateInstance), new CreateTotalInstanceDelegate(PerformanceCountersPerAssistant.CreateTotalInstance));
	}
}
