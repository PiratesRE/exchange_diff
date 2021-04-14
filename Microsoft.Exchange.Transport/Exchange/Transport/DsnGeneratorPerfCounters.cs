using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class DsnGeneratorPerfCounters
	{
		public static DsnGeneratorPerfCountersInstance GetInstance(string instanceName)
		{
			return (DsnGeneratorPerfCountersInstance)DsnGeneratorPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			DsnGeneratorPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return DsnGeneratorPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return DsnGeneratorPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			DsnGeneratorPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			DsnGeneratorPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			DsnGeneratorPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new DsnGeneratorPerfCountersInstance(instanceName, (DsnGeneratorPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new DsnGeneratorPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (DsnGeneratorPerfCounters.counters == null)
			{
				DsnGeneratorPerfCounters.CategoryName = categoryName;
				DsnGeneratorPerfCounters.counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal(DsnGeneratorPerfCounters.CategoryName, new CreateInstanceDelegate(DsnGeneratorPerfCounters.CreateInstance));
			}
		}

		public static DsnGeneratorPerfCountersInstance TotalInstance
		{
			get
			{
				return (DsnGeneratorPerfCountersInstance)DsnGeneratorPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (DsnGeneratorPerfCounters.counters == null)
			{
				return;
			}
			DsnGeneratorPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters;
	}
}
