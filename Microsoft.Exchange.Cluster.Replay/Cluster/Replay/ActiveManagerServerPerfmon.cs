using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ActiveManagerServerPerfmon
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveManagerServerPerfmon.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ActiveManagerServerPerfmon.AllCounters)
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

		public const string CategoryName = "MSExchange Active Manager Server";

		public static readonly ExPerformanceCounter GetServerForDatabaseServerCalls = new ExPerformanceCounter("MSExchange Active Manager Server", "GetServerForDatabase Server-Side Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetServerForDatabaseServerCallsPerSec = new ExPerformanceCounter("MSExchange Active Manager Server", "Server-Side Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DatabaseStateInfoWrites = new ExPerformanceCounter("MSExchange Active Manager Server", "Active Manager Database State writes to Persistent storage", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DatabaseStateInfoWritesPerSec = new ExPerformanceCounter("MSExchange Active Manager Server", "Active Manager Database State writes to Persistent storage/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CountOfDatabases = new ExPerformanceCounter("MSExchange Active Manager Server", "Total Number of Databases", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveManagerRole = new ExPerformanceCounter("MSExchange Active Manager Server", "Active Manager Role", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClusterBatchWriteCalls = new ExPerformanceCounter("MSExchange Active Manager Server", "All cluster batch writes issued on the local node", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastLogRemoteUpdateRpcAttempted = new ExPerformanceCounter("MSExchange Active Manager Server", "LastLog cluster batch remote updates attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastLogRemoteUpdateRpcFailed = new ExPerformanceCounter("MSExchange Active Manager Server", "LastLog cluster batch remote updates failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastLogLocalClusterBatchUpdatesAttempted = new ExPerformanceCounter("MSExchange Active Manager Server", "LastLog cluster batch local updates attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastLogLocalClusterBatchUpdatesFailed = new ExPerformanceCounter("MSExchange Active Manager Server", "LastLog cluster batch local updates failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ActiveManagerServerPerfmon.GetServerForDatabaseServerCalls,
			ActiveManagerServerPerfmon.GetServerForDatabaseServerCallsPerSec,
			ActiveManagerServerPerfmon.DatabaseStateInfoWrites,
			ActiveManagerServerPerfmon.DatabaseStateInfoWritesPerSec,
			ActiveManagerServerPerfmon.CountOfDatabases,
			ActiveManagerServerPerfmon.ActiveManagerRole,
			ActiveManagerServerPerfmon.ClusterBatchWriteCalls,
			ActiveManagerServerPerfmon.LastLogRemoteUpdateRpcAttempted,
			ActiveManagerServerPerfmon.LastLogRemoteUpdateRpcFailed,
			ActiveManagerServerPerfmon.LastLogLocalClusterBatchUpdatesAttempted,
			ActiveManagerServerPerfmon.LastLogLocalClusterBatchUpdatesFailed
		};
	}
}
