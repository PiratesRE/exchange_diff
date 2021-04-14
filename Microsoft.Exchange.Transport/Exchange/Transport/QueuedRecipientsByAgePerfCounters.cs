using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class QueuedRecipientsByAgePerfCounters
	{
		public static QueuedRecipientsByAgePerfCountersInstance GetInstance(string instanceName)
		{
			return (QueuedRecipientsByAgePerfCountersInstance)QueuedRecipientsByAgePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			QueuedRecipientsByAgePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return QueuedRecipientsByAgePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return QueuedRecipientsByAgePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			QueuedRecipientsByAgePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			QueuedRecipientsByAgePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			QueuedRecipientsByAgePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new QueuedRecipientsByAgePerfCountersInstance(instanceName, (QueuedRecipientsByAgePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new QueuedRecipientsByAgePerfCountersInstance(instanceName);
		}

		public static QueuedRecipientsByAgePerfCountersInstance TotalInstance
		{
			get
			{
				return (QueuedRecipientsByAgePerfCountersInstance)QueuedRecipientsByAgePerfCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (QueuedRecipientsByAgePerfCounters.counters == null)
			{
				return;
			}
			QueuedRecipientsByAgePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeTransport Queued Recipients By Age";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeTransport Queued Recipients By Age", new CreateInstanceDelegate(QueuedRecipientsByAgePerfCounters.CreateInstance), new CreateTotalInstanceDelegate(QueuedRecipientsByAgePerfCounters.CreateTotalInstance));
	}
}
