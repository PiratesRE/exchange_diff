using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Mserve.Perf
{
	internal static class MserveWebServiceCounters
	{
		public static MserveWebServiceCountersInstance GetInstance(string instanceName)
		{
			return (MserveWebServiceCountersInstance)MserveWebServiceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MserveWebServiceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MserveWebServiceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MserveWebServiceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MserveWebServiceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MserveWebServiceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MserveWebServiceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MserveWebServiceCountersInstance(instanceName, (MserveWebServiceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MserveWebServiceCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MserveWebServiceCounters.counters == null)
			{
				return;
			}
			MserveWebServiceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange MserveWebService";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange MserveWebService", new CreateInstanceDelegate(MserveWebServiceCounters.CreateInstance));
	}
}
