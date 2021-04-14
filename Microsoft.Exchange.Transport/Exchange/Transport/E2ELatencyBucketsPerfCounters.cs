using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class E2ELatencyBucketsPerfCounters
	{
		public static E2ELatencyBucketsPerfCountersInstance GetInstance(string instanceName)
		{
			return (E2ELatencyBucketsPerfCountersInstance)E2ELatencyBucketsPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			E2ELatencyBucketsPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return E2ELatencyBucketsPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return E2ELatencyBucketsPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			E2ELatencyBucketsPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			E2ELatencyBucketsPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			E2ELatencyBucketsPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new E2ELatencyBucketsPerfCountersInstance(instanceName, (E2ELatencyBucketsPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new E2ELatencyBucketsPerfCountersInstance(instanceName);
		}

		public static E2ELatencyBucketsPerfCountersInstance TotalInstance
		{
			get
			{
				return (E2ELatencyBucketsPerfCountersInstance)E2ELatencyBucketsPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (E2ELatencyBucketsPerfCounters.counters == null)
			{
				return;
			}
			E2ELatencyBucketsPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport E2E Latency Buckets";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTransport E2E Latency Buckets", new CreateInstanceDelegate(E2ELatencyBucketsPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(E2ELatencyBucketsPerfCounters.CreateTotalInstance));
	}
}
