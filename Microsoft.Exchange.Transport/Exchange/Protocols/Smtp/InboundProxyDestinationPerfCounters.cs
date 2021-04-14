using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class InboundProxyDestinationPerfCounters
	{
		public static InboundProxyDestinationPerfCountersInstance GetInstance(string instanceName)
		{
			return (InboundProxyDestinationPerfCountersInstance)InboundProxyDestinationPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			InboundProxyDestinationPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return InboundProxyDestinationPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return InboundProxyDestinationPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			InboundProxyDestinationPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			InboundProxyDestinationPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			InboundProxyDestinationPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new InboundProxyDestinationPerfCountersInstance(instanceName, (InboundProxyDestinationPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new InboundProxyDestinationPerfCountersInstance(instanceName);
		}

		public static InboundProxyDestinationPerfCountersInstance TotalInstance
		{
			get
			{
				return (InboundProxyDestinationPerfCountersInstance)InboundProxyDestinationPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (InboundProxyDestinationPerfCounters.counters == null)
			{
				return;
			}
			InboundProxyDestinationPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeFrontendTransport InboundProxyDestinations";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeFrontendTransport InboundProxyDestinations", new CreateInstanceDelegate(InboundProxyDestinationPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(InboundProxyDestinationPerfCounters.CreateTotalInstance));
	}
}
