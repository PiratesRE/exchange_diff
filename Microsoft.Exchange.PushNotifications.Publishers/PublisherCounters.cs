using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class PublisherCounters
	{
		public static PublisherCountersInstance GetInstance(string instanceName)
		{
			return (PublisherCountersInstance)PublisherCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PublisherCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PublisherCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PublisherCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PublisherCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PublisherCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PublisherCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PublisherCountersInstance(instanceName, (PublisherCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PublisherCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PublisherCounters.counters == null)
			{
				return;
			}
			PublisherCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Push Notifications Publishers";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Push Notifications Publishers", new CreateInstanceDelegate(PublisherCounters.CreateInstance));
	}
}
