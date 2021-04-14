using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal static class MExCounters
	{
		public static MExCountersInstance GetInstance(string instanceName)
		{
			return (MExCountersInstance)MExCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MExCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MExCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MExCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MExCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MExCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MExCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MExCountersInstance(instanceName, (MExCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MExCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (MExCounters.counters == null)
			{
				MExCounters.CategoryName = categoryName;
				MExCounters.counters = new PerformanceCounterMultipleInstance(MExCounters.CategoryName, new CreateInstanceDelegate(MExCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MExCounters.counters == null)
			{
				return;
			}
			MExCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
