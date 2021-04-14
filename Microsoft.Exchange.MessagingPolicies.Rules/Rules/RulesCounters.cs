using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class RulesCounters
	{
		public static RulesCountersInstance GetInstance(string instanceName)
		{
			return (RulesCountersInstance)RulesCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			RulesCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return RulesCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return RulesCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			RulesCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			RulesCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			RulesCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new RulesCountersInstance(instanceName, (RulesCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new RulesCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (RulesCounters.counters == null)
			{
				return;
			}
			RulesCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Transport Rules";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Transport Rules", new CreateInstanceDelegate(RulesCounters.CreateInstance));
	}
}
