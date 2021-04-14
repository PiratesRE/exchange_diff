using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class MwiLoadBalancerPerformanceCounters
	{
		public static MwiLoadBalancerPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (MwiLoadBalancerPerformanceCountersInstance)MwiLoadBalancerPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MwiLoadBalancerPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MwiLoadBalancerPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MwiLoadBalancerPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MwiLoadBalancerPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MwiLoadBalancerPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MwiLoadBalancerPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MwiLoadBalancerPerformanceCountersInstance(instanceName, (MwiLoadBalancerPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MwiLoadBalancerPerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MwiLoadBalancerPerformanceCounters.counters == null)
			{
				return;
			}
			MwiLoadBalancerPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeUMMessageWaitingIndicator";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeUMMessageWaitingIndicator", new CreateInstanceDelegate(MwiLoadBalancerPerformanceCounters.CreateInstance));
	}
}
