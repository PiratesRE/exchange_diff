using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class RemotePowershellPerformanceCounters
	{
		public static RemotePowershellPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (RemotePowershellPerformanceCountersInstance)RemotePowershellPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			RemotePowershellPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return RemotePowershellPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return RemotePowershellPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			RemotePowershellPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			RemotePowershellPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			RemotePowershellPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new RemotePowershellPerformanceCountersInstance(instanceName, (RemotePowershellPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new RemotePowershellPerformanceCountersInstance(instanceName);
		}

		public static RemotePowershellPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (RemotePowershellPerformanceCountersInstance)RemotePowershellPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (RemotePowershellPerformanceCounters.counters == null)
			{
				return;
			}
			RemotePowershellPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeRemotePowershell";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeRemotePowershell", new CreateInstanceDelegate(RemotePowershellPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(RemotePowershellPerformanceCounters.CreateTotalInstance));
	}
}
