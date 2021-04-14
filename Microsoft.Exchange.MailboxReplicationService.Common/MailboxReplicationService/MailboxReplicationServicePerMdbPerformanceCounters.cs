using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MailboxReplicationServicePerMdbPerformanceCounters
	{
		public static MailboxReplicationServicePerMdbPerformanceCountersInstance GetInstance(string instanceName)
		{
			return (MailboxReplicationServicePerMdbPerformanceCountersInstance)MailboxReplicationServicePerMdbPerformanceCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MailboxReplicationServicePerMdbPerformanceCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MailboxReplicationServicePerMdbPerformanceCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MailboxReplicationServicePerMdbPerformanceCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MailboxReplicationServicePerMdbPerformanceCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MailboxReplicationServicePerMdbPerformanceCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MailboxReplicationServicePerMdbPerformanceCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MailboxReplicationServicePerMdbPerformanceCountersInstance(instanceName, (MailboxReplicationServicePerMdbPerformanceCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MailboxReplicationServicePerMdbPerformanceCountersInstance(instanceName);
		}

		public static MailboxReplicationServicePerMdbPerformanceCountersInstance TotalInstance
		{
			get
			{
				return (MailboxReplicationServicePerMdbPerformanceCountersInstance)MailboxReplicationServicePerMdbPerformanceCounters.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MailboxReplicationServicePerMdbPerformanceCounters.counters == null)
			{
				return;
			}
			MailboxReplicationServicePerMdbPerformanceCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Mailbox Replication Service Per Mdb";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Mailbox Replication Service Per Mdb", new CreateInstanceDelegate(MailboxReplicationServicePerMdbPerformanceCounters.CreateInstance), new CreateTotalInstanceDelegate(MailboxReplicationServicePerMdbPerformanceCounters.CreateTotalInstance));
	}
}
