using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncTransportServer : SyncResource
	{
		protected SyncTransportServer(SyncLogSession syncLogSession, int maxPendingMessages, int maxPendingMessagesPerUser) : base(syncLogSession, "TransportServer:" + Environment.MachineName)
		{
			this.maxPendingMessages = maxPendingMessages;
			this.maxPendingMessagesPerUser = maxPendingMessagesPerUser;
			base.Initialize();
		}

		private protected SyncTransportResourceMonitor SyncTransportResourceMonitor { protected get; private set; }

		private protected SyncTransportUserResourceMonitor SyncTransportUserResourceMonitor { protected get; private set; }

		protected override int MaxConcurrentWorkInUnknownState
		{
			get
			{
				return AggregationConfiguration.Instance.MaxItemsForTransportServerInUnknownHealthState;
			}
		}

		protected override SubscriptionSubmissionResult ResourceHealthUnknownResult
		{
			get
			{
				return SubscriptionSubmissionResult.TransportQueueHealthUnknown;
			}
		}

		protected override SubscriptionSubmissionResult MaxConcurrentWorkAgainstResourceLimitReachedResult
		{
			get
			{
				throw new InvalidOperationException("SuggestedConcurrency is not to be supported against SyncTransportServer.");
			}
		}

		internal static SyncTransportServer CreateSyncTransportServer(SyncLogSession syncLogSession, int maxPendingMessages, int maxPendingMessagesPerUser)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentLessThanEqualToZero("maxPendingMessages", maxPendingMessages);
			SyncUtilities.ThrowIfArgumentLessThanEqualToZero("maxPendingMessagesPerUser", maxPendingMessagesPerUser);
			return new SyncTransportServer(syncLogSession, maxPendingMessages, maxPendingMessagesPerUser);
		}

		internal override void RemoveWorkItem(AggregationWorkItem workItem)
		{
			base.RemoveWorkItem(workItem);
			int maxDownloadItemsPerConnection = AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(workItem.AggregationType);
			int num = 0;
			if (workItem.SyncHealthData != null)
			{
				num = workItem.SyncHealthData.TotalItemsSubmittedToTransport;
			}
			num = Math.Max(0, num);
			num = Math.Min(num, maxDownloadItemsPerConnection);
			int num2 = maxDownloadItemsPerConnection - num;
			if (num2 > 0)
			{
				base.SyncLogSession.LogDebugging((TSLID)344UL, "Reducing load by: {0} for subscription {1} in userMailbox {2}.", new object[]
				{
					num2,
					workItem.SubscriptionId,
					workItem.UserMailboxGuid
				});
				this.SyncTransportResourceMonitor.RemoveLoad(num2);
				this.SyncTransportUserResourceMonitor.RemoveLoad(workItem.UserMailboxGuid, num2);
			}
		}

		internal void TrackMailItem(TransportMailItem transportMailItem, Guid userMailboxGuid, Guid subscriptionGuid, string cloudId)
		{
			SyncUtilities.ThrowIfArgumentNull("transportMailItem", transportMailItem);
			SyncUtilities.ThrowIfGuidEmpty("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			base.SyncLogSession.LogDebugging((TSLID)347UL, "Submitted cloudId: {0} for subscription {1} in userMailbox {2}.", new object[]
			{
				cloudId,
				subscriptionGuid,
				userMailboxGuid
			});
			int invocationCount = 0;
			ExDateTime startTime = ExDateTime.UtcNow;
			transportMailItem.OnReleaseFromActive += delegate(TransportMailItem tmi)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - startTime;
				int num = Interlocked.Increment(ref invocationCount);
				this.SyncLogSession.LogDebugging((TSLID)348UL, "Received notification for cloudId: {0} for subscription {1} in userMailbox {2}. Time to deliver: {3}", new object[]
				{
					cloudId,
					subscriptionGuid,
					userMailboxGuid,
					timeSpan
				});
				bool flag = num == 1;
				if (flag)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveLoadByOneForMailSubmission), userMailboxGuid);
				}
			};
		}

		protected override SyncResourceMonitor[] InitializeHealthMonitoring()
		{
			this.SyncTransportResourceMonitor = this.CreateSyncTransportResourceMonitor(base.SyncLogSession, this.maxPendingMessages);
			this.SyncTransportUserResourceMonitor = this.CreateSyncTransportUserResourceMonitor(base.SyncLogSession, this.maxPendingMessagesPerUser);
			return new SyncResourceMonitor[]
			{
				this.SyncTransportResourceMonitor,
				this.SyncTransportUserResourceMonitor
			};
		}

		protected override SubscriptionSubmissionResult GetResultForResourceUnhealthy(SyncResourceMonitorType syncResourceMonitorType)
		{
			switch (syncResourceMonitorType)
			{
			case SyncResourceMonitorType.ServerTransportQueue:
				return SubscriptionSubmissionResult.ServerTransportQueueUnhealthy;
			case SyncResourceMonitorType.UserTransportQueue:
				return SubscriptionSubmissionResult.UserTransportQueueUnhealthy;
			default:
				throw new InvalidOperationException("Invalid syncResourceMonitorType found: " + syncResourceMonitorType);
			}
		}

		protected override bool CanAcceptWorkBasedOnResourceSpecificChecks(out SubscriptionSubmissionResult result)
		{
			result = SubscriptionSubmissionResult.Success;
			return true;
		}

		protected override void AddWorkItem(AggregationWorkItem workItem)
		{
			base.AddWorkItem(workItem);
			int maxDownloadItemsPerConnection = AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(workItem.AggregationType);
			this.SyncTransportResourceMonitor.AddLoad(maxDownloadItemsPerConnection);
			this.SyncTransportUserResourceMonitor.AddLoad(workItem.UserMailboxGuid, maxDownloadItemsPerConnection);
		}

		protected virtual SyncTransportResourceMonitor CreateSyncTransportResourceMonitor(SyncLogSession syncLogSession, int maxPendingMessages)
		{
			return new SyncTransportResourceMonitor(syncLogSession, maxPendingMessages);
		}

		protected virtual SyncTransportUserResourceMonitor CreateSyncTransportUserResourceMonitor(SyncLogSession syncLogSession, int maxPendingMessagesPerUser)
		{
			return new SyncTransportUserResourceMonitor(syncLogSession, maxPendingMessagesPerUser);
		}

		private void RemoveLoadByOneForMailSubmission(object state)
		{
			Guid userMailboxGuid = (Guid)state;
			this.SyncTransportResourceMonitor.RemoveLoad(1);
			this.SyncTransportUserResourceMonitor.RemoveLoad(userMailboxGuid, 1);
		}

		private readonly int maxPendingMessages;

		private readonly int maxPendingMessagesPerUser;
	}
}
