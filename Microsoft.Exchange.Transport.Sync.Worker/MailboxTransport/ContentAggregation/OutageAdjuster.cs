using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OutageAdjuster
	{
		public void Execute(ISyncWorkerData subscription, DateTime? previousSyncTime, TimeSpan outageDetectionThreshold, SyncLogSession syncLogSession, Trace tracer, string machineName, Guid databaseGuid)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("tracer", tracer);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("machineName", machineName);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			if (this.WasSubscriptionReactivated(subscription, previousSyncTime))
			{
				subscription.AdjustedLastSuccessfulSyncTime = subscription.LastSyncTime.Value;
				return;
			}
			TimeSpan timeSpan;
			if (!this.IsSyncAfterOutage(subscription, previousSyncTime, outageDetectionThreshold, out timeSpan))
			{
				return;
			}
			subscription.AppendOutageDetectionDiagnostics(machineName, databaseGuid, outageDetectionThreshold, timeSpan);
			syncLogSession.LogDebugging((TSLID)1306UL, tracer, "Outage Detected. Database: {0}, Threshold: {1}, Observed Duration: {2}", new object[]
			{
				databaseGuid,
				outageDetectionThreshold,
				timeSpan
			});
			subscription.AdjustedLastSuccessfulSyncTime += timeSpan;
		}

		private bool WasSubscriptionReactivated(ISyncWorkerData subscription, DateTime? previousSyncTime)
		{
			if (previousSyncTime == null)
			{
				return subscription.AdjustedLastSuccessfulSyncTime > subscription.CreationTime;
			}
			return subscription.AdjustedLastSuccessfulSyncTime > previousSyncTime.Value;
		}

		private bool IsSyncAfterOutage(ISyncWorkerData subscription, DateTime? previousSyncTime, TimeSpan outageDetectionThreshold, out TimeSpan timeSinceLastSync)
		{
			if (previousSyncTime != null)
			{
				timeSinceLastSync = subscription.LastSyncTime.Value - previousSyncTime.Value;
			}
			else
			{
				timeSinceLastSync = subscription.LastSyncTime.Value - subscription.CreationTime;
			}
			return timeSinceLastSync >= outageDetectionThreshold;
		}
	}
}
