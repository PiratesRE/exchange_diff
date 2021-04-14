using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class OutboundProxyBySourcePerfCounters
	{
		public static OutboundProxyBySourcePerfCountersInstance GetInstance(string instanceName)
		{
			return (OutboundProxyBySourcePerfCountersInstance)OutboundProxyBySourcePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			OutboundProxyBySourcePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return OutboundProxyBySourcePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return OutboundProxyBySourcePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			OutboundProxyBySourcePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			OutboundProxyBySourcePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			OutboundProxyBySourcePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new OutboundProxyBySourcePerfCountersInstance(instanceName, (OutboundProxyBySourcePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new OutboundProxyBySourcePerfCountersInstance(instanceName);
		}

		public static OutboundProxyBySourcePerfCountersInstance TotalInstance
		{
			get
			{
				return (OutboundProxyBySourcePerfCountersInstance)OutboundProxyBySourcePerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (OutboundProxyBySourcePerfCounters.counters == null)
			{
				return;
			}
			OutboundProxyBySourcePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeFrontendTransport OutboundProxyBySource";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeFrontendTransport OutboundProxyBySource", new CreateInstanceDelegate(OutboundProxyBySourcePerfCounters.CreateInstance), new CreateTotalInstanceDelegate(OutboundProxyBySourcePerfCounters.CreateTotalInstance));
	}
}
