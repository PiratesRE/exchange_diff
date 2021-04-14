using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class AuthenticationCounters
	{
		public static AuthenticationCountersInstance GetInstance(string instanceName)
		{
			return (AuthenticationCountersInstance)AuthenticationCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			AuthenticationCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return AuthenticationCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return AuthenticationCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			AuthenticationCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			AuthenticationCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			AuthenticationCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new AuthenticationCountersInstance(instanceName, (AuthenticationCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new AuthenticationCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (AuthenticationCounters.counters == null)
			{
				return;
			}
			AuthenticationCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Authentication";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Authentication", new CreateInstanceDelegate(AuthenticationCounters.CreateInstance));
	}
}
