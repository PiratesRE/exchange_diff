using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	internal static class MailboxLoadBalanceMultiInstancePerformanceCounters
	{
		public static MailboxLoadBalanceMultiInstancePerformanceCountersInstance GetInstance(string instanceName)
		{
			return (MailboxLoadBalanceMultiInstancePerformanceCountersInstance)MailboxLoadBalanceMultiInstancePerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MailboxLoadBalanceMultiInstancePerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MailboxLoadBalanceMultiInstancePerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MailboxLoadBalanceMultiInstancePerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MailboxLoadBalanceMultiInstancePerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MailboxLoadBalanceMultiInstancePerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MailboxLoadBalanceMultiInstancePerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MailboxLoadBalanceMultiInstancePerformanceCountersInstance(instanceName, (MailboxLoadBalanceMultiInstancePerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MailboxLoadBalanceMultiInstancePerformanceCountersInstance(instanceName);
		}

		public static MailboxLoadBalanceMultiInstancePerformanceCountersInstance TotalInstance
		{
			get
			{
				return (MailboxLoadBalanceMultiInstancePerformanceCountersInstance)MailboxLoadBalanceMultiInstancePerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MailboxLoadBalanceMultiInstancePerformanceCounters.counters == null)
			{
				return;
			}
			MailboxLoadBalanceMultiInstancePerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Mailbox Load Balancing Queues";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Mailbox Load Balancing Queues", new CreateInstanceDelegate(MailboxLoadBalanceMultiInstancePerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(MailboxLoadBalanceMultiInstancePerformanceCounters.CreateTotalInstance));
	}
}
