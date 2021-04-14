using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class ResolverPerfCounters
	{
		public static ResolverPerfCountersInstance GetInstance(string instanceName)
		{
			return (ResolverPerfCountersInstance)ResolverPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ResolverPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ResolverPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ResolverPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ResolverPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ResolverPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ResolverPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ResolverPerfCountersInstance(instanceName, (ResolverPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ResolverPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ResolverPerfCounters.counters == null)
			{
				return;
			}
			ResolverPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Resolver";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeTransport Resolver", new CreateInstanceDelegate(ResolverPerfCounters.CreateInstance));
	}
}
