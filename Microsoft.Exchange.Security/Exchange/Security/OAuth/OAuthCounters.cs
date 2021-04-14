using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class OAuthCounters
	{
		public static OAuthCountersInstance GetInstance(string instanceName)
		{
			return (OAuthCountersInstance)OAuthCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			OAuthCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return OAuthCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return OAuthCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			OAuthCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			OAuthCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			OAuthCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new OAuthCountersInstance(instanceName, (OAuthCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new OAuthCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (OAuthCounters.counters == null)
			{
				return;
			}
			OAuthCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange OAuth";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange OAuth", new CreateInstanceDelegate(OAuthCounters.CreateInstance));
	}
}
