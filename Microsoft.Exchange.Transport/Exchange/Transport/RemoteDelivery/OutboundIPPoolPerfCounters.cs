using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal static class OutboundIPPoolPerfCounters
	{
		public static OutboundIPPoolPerfCountersInstance GetInstance(string instanceName)
		{
			return (OutboundIPPoolPerfCountersInstance)OutboundIPPoolPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			OutboundIPPoolPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return OutboundIPPoolPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return OutboundIPPoolPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			OutboundIPPoolPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			OutboundIPPoolPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			OutboundIPPoolPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new OutboundIPPoolPerfCountersInstance(instanceName, (OutboundIPPoolPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new OutboundIPPoolPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (OutboundIPPoolPerfCounters.counters == null)
			{
				return;
			}
			OutboundIPPoolPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Outbound IP Pools";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport Outbound IP Pools", new CreateInstanceDelegate(OutboundIPPoolPerfCounters.CreateInstance));
	}
}
