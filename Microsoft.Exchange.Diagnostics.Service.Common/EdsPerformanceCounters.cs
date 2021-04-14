using System;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	internal static class EdsPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (EdsPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in EdsPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Diagnostics Service";

		public static readonly ExPerformanceCounter BytesLeftToBeProcessedOnDisk = new ExPerformanceCounter("MSExchange Diagnostics Service", "Bytes left to be processed on disk", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfBatchesUploadedForTheLastHour = new ExPerformanceCounter("MSExchange Diagnostics Service", "Number of batches uploaded for the last hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfBatchesUploadedSinceServiceStarted = new ExPerformanceCounter("MSExchange Diagnostics Service", "Number of batches uploaded since service started", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulUploadsInLastHundredBatches = new ExPerformanceCounter("MSExchange Diagnostics Service", "Successful uploads in last hundred batches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedUploadsInLastHundredBatches = new ExPerformanceCounter("MSExchange Diagnostics Service", "Failed uploads in last hundred batches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RetryCountSinceLastSuccessfulUpload = new ExPerformanceCounter("MSExchange Diagnostics Service", "Retry count since last successful upload", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgConnectionOpenLatencyForLastHundredBatches = new ExPerformanceCounter("MSExchange Diagnostics Service", "Average connection open latency for last hundred batches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionOpenLatencyForLastBatch = new ExPerformanceCounter("MSExchange Diagnostics Service", "Connection open latency for last batch", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgUploadLatencyForLastHundredBatches = new ExPerformanceCounter("MSExchange Diagnostics Service", "Average upload latency for last hundred batches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UploadLatencyForLastBatch = new ExPerformanceCounter("MSExchange Diagnostics Service", "Upload latency for last batch", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AvgUploadSizeForLastHundredBatches = new ExPerformanceCounter("MSExchange Diagnostics Service", "Average upload size for last hundred batches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UploadSizeForLastBatch = new ExPerformanceCounter("MSExchange Diagnostics Service", "Upload size for last batch", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			EdsPerformanceCounters.BytesLeftToBeProcessedOnDisk,
			EdsPerformanceCounters.NumberOfBatchesUploadedForTheLastHour,
			EdsPerformanceCounters.NumberOfBatchesUploadedSinceServiceStarted,
			EdsPerformanceCounters.SuccessfulUploadsInLastHundredBatches,
			EdsPerformanceCounters.FailedUploadsInLastHundredBatches,
			EdsPerformanceCounters.RetryCountSinceLastSuccessfulUpload,
			EdsPerformanceCounters.AvgConnectionOpenLatencyForLastHundredBatches,
			EdsPerformanceCounters.ConnectionOpenLatencyForLastBatch,
			EdsPerformanceCounters.AvgUploadLatencyForLastHundredBatches,
			EdsPerformanceCounters.UploadLatencyForLastBatch,
			EdsPerformanceCounters.AvgUploadSizeForLastHundredBatches,
			EdsPerformanceCounters.UploadSizeForLastBatch
		};
	}
}
