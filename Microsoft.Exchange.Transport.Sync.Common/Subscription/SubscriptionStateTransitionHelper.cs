using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionStateTransitionHelper
	{
		public SubscriptionStateTransitionHelper(PimAggregationSubscription subscription)
		{
			this.subscription = subscription;
		}

		public void DisableAsPoisonous()
		{
			if (this.subscription.Status == AggregationStatus.Poisonous)
			{
				throw new InvalidOperationException("method shouldn't be invoked for a poisonous subscription.");
			}
			if (this.subscription.Status == AggregationStatus.InvalidVersion)
			{
				throw new InvalidOperationException("method shouldn't be invoked for an invalid version subscription.");
			}
			this.subscription.Status = AggregationStatus.Poisonous;
			this.subscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
			if (string.IsNullOrEmpty(this.subscription.PoisonCallstack))
			{
				this.subscription.PoisonCallstack = this.ManuallyDisabledIndicator();
				return;
			}
			this.subscription.PoisonCallstack = this.ManuallyDisabledIndicator() + Environment.NewLine + this.subscription.PoisonCallstack;
		}

		public void Disable()
		{
			if (this.subscription.Status == AggregationStatus.Disabled)
			{
				return;
			}
			if (this.subscription.Status == AggregationStatus.InvalidVersion)
			{
				throw new InvalidOperationException("method shouldn't be invoked for an invalid version subscription.");
			}
			if (this.subscription.Status == AggregationStatus.Poisonous)
			{
				this.ResetPoisonProperties();
			}
			this.subscription.Status = AggregationStatus.Disabled;
			this.subscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
			if (string.IsNullOrEmpty(this.subscription.Diagnostics))
			{
				this.subscription.Diagnostics = this.ManuallyDisabledIndicator();
				return;
			}
			PimAggregationSubscription pimAggregationSubscription = this.subscription;
			pimAggregationSubscription.Diagnostics = pimAggregationSubscription.Diagnostics + Environment.NewLine + this.ManuallyDisabledIndicator();
		}

		public void Enable()
		{
			AggregationStatus status = this.subscription.Status;
			if (status == AggregationStatus.Poisonous)
			{
				throw new InvalidOperationException("method shouldn't be invoked for a poisonous subscription.");
			}
			if (status == AggregationStatus.InvalidVersion)
			{
				throw new InvalidOperationException("method shouldn't be invoked for an invalid version subscription.");
			}
			if (status == AggregationStatus.InProgress || status == AggregationStatus.Succeeded)
			{
				return;
			}
			this.SetEnabledStatus();
			if (status != AggregationStatus.Delayed)
			{
				this.ResetOutageDetectionProperties();
			}
		}

		public void EnableFromPoison()
		{
			if (this.subscription.Status != AggregationStatus.Poisonous)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "method shouldn't be invoked for non-poisonous subscriptions, actual status: {0}", new object[]
				{
					this.subscription.Status
				}));
			}
			this.SetEnabledStatus();
			this.ResetPoisonProperties();
			this.ResetOutageDetectionProperties();
		}

		protected virtual DateTime ResetAdjustedLastSuccessfulSyncTime()
		{
			return DateTime.UtcNow;
		}

		private void SetEnabledStatus()
		{
			this.subscription.Status = AggregationStatus.InProgress;
			this.subscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
		}

		private void ResetOutageDetectionProperties()
		{
			this.subscription.AdjustedLastSuccessfulSyncTime = this.ResetAdjustedLastSuccessfulSyncTime();
			this.subscription.OutageDetectionDiagnostics = string.Empty;
		}

		private void ResetPoisonProperties()
		{
			this.subscription.PoisonCallstack = null;
		}

		private string ManuallyDisabledIndicator()
		{
			return string.Format(CultureInfo.InvariantCulture, "Manually disabled on {0}", new object[]
			{
				DateTime.UtcNow
			});
		}

		private PimAggregationSubscription subscription;
	}
}
