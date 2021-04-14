using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	internal static class PhysicalAccessPerformanceCounters
	{
		public static PhysicalAccessPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (PhysicalAccessPerformanceCountersInstance)PhysicalAccessPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PhysicalAccessPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PhysicalAccessPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PhysicalAccessPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PhysicalAccessPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PhysicalAccessPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PhysicalAccessPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PhysicalAccessPerformanceCountersInstance(instanceName, (PhysicalAccessPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PhysicalAccessPerformanceCountersInstance(instanceName);
		}

		public static PhysicalAccessPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (PhysicalAccessPerformanceCountersInstance)PhysicalAccessPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PhysicalAccessPerformanceCounters.counters == null)
			{
				return;
			}
			PhysicalAccessPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeIS Physical Access";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeIS Physical Access", new CreateInstanceDelegate(PhysicalAccessPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(PhysicalAccessPerformanceCounters.CreateTotalInstance));
	}
}
