using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpReceiveFrontendPerfCounters
	{
		public static SmtpReceiveFrontendPerfCountersInstance GetInstance(string instanceName)
		{
			return (SmtpReceiveFrontendPerfCountersInstance)SmtpReceiveFrontendPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SmtpReceiveFrontendPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SmtpReceiveFrontendPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SmtpReceiveFrontendPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SmtpReceiveFrontendPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SmtpReceiveFrontendPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SmtpReceiveFrontendPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SmtpReceiveFrontendPerfCountersInstance(instanceName, (SmtpReceiveFrontendPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SmtpReceiveFrontendPerfCountersInstance(instanceName);
		}

		public static SmtpReceiveFrontendPerfCountersInstance TotalInstance
		{
			get
			{
				return (SmtpReceiveFrontendPerfCountersInstance)SmtpReceiveFrontendPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpReceiveFrontendPerfCounters.counters == null)
			{
				return;
			}
			SmtpReceiveFrontendPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeFrontEndTransport SMTPReceive";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeFrontEndTransport SMTPReceive", new CreateInstanceDelegate(SmtpReceiveFrontendPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(SmtpReceiveFrontendPerfCounters.CreateTotalInstance));
	}
}
