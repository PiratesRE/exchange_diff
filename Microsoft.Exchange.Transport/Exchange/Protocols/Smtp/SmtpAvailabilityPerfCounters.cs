using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpAvailabilityPerfCounters
	{
		public static SmtpAvailabilityPerfCountersInstance GetInstance(string instanceName)
		{
			return (SmtpAvailabilityPerfCountersInstance)SmtpAvailabilityPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SmtpAvailabilityPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SmtpAvailabilityPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SmtpAvailabilityPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SmtpAvailabilityPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SmtpAvailabilityPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SmtpAvailabilityPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SmtpAvailabilityPerfCountersInstance(instanceName, (SmtpAvailabilityPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SmtpAvailabilityPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (SmtpAvailabilityPerfCounters.counters == null)
			{
				SmtpAvailabilityPerfCounters.CategoryName = categoryName;
				SmtpAvailabilityPerfCounters.counters = new PerformanceCounterMultipleInstance(SmtpAvailabilityPerfCounters.CategoryName, new CreateInstanceDelegate(SmtpAvailabilityPerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpAvailabilityPerfCounters.counters == null)
			{
				return;
			}
			SmtpAvailabilityPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
