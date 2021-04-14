using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal static class DeliveryAgentPerfCounters
	{
		public static DeliveryAgentPerfCountersInstance GetInstance(string instanceName)
		{
			return (DeliveryAgentPerfCountersInstance)DeliveryAgentPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			DeliveryAgentPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return DeliveryAgentPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return DeliveryAgentPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			DeliveryAgentPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			DeliveryAgentPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			DeliveryAgentPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new DeliveryAgentPerfCountersInstance(instanceName, (DeliveryAgentPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new DeliveryAgentPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (DeliveryAgentPerfCounters.counters == null)
			{
				return;
			}
			DeliveryAgentPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport DeliveryAgent";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport DeliveryAgent", new CreateInstanceDelegate(DeliveryAgentPerfCounters.CreateInstance));
	}
}
