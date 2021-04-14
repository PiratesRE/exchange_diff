using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class TransportSyncWorkerCore
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (TransportSyncWorkerCore.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in TransportSyncWorkerCore.AllCounters)
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

		public const string CategoryName = "MSExchange Transport Sync Worker Core";

		private static readonly ExPerformanceCounter DownloadSpeed = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Download Speed (Bytes/sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutstandingJobs = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Sync Scheduler - Total On Server", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutstandingJobsInRetry = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Sync Scheduler - Total In Retry Queue", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfMessagesSubmittedToPipeline = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Messages Submitted to Pipeline - Rate (per second)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesSubmittedToPipeline = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Messages Submitted to Pipeline - Total Count", string.Empty, null, new ExPerformanceCounter[]
		{
			TransportSyncWorkerCore.RateOfMessagesSubmittedToPipeline
		});

		public static readonly ExPerformanceCounter TotalDownloadedBytes = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Total bytes downloaded", string.Empty, null, new ExPerformanceCounter[]
		{
			TransportSyncWorkerCore.DownloadSpeed
		});

		public static readonly ExPerformanceCounter TotalSubscriptionsAggregated = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Total subscriptions aggregated", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UploadSpeed = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Upload speed (bytes/sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUploadedBytes = new ExPerformanceCounter("MSExchange Transport Sync Worker Core", "Total bytes uploaded", string.Empty, null, new ExPerformanceCounter[]
		{
			TransportSyncWorkerCore.UploadSpeed
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			TransportSyncWorkerCore.MessagesSubmittedToPipeline,
			TransportSyncWorkerCore.OutstandingJobs,
			TransportSyncWorkerCore.OutstandingJobsInRetry,
			TransportSyncWorkerCore.TotalDownloadedBytes,
			TransportSyncWorkerCore.TotalSubscriptionsAggregated,
			TransportSyncWorkerCore.TotalUploadedBytes
		};
	}
}
