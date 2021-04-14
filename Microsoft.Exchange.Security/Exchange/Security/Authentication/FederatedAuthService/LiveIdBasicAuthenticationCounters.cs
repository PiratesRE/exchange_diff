using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal static class LiveIdBasicAuthenticationCounters
	{
		public static LiveIdBasicAuthenticationCountersInstance GetInstance(string instanceName)
		{
			return (LiveIdBasicAuthenticationCountersInstance)LiveIdBasicAuthenticationCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			LiveIdBasicAuthenticationCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return LiveIdBasicAuthenticationCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return LiveIdBasicAuthenticationCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			LiveIdBasicAuthenticationCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			LiveIdBasicAuthenticationCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			LiveIdBasicAuthenticationCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new LiveIdBasicAuthenticationCountersInstance(instanceName, (LiveIdBasicAuthenticationCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new LiveIdBasicAuthenticationCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (LiveIdBasicAuthenticationCounters.counters == null)
			{
				return;
			}
			LiveIdBasicAuthenticationCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange LiveIdBasicAuthentication";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange LiveIdBasicAuthentication", new CreateInstanceDelegate(LiveIdBasicAuthenticationCounters.CreateInstance));
	}
}
