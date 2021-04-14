using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpSendPerfCounters
	{
		public static SmtpSendPerfCountersInstance GetInstance(string instanceName)
		{
			return (SmtpSendPerfCountersInstance)SmtpSendPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SmtpSendPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SmtpSendPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SmtpSendPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SmtpSendPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SmtpSendPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SmtpSendPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SmtpSendPerfCountersInstance(instanceName, (SmtpSendPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SmtpSendPerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (SmtpSendPerfCounters.counters == null)
			{
				SmtpSendPerfCounters.CategoryName = categoryName;
				SmtpSendPerfCounters.counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal(SmtpSendPerfCounters.CategoryName, new CreateInstanceDelegate(SmtpSendPerfCounters.CreateInstance));
			}
		}

		public static SmtpSendPerfCountersInstance TotalInstance
		{
			get
			{
				return (SmtpSendPerfCountersInstance)SmtpSendPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpSendPerfCounters.counters == null)
			{
				return;
			}
			SmtpSendPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters;
	}
}
