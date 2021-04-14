using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Framework;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FrameworkPerfCounterHandler
	{
		private FrameworkPerfCounterHandler()
		{
			this.pop3MessageBytesReceivedTotal = MSExchangePop3Aggregation.MessageBytesReceivedTotal;
			this.pop3MessagesReceivedTotal = MSExchangePop3Aggregation.MessagesReceivedTotal;
			this.deltaSyncTotalBytesDownloaded = MSExchangeDeltaSyncAggregation.TotalBytesDownloaded;
			this.deltaSyncTotalBytesUploaded = MSExchangeDeltaSyncAggregation.TotalBytesUploaded;
			this.deltaSyncTotalMessagesDownloaded = MSExchangeDeltaSyncAggregation.TotalMessagesDownloaded;
			this.deltaSyncTotalMessagesUploaded = MSExchangeDeltaSyncAggregation.TotalMessagesUploaded;
			this.imapSyncTotalBytesDownloaded = MSExchangeImapAggregation.TotalBytesDownloaded;
			this.imapSyncTotalBytesUploaded = MSExchangeImapAggregation.TotalBytesUploaded;
			this.imapSyncTotalMessagesDownloaded = MSExchangeImapAggregation.TotalMessagesDownloaded;
			this.imapSyncTotalMessagesUploaded = MSExchangeImapAggregation.TotalMessagesUploaded;
			this.successfulSyncs = TransportSyncWorkerFramework.SuccessfulSyncs;
			this.failedSyncs = TransportSyncWorkerFramework.FailedSyncs;
			this.canceledSyncs = TransportSyncWorkerFramework.CanceledSyncs;
			this.temporaryFailedSyncs = TransportSyncWorkerFramework.TemporaryFailedSyncs;
			this.outstandingSyncs = TransportSyncWorkerFramework.OutstandingSyncs;
			this.averageSyncTime = TransportSyncWorkerFramework.AverageSyncTime;
			this.averageSyncTimeBase = TransportSyncWorkerFramework.AverageSyncTimeBase;
			this.lastSyncTime = TransportSyncWorkerFramework.LastSyncTime;
			this.successfulDeletes = TransportSyncWorkerFramework.SuccessfulDeletes;
			this.failedDeletes = TransportSyncWorkerFramework.FailedDeletes;
			this.outstandingDeletes = TransportSyncWorkerFramework.OutstandingDeletes;
			this.averageDeleteTime = TransportSyncWorkerFramework.AverageDeleteTime;
			this.averageDeleteTimeBase = TransportSyncWorkerFramework.AverageDeleteTimeBase;
			this.lastDeleteTime = TransportSyncWorkerFramework.LastDeleteTime;
			this.totalInstanceForPeopleConnectionCounters = this.GetPeopleConnectionCounterInstance("All");
			this.facebookPeopleConnectionCounter = this.GetPeopleConnectionCounterInstance(AggregationSubscriptionType.Facebook.ToString());
			this.linkedInPeopleConnectionCounter = this.GetPeopleConnectionCounterInstance(AggregationSubscriptionType.LinkedIn.ToString());
			this.ResetCounters();
		}

		internal static FrameworkPerfCounterHandler Instance
		{
			get
			{
				if (FrameworkPerfCounterHandler.instance == null)
				{
					lock (FrameworkPerfCounterHandler.instanceInitializationLock)
					{
						if (FrameworkPerfCounterHandler.instance == null)
						{
							FrameworkPerfCounterHandler.instance = new FrameworkPerfCounterHandler();
						}
					}
				}
				return FrameworkPerfCounterHandler.instance;
			}
		}

		internal void OnDeltaSyncDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.deltaSyncTotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
			this.deltaSyncTotalBytesUploaded.IncrementBy(e.BytesUploaded);
			PerfCounterHandler.Instance.OnDownloadCompletion(sender, e);
		}

		internal void OnDeltaSyncMessageDownloadCompletion(object sender, EventArgs e)
		{
			this.deltaSyncTotalMessagesDownloaded.Increment();
		}

		internal void OnDeltaSyncMessageUploadCompletion(object sender, EventArgs e)
		{
			this.deltaSyncTotalMessagesUploaded.Increment();
		}

		internal void OnImapSyncDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.imapSyncTotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
			this.imapSyncTotalBytesUploaded.IncrementBy(e.BytesUploaded);
			PerfCounterHandler.Instance.OnDownloadCompletion(sender, e);
		}

		internal void OnImapSyncMessageDownloadCompletion(object sender, EventArgs e)
		{
			this.imapSyncTotalMessagesDownloaded.Increment();
		}

		internal void OnImapSyncMessageUploadCompletion(object sender, EventArgs e)
		{
			this.imapSyncTotalMessagesUploaded.Increment();
		}

		internal void OnPop3RetrieveMessageCompletion(object sender, DownloadCompleteEventArgs e)
		{
			PerfCounterHandler.Instance.OnDownloadCompletion(sender, e);
			this.pop3MessagesReceivedTotal.Increment();
			this.pop3MessageBytesReceivedTotal.IncrementBy(e.BytesDownloaded);
		}

		internal void OnSyncStarted()
		{
			this.outstandingSyncs.Increment();
		}

		internal void OnSyncCompletion(AsyncOperationResult<SyncEngineResultData> result)
		{
			this.outstandingSyncs.Decrement();
			if (result.Exception == null)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - result.Data.StartSyncTime;
				this.averageSyncTime.IncrementBy(timeSpan.Ticks);
				this.averageSyncTimeBase.Increment();
				this.lastSyncTime.RawValue = timeSpan.Ticks / 10000L;
				this.successfulSyncs.Increment();
				return;
			}
			if (result.Exception is OperationCanceledException)
			{
				this.canceledSyncs.Increment();
				return;
			}
			if (result.Exception is SyncPermanentException)
			{
				this.failedSyncs.Increment();
				return;
			}
			if (result.Exception is SyncTransientException)
			{
				this.temporaryFailedSyncs.Increment();
			}
		}

		internal void OnFacebookSyncDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.facebookPeopleConnectionCounter.TotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
			this.totalInstanceForPeopleConnectionCounters.TotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
		}

		internal void OnFacebookContactDownload(object sender, EventArgs e)
		{
			this.facebookPeopleConnectionCounter.TotalContactsDownloaded.Increment();
			this.totalInstanceForPeopleConnectionCounters.TotalContactsDownloaded.Increment();
		}

		internal void OnFacebookRequest(object sender, EventArgs e)
		{
			this.facebookPeopleConnectionCounter.TotalRequests.Increment();
			this.totalInstanceForPeopleConnectionCounters.TotalRequests.Increment();
		}

		internal void OnFacebookRequestWithNoChanges(object sender, EventArgs e)
		{
			this.facebookPeopleConnectionCounter.TotalRequestsWithNoChanges.Increment();
			this.totalInstanceForPeopleConnectionCounters.TotalRequestsWithNoChanges.Increment();
		}

		internal void OnLinkedInSyncDownloadCompletion(object sender, DownloadCompleteEventArgs e)
		{
			this.linkedInPeopleConnectionCounter.TotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
			this.totalInstanceForPeopleConnectionCounters.TotalBytesDownloaded.IncrementBy(e.BytesDownloaded);
		}

		internal void OnLinkedInContactDownload()
		{
			this.linkedInPeopleConnectionCounter.TotalContactsDownloaded.Increment();
			this.totalInstanceForPeopleConnectionCounters.TotalContactsDownloaded.Increment();
		}

		internal void OnLinkedInRequest()
		{
			this.linkedInPeopleConnectionCounter.TotalRequests.Increment();
			this.totalInstanceForPeopleConnectionCounters.TotalRequests.Increment();
		}

		internal void OnDeleteStarted()
		{
			this.outstandingDeletes.Increment();
		}

		internal void OnDeleteCompletion(AsyncOperationResult<SyncEngineResultData> result)
		{
			this.outstandingDeletes.Decrement();
			if (result.Exception == null)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - result.Data.StartSyncTime;
				this.averageDeleteTime.IncrementBy(timeSpan.Ticks);
				this.averageDeleteTimeBase.Increment();
				this.lastDeleteTime.RawValue = timeSpan.Ticks / 10000L;
				this.successfulDeletes.Increment();
				return;
			}
			if (result.Exception is FailedDeletePeopleConnectSubscriptionException)
			{
				this.failedDeletes.Increment();
			}
		}

		private void ResetCounters()
		{
			this.pop3MessageBytesReceivedTotal.RawValue = 0L;
			this.pop3MessagesReceivedTotal.RawValue = 0L;
			this.deltaSyncTotalBytesDownloaded.RawValue = 0L;
			this.deltaSyncTotalBytesUploaded.RawValue = 0L;
			this.deltaSyncTotalMessagesDownloaded.RawValue = 0L;
			this.deltaSyncTotalMessagesUploaded.RawValue = 0L;
			this.imapSyncTotalBytesDownloaded.RawValue = 0L;
			this.imapSyncTotalBytesUploaded.RawValue = 0L;
			this.imapSyncTotalMessagesDownloaded.RawValue = 0L;
			this.imapSyncTotalMessagesUploaded.RawValue = 0L;
			this.successfulSyncs.RawValue = 0L;
			this.failedSyncs.RawValue = 0L;
			this.canceledSyncs.RawValue = 0L;
			this.temporaryFailedSyncs.RawValue = 0L;
			this.outstandingSyncs.RawValue = 0L;
			this.averageSyncTime.RawValue = 0L;
			this.averageSyncTimeBase.RawValue = 0L;
			this.lastSyncTime.RawValue = 0L;
			this.successfulDeletes.RawValue = 0L;
			this.failedDeletes.RawValue = 0L;
			this.outstandingDeletes.RawValue = 0L;
			this.averageDeleteTime.RawValue = 0L;
			this.averageDeleteTimeBase.RawValue = 0L;
			this.lastDeleteTime.RawValue = 0L;
		}

		private MSExchangePeopleConnectionInstance GetPeopleConnectionCounterInstance(string instanceName)
		{
			MSExchangePeopleConnection.ResetInstance(instanceName);
			return MSExchangePeopleConnection.GetInstance(instanceName);
		}

		private const string TotalInstanceName = "All";

		private static object instanceInitializationLock = new object();

		private static FrameworkPerfCounterHandler instance;

		private ExPerformanceCounter pop3MessageBytesReceivedTotal;

		private ExPerformanceCounter pop3MessagesReceivedTotal;

		private ExPerformanceCounter deltaSyncTotalBytesDownloaded;

		private ExPerformanceCounter deltaSyncTotalBytesUploaded;

		private ExPerformanceCounter deltaSyncTotalMessagesDownloaded;

		private ExPerformanceCounter deltaSyncTotalMessagesUploaded;

		private ExPerformanceCounter imapSyncTotalBytesDownloaded;

		private ExPerformanceCounter imapSyncTotalBytesUploaded;

		private ExPerformanceCounter imapSyncTotalMessagesDownloaded;

		private ExPerformanceCounter imapSyncTotalMessagesUploaded;

		private MSExchangePeopleConnectionInstance totalInstanceForPeopleConnectionCounters;

		private MSExchangePeopleConnectionInstance facebookPeopleConnectionCounter;

		private MSExchangePeopleConnectionInstance linkedInPeopleConnectionCounter;

		private ExPerformanceCounter successfulSyncs;

		private ExPerformanceCounter failedSyncs;

		private ExPerformanceCounter canceledSyncs;

		private ExPerformanceCounter temporaryFailedSyncs;

		private ExPerformanceCounter outstandingSyncs;

		private ExPerformanceCounter averageSyncTime;

		private ExPerformanceCounter averageSyncTimeBase;

		private ExPerformanceCounter lastSyncTime;

		private ExPerformanceCounter successfulDeletes;

		private ExPerformanceCounter failedDeletes;

		private ExPerformanceCounter outstandingDeletes;

		private ExPerformanceCounter averageDeleteTime;

		private ExPerformanceCounter averageDeleteTimeBase;

		private ExPerformanceCounter lastDeleteTime;
	}
}
