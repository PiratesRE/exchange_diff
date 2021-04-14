using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SecureMail
{
	internal static class SecureMailTransportPerfCounters
	{
		public static SecureMailTransportPerfCountersInstance GetInstance(string instanceName)
		{
			return (SecureMailTransportPerfCountersInstance)SecureMailTransportPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SecureMailTransportPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SecureMailTransportPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SecureMailTransportPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SecureMailTransportPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SecureMailTransportPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SecureMailTransportPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SecureMailTransportPerfCountersInstance(instanceName, (SecureMailTransportPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SecureMailTransportPerfCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SecureMailTransportPerfCounters.counters == null)
			{
				return;
			}
			SecureMailTransportPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Secure Mail Transport";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Secure Mail Transport", new CreateInstanceDelegate(SecureMailTransportPerfCounters.CreateInstance));
	}
}
