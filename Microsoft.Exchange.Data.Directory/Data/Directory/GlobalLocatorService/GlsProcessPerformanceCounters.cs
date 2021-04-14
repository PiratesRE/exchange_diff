using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal static class GlsProcessPerformanceCounters
	{
		public static GlsProcessPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (GlsProcessPerformanceCountersInstance)GlsProcessPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			GlsProcessPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return GlsProcessPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return GlsProcessPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			GlsProcessPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			GlsProcessPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			GlsProcessPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new GlsProcessPerformanceCountersInstance(instanceName, (GlsProcessPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new GlsProcessPerformanceCountersInstance(instanceName);
		}

		public static GlsProcessPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (GlsProcessPerformanceCountersInstance)GlsProcessPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (GlsProcessPerformanceCounters.counters == null)
			{
				return;
			}
			GlsProcessPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Global Locator Processes";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Global Locator Processes", new CreateInstanceDelegate(GlsProcessPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(GlsProcessPerformanceCounters.CreateTotalInstance));
	}
}
