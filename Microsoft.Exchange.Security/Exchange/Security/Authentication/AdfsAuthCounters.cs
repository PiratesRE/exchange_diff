using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class AdfsAuthCounters
	{
		public static AdfsAuthCountersInstance GetInstance(string instanceName)
		{
			return (AdfsAuthCountersInstance)AdfsAuthCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			AdfsAuthCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return AdfsAuthCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return AdfsAuthCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			AdfsAuthCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			AdfsAuthCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			AdfsAuthCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new AdfsAuthCountersInstance(instanceName, (AdfsAuthCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new AdfsAuthCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (AdfsAuthCounters.counters == null)
			{
				return;
			}
			AdfsAuthCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange AdfsAuth";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange AdfsAuth", new CreateInstanceDelegate(AdfsAuthCounters.CreateInstance));
	}
}
