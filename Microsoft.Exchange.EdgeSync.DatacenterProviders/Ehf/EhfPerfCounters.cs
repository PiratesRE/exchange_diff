using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal static class EhfPerfCounters
	{
		public static EhfPerfCountersInstance GetInstance(string instanceName)
		{
			return (EhfPerfCountersInstance)EhfPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			EhfPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return EhfPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return EhfPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			EhfPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			EhfPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			EhfPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new EhfPerfCountersInstance(instanceName, (EhfPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new EhfPerfCountersInstance(instanceName);
		}

		public static EhfPerfCountersInstance TotalInstance
		{
			get
			{
				return (EhfPerfCountersInstance)EhfPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (EhfPerfCounters.counters == null)
			{
				return;
			}
			EhfPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeEdgeSync EHF Sync Operations";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeEdgeSync EHF Sync Operations", new CreateInstanceDelegate(EhfPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(EhfPerfCounters.CreateTotalInstance));
	}
}
