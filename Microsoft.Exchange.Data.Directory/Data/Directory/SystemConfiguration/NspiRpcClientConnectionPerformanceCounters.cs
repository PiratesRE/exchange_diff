using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class NspiRpcClientConnectionPerformanceCounters
	{
		public static NspiRpcClientConnectionPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (NspiRpcClientConnectionPerformanceCountersInstance)NspiRpcClientConnectionPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			NspiRpcClientConnectionPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return NspiRpcClientConnectionPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return NspiRpcClientConnectionPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			NspiRpcClientConnectionPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			NspiRpcClientConnectionPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			NspiRpcClientConnectionPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new NspiRpcClientConnectionPerformanceCountersInstance(instanceName, (NspiRpcClientConnectionPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new NspiRpcClientConnectionPerformanceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (NspiRpcClientConnectionPerformanceCounters.counters == null)
			{
				return;
			}
			NspiRpcClientConnectionPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange NSPI RPC Client Connections";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange NSPI RPC Client Connections", new CreateInstanceDelegate(NspiRpcClientConnectionPerformanceCounters.CreateInstance));
	}
}
