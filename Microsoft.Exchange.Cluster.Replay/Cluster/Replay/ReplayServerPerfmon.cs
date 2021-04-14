using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ReplayServerPerfmon
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ReplayServerPerfmon.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ReplayServerPerfmon.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Replication Server";

		public static readonly ExPerformanceCounter GetCopyStatusServerCalls = new ExPerformanceCounter("MSExchange Replication Server", "GetCopyStatus Server-Side Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetCopyStatusServerCallsPerSec = new ExPerformanceCounter("MSExchange Replication Server", "GetCopyStatus Server-Side Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetServerForDatabaseCalls = new ExPerformanceCounter("MSExchange Replication Server", "WCF GetServerForDatabase Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetServerForDatabaseCallsPerSec = new ExPerformanceCounter("MSExchange Replication Server", "WCF GetServerForDatabase Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetAllCalls = new ExPerformanceCounter("MSExchange Replication Server", "WCF GetActiveCopiesForDatabaseAvailabilityGroup Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetAllCallsPerSec = new ExPerformanceCounter("MSExchange Replication Server", "WCF GetActiveCopiesForDatabaseAvailabilityGroup Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetServerForDatabaseCallErrors = new ExPerformanceCounter("MSExchange Replication Server", "WCF Calls returning an error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WCFGetServerForDatabaseCallErrorsPerSec = new ExPerformanceCounter("MSExchange Replication Server", "WCF Calls/sec returning an error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgWCFCallLatency = new ExPerformanceCounter("MSExchange Replication Server", "Average WCF GetServerForDatabase call latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgWCFCallLatencyBase = new ExPerformanceCounter("MSExchange Replication Server", "Base for AvgWCFCallLatency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgWCFGetAllCallLatency = new ExPerformanceCounter("MSExchange Replication Server", "Average WCF GetActiveCopiesForDatabaseAvailabilityGroup call latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgWCFGetAllCallLatencyBase = new ExPerformanceCounter("MSExchange Replication Server", "Base for AvgWCFGetAllCallLatency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADConfigRefreshCalls = new ExPerformanceCounter("MSExchange Replication Server", "AD Configuration Refresh Operations", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADConfigRefreshCallsPerSec = new ExPerformanceCounter("MSExchange Replication Server", "AD Configuration Refresh Operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADConfigRefreshLatency = new ExPerformanceCounter("MSExchange Replication Server", "Avg. sec/AD Config Refresh", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADConfigRefreshLatencyBase = new ExPerformanceCounter("MSExchange Replication Server", "Base not visible", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ReplayServerPerfmon.GetCopyStatusServerCalls,
			ReplayServerPerfmon.GetCopyStatusServerCallsPerSec,
			ReplayServerPerfmon.WCFGetServerForDatabaseCalls,
			ReplayServerPerfmon.WCFGetServerForDatabaseCallsPerSec,
			ReplayServerPerfmon.WCFGetAllCalls,
			ReplayServerPerfmon.WCFGetAllCallsPerSec,
			ReplayServerPerfmon.WCFGetServerForDatabaseCallErrors,
			ReplayServerPerfmon.WCFGetServerForDatabaseCallErrorsPerSec,
			ReplayServerPerfmon.AvgWCFCallLatency,
			ReplayServerPerfmon.AvgWCFCallLatencyBase,
			ReplayServerPerfmon.AvgWCFGetAllCallLatency,
			ReplayServerPerfmon.AvgWCFGetAllCallLatencyBase,
			ReplayServerPerfmon.ADConfigRefreshCalls,
			ReplayServerPerfmon.ADConfigRefreshCallsPerSec,
			ReplayServerPerfmon.ADConfigRefreshLatency,
			ReplayServerPerfmon.ADConfigRefreshLatencyBase
		};
	}
}
