using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionWorkItemCompletedEventResult : SubscriptionWorkItemEventResult
	{
		public DetailedAggregationStatus DetailedAggregationStatus
		{
			get
			{
				return this.detailedAggregationStatus;
			}
		}

		public bool DisableSubscription
		{
			get
			{
				return this.disableSubscription;
			}
		}

		public bool KeepSubscriptionEnabled
		{
			get
			{
				return this.keepSubscriptionEnabled;
			}
		}

		public bool SyncPhaseCompleted
		{
			get
			{
				return this.syncPhaseCompleted;
			}
		}

		public void SetDisableSubscription(DetailedAggregationStatus detailedAggregationStatus)
		{
			this.disableSubscription = true;
			this.detailedAggregationStatus = detailedAggregationStatus;
		}

		public void SetKeepEnabledSubscription()
		{
			this.keepSubscriptionEnabled = true;
		}

		public void SetSyncPhaseCompleted()
		{
			this.syncPhaseCompleted = true;
		}

		private bool disableSubscription;

		private bool keepSubscriptionEnabled;

		private DetailedAggregationStatus detailedAggregationStatus;

		private bool syncPhaseCompleted;
	}
}
