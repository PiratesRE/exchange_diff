using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpReceivePerfCounters
	{
		public static SmtpReceivePerfCountersInstance GetInstance(string instanceName)
		{
			return (SmtpReceivePerfCountersInstance)SmtpReceivePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SmtpReceivePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SmtpReceivePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SmtpReceivePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SmtpReceivePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SmtpReceivePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SmtpReceivePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SmtpReceivePerfCountersInstance(instanceName, (SmtpReceivePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SmtpReceivePerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (SmtpReceivePerfCounters.counters == null)
			{
				SmtpReceivePerfCounters.CategoryName = categoryName;
				SmtpReceivePerfCounters.counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal(SmtpReceivePerfCounters.CategoryName, new CreateInstanceDelegate(SmtpReceivePerfCounters.CreateInstance));
			}
		}

		public static SmtpReceivePerfCountersInstance TotalInstance
		{
			get
			{
				return (SmtpReceivePerfCountersInstance)SmtpReceivePerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpReceivePerfCounters.counters == null)
			{
				return;
			}
			SmtpReceivePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters;
	}
}
