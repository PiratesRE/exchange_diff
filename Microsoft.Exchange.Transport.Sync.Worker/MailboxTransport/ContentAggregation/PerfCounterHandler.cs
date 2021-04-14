using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Worker;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerfCounterHandler
	{
		private PerfCounterHandler()
		{
			this.totalDownloadedBytes = TransportSyncWorkerCore.TotalDownloadedBytes;
			this.totalUploadedBytes = TransportSyncWorkerCore.TotalUploadedBytes;
			this.totalMessagesSubmittedToPipeline = TransportSyncWorkerCore.MessagesSubmittedToPipeline;
			this.totalOutstandingJobs = TransportSyncWorkerCore.OutstandingJobs;
			this.totalOutstandingJobsInRetry = TransportSyncWorkerCore.OutstandingJobsInRetry;
			this.totalSubscriptionsAggregated = TransportSyncWorkerCore.TotalSubscriptionsAggregated;
			this.ResetCounters();
		}

		public static PerfCounterHandler Instance
		{
			get
			{
				if (PerfCounterHandler.instance == null)
				{
					lock (PerfCounterHandler.instanceInitializationLock)
					{
						if (PerfCounterHandler.instance == null)
						{
							PerfCounterHandler.instance = new PerfCounterHandler();
						}
					}
				}
				return PerfCounterHandler.instance;
			}
		}

		public void OnAggregationMailSubmission(object sender, EventArgs e)
		{
			this.totalMessagesSubmittedToPipeline.Increment();
		}

		public void OnDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.totalDownloadedBytes.IncrementBy(e.BytesDownloaded);
			this.totalUploadedBytes.IncrementBy(e.BytesUploaded);
		}

		public void OnWorkItemSubmitted(object sender, EventArgs e)
		{
			this.totalOutstandingJobs.Increment();
		}

		public void OnWorkItemDropped(object sender, EventArgs e)
		{
			this.totalOutstandingJobs.Decrement();
		}

		public void OnWorkItemAggregated(object sender, EventArgs e)
		{
			this.totalOutstandingJobs.Decrement();
			this.totalSubscriptionsAggregated.Increment();
		}

		public void OnRetryQueueLengthChanged(object sender, RetryableWorkQueueEventArgs e)
		{
			this.totalOutstandingJobsInRetry.IncrementBy((long)e.Difference);
		}

		private void ResetCounters()
		{
			this.totalDownloadedBytes.RawValue = 0L;
			this.totalUploadedBytes.RawValue = 0L;
			this.totalMessagesSubmittedToPipeline.RawValue = 0L;
			this.totalOutstandingJobs.RawValue = 0L;
			this.totalOutstandingJobsInRetry.RawValue = 0L;
			this.totalSubscriptionsAggregated.RawValue = 0L;
		}

		private static object instanceInitializationLock = new object();

		private static PerfCounterHandler instance;

		private ExPerformanceCounter totalDownloadedBytes;

		private ExPerformanceCounter totalUploadedBytes;

		private ExPerformanceCounter totalMessagesSubmittedToPipeline;

		private ExPerformanceCounter totalOutstandingJobs;

		private ExPerformanceCounter totalOutstandingJobsInRetry;

		private ExPerformanceCounter totalSubscriptionsAggregated;
	}
}
