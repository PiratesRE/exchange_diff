using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionCompletionData
	{
		internal SubscriptionCompletionStatus SubscriptionCompletionStatus { get; set; }

		internal AggregationType AggregationType { get; set; }

		internal SyncPhase SyncPhase { get; set; }

		internal Guid DatabaseGuid { get; set; }

		internal Guid MailboxGuid { get; set; }

		internal Guid SubscriptionGuid { get; set; }

		internal StoreObjectId SubscriptionMessageID { get; set; }

		internal bool MoreDataToDownload { get; set; }

		internal SerializedSubscription SerializedSubscription { get; set; }

		internal string SyncWatermark { get; set; }

		internal ExDateTime? LastSuccessfulDispatchTime { get; set; }

		internal WorkType? DispatchedWorkType { get; set; }

		internal bool DisableSubscription
		{
			get
			{
				return SubscriptionCompletionStatus.DisableSubscription == this.SubscriptionCompletionStatus;
			}
		}

		internal bool InvalidState
		{
			get
			{
				return SubscriptionCompletionStatus.InvalidState == this.SubscriptionCompletionStatus;
			}
		}

		internal bool SubscriptionDeleted
		{
			get
			{
				return SubscriptionCompletionStatus.DeleteSubscription == this.SubscriptionCompletionStatus;
			}
		}

		internal bool SyncFailed
		{
			get
			{
				return SubscriptionCompletionStatus.SyncError == this.SubscriptionCompletionStatus;
			}
		}

		internal bool TryGetCurrentWorkType(SyncLogSession syncLogSession, out WorkType? workType)
		{
			workType = null;
			bool result;
			try
			{
				workType = new WorkType?(WorkTypeManager.ClassifyWorkTypeFromSubscriptionInformation(this.AggregationType, this.SyncPhase));
				result = true;
			}
			catch (NotSupportedException ex)
			{
				syncLogSession.LogError((TSLID)1289UL, "TryGetCurrentWorkType: Invalid Aggregation type ({0}) or Sync Phase ({1}): {2}", new object[]
				{
					this.AggregationType,
					this.SyncPhase,
					ex
				});
				result = false;
			}
			return result;
		}
	}
}
