using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class QueueQuotaComponentPerfCounters
	{
		public static QueueQuotaComponentPerfCountersInstance GetInstance(string instanceName)
		{
			return (QueueQuotaComponentPerfCountersInstance)QueueQuotaComponentPerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			QueueQuotaComponentPerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return QueueQuotaComponentPerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return QueueQuotaComponentPerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			QueueQuotaComponentPerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			QueueQuotaComponentPerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			QueueQuotaComponentPerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new QueueQuotaComponentPerfCountersInstance(instanceName, (QueueQuotaComponentPerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new QueueQuotaComponentPerfCountersInstance(instanceName);
		}

		public static QueueQuotaComponentPerfCountersInstance TotalInstance
		{
			get
			{
				return (QueueQuotaComponentPerfCountersInstance)QueueQuotaComponentPerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (QueueQuotaComponentPerfCounters.counters == null)
			{
				return;
			}
			QueueQuotaComponentPerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Queue Quota Component";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTransport Queue Quota Component", new CreateInstanceDelegate(QueueQuotaComponentPerfCounters.CreateInstance), new CreateTotalInstanceDelegate(QueueQuotaComponentPerfCounters.CreateTotalInstance));
	}
}
