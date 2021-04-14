using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal static class IsMemberOfResolverPerfCounters
	{
		public static IsMemberOfResolverPerfCountersInstance GetInstance(string instanceName)
		{
			return (IsMemberOfResolverPerfCountersInstance)IsMemberOfResolverPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			IsMemberOfResolverPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return IsMemberOfResolverPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return IsMemberOfResolverPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			IsMemberOfResolverPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			IsMemberOfResolverPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			IsMemberOfResolverPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new IsMemberOfResolverPerfCountersInstance(instanceName, (IsMemberOfResolverPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new IsMemberOfResolverPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (IsMemberOfResolverPerfCounters.counters == null)
			{
				return;
			}
			IsMemberOfResolverPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "Expanded Groups Cache";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("Expanded Groups Cache", new CreateInstanceDelegate(IsMemberOfResolverPerfCounters.CreateInstance));
	}
}
