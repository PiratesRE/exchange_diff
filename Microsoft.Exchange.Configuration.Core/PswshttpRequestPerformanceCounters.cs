using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class PswshttpRequestPerformanceCounters
	{
		public static PswshttpRequestPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (PswshttpRequestPerformanceCountersInstance)PswshttpRequestPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PswshttpRequestPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PswshttpRequestPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PswshttpRequestPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PswshttpRequestPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PswshttpRequestPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PswshttpRequestPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PswshttpRequestPerformanceCountersInstance(instanceName, (PswshttpRequestPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PswshttpRequestPerformanceCountersInstance(instanceName);
		}

		public static PswshttpRequestPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (PswshttpRequestPerformanceCountersInstance)PswshttpRequestPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PswshttpRequestPerformanceCounters.counters == null)
			{
				return;
			}
			PswshttpRequestPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangePowershellWebServiceHttpRequest";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangePowershellWebServiceHttpRequest", new CreateInstanceDelegate(PswshttpRequestPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(PswshttpRequestPerformanceCounters.CreateTotalInstance));
	}
}
