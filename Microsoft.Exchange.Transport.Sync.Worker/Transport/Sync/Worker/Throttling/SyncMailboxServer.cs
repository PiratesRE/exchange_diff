using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncMailboxServer : SyncResource
	{
		protected SyncMailboxServer(Guid mailboxServerGuid, string mailboxServer, SyncLogSession syncLogSession) : base(syncLogSession, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
		{
			mailboxServerGuid,
			mailboxServer
		}))
		{
			base.Initialize();
		}

		protected override int MaxConcurrentWorkInUnknownState
		{
			get
			{
				return AggregationConfiguration.Instance.MaxItemsForMailboxServerInUnknownHealthState;
			}
		}

		protected override SubscriptionSubmissionResult ResourceHealthUnknownResult
		{
			get
			{
				return SubscriptionSubmissionResult.MailboxServerCpuUnknown;
			}
		}

		protected override SubscriptionSubmissionResult MaxConcurrentWorkAgainstResourceLimitReachedResult
		{
			get
			{
				return SubscriptionSubmissionResult.MaxConcurrentMailboxSubmissions;
			}
		}

		internal static SyncMailboxServer CreateSyncMailboxServer(Guid mailboxServerGuid, string mailboxServer, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfGuidEmpty("mailboxServerGuid", mailboxServerGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxServer", mailboxServer);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			return new SyncMailboxServer(mailboxServerGuid, mailboxServer, syncLogSession);
		}

		protected override SyncResourceMonitor[] InitializeHealthMonitoring()
		{
			ResourceKey local = ProcessorResourceKey.Local;
			return new SyncResourceMonitor[]
			{
				this.CreateSyncResourceMonitor(local, SyncResourceMonitorType.MailboxCPU)
			};
		}

		protected virtual SyncResourceMonitor CreateSyncResourceMonitor(ResourceKey resourceKey, SyncResourceMonitorType syncResourceMonitorType)
		{
			return new SyncResourceMonitor(base.SyncLogSession, resourceKey, syncResourceMonitorType);
		}

		protected override SubscriptionSubmissionResult GetResultForResourceUnhealthy(SyncResourceMonitorType syncResourceMonitorType)
		{
			return SubscriptionSubmissionResult.MailboxServerCpuUnhealthy;
		}

		protected override bool CanAcceptWorkBasedOnResourceSpecificChecks(out SubscriptionSubmissionResult result)
		{
			if (!base.CanAddOneMoreConcurrentRequestToResource())
			{
				result = this.MaxConcurrentWorkAgainstResourceLimitReachedResult;
				return false;
			}
			result = SubscriptionSubmissionResult.Success;
			return true;
		}
	}
}
