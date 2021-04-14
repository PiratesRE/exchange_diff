using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class ApnsChannelCounters
	{
		public static ApnsChannelCountersInstance GetInstance(string instanceName)
		{
			return (ApnsChannelCountersInstance)ApnsChannelCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ApnsChannelCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ApnsChannelCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ApnsChannelCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ApnsChannelCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ApnsChannelCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ApnsChannelCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ApnsChannelCountersInstance(instanceName, (ApnsChannelCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ApnsChannelCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ApnsChannelCounters.counters == null)
			{
				return;
			}
			ApnsChannelCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Push Notifications Apns Channel";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Push Notifications Apns Channel", new CreateInstanceDelegate(ApnsChannelCounters.CreateInstance));
	}
}
