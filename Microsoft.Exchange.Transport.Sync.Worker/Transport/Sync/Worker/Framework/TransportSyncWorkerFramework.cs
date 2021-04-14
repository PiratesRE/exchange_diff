using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class TransportSyncWorkerFramework
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (TransportSyncWorkerFramework.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in TransportSyncWorkerFramework.AllCounters)
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

		public const string CategoryName = "MSExchange Transport Sync Worker Framework";

		public static readonly ExPerformanceCounter AverageSyncTime = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Average Processing Time (sec) to sync a subscription", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSyncTimeBase = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Average Processing Time Base (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastSyncTime = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Last Processing Time (msec) to sync a subscription", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutstandingSyncs = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Subscriptions Syncing", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CanceledSyncs = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Canceled", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedSyncs = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Failed", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SuccessfulSyncsPerSec = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Successful Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulSyncs = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Successful", string.Empty, null, new ExPerformanceCounter[]
		{
			TransportSyncWorkerFramework.SuccessfulSyncsPerSec
		});

		public static readonly ExPerformanceCounter TemporaryFailedSyncs = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Failed Transiently", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecoverySyncsNeeded = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Recovery Syncs Needed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageDeleteTime = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Average Processing Time (sec) to delete a subscription", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageDeleteTimeBase = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Average Delete Processing Time Base (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastDeleteTime = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Last Processing Time (msec) to delete a subscription", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutstandingDeletes = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Framework - Subscriptions Deleting", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedDeletes = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Deletes Failed", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SuccessfulDeletesPerSec = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Successful Deletes Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulDeletes = new ExPerformanceCounter("MSExchange Transport Sync Worker Framework", "Sync Result - Total Successful Deletes", string.Empty, null, new ExPerformanceCounter[]
		{
			TransportSyncWorkerFramework.SuccessfulDeletesPerSec
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			TransportSyncWorkerFramework.AverageSyncTime,
			TransportSyncWorkerFramework.AverageSyncTimeBase,
			TransportSyncWorkerFramework.LastSyncTime,
			TransportSyncWorkerFramework.OutstandingSyncs,
			TransportSyncWorkerFramework.CanceledSyncs,
			TransportSyncWorkerFramework.FailedSyncs,
			TransportSyncWorkerFramework.SuccessfulSyncs,
			TransportSyncWorkerFramework.TemporaryFailedSyncs,
			TransportSyncWorkerFramework.RecoverySyncsNeeded,
			TransportSyncWorkerFramework.AverageDeleteTime,
			TransportSyncWorkerFramework.AverageDeleteTimeBase,
			TransportSyncWorkerFramework.LastDeleteTime,
			TransportSyncWorkerFramework.OutstandingDeletes,
			TransportSyncWorkerFramework.FailedDeletes,
			TransportSyncWorkerFramework.SuccessfulDeletes
		};
	}
}
