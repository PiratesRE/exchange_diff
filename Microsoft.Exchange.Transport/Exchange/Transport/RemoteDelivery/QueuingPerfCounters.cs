using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal static class QueuingPerfCounters
	{
		public static QueuingPerfCountersInstance GetInstance(string instanceName)
		{
			return (QueuingPerfCountersInstance)QueuingPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			QueuingPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return QueuingPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return QueuingPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			QueuingPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			QueuingPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			QueuingPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new QueuingPerfCountersInstance(instanceName, (QueuingPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new QueuingPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (QueuingPerfCounters.counters == null)
			{
				return;
			}
			QueuingPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Queues";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport Queues", new CreateInstanceDelegate(QueuingPerfCounters.CreateInstance));
	}
}
