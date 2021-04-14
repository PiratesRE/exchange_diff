using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class StorePerClientTypePerformanceCounters
	{
		public static StorePerClientTypePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (StorePerClientTypePerformanceCountersInstance)StorePerClientTypePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			StorePerClientTypePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return StorePerClientTypePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return StorePerClientTypePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			StorePerClientTypePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			StorePerClientTypePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			StorePerClientTypePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new StorePerClientTypePerformanceCountersInstance(instanceName, (StorePerClientTypePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new StorePerClientTypePerformanceCountersInstance(instanceName);
		}

		public static StorePerClientTypePerformanceCountersInstance TotalInstance
		{
			get
			{
				return (StorePerClientTypePerformanceCountersInstance)StorePerClientTypePerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (StorePerClientTypePerformanceCounters.counters == null)
			{
				return;
			}
			StorePerClientTypePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeIS Client Type";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeIS Client Type", new CreateInstanceDelegate(StorePerClientTypePerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(StorePerClientTypePerformanceCounters.CreateTotalInstance));
	}
}
