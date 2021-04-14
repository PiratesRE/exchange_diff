using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class StoreDriverDeliveryPerfCounters
	{
		private StoreDriverDeliveryPerfCounters()
		{
		}

		public static StoreDriverDeliveryPerfCounters Instance
		{
			get
			{
				return StoreDriverDeliveryPerfCounters.DefaultInstance;
			}
		}

		public static void RefreshPerformanceCounters()
		{
			StoreDriverDeliveryPerfCounters.Instance.IncrementDeliveryAttempt(true);
			StoreDriverDeliveryPerfCounters.Instance.IncrementDeliveryFailure(false, true);
			StoreDriverDeliveryPerfCounters.Instance.AddDeliveryLatencySample(TimeSpan.Zero, true);
		}

		public void IncrementDeliveryAttempt(bool calculateOnly = false)
		{
			if (!calculateOnly)
			{
				this.deliveryAttemptsCounter.AddValue(1L);
			}
			lock (this.deliveryAttemptSyncObject)
			{
				MSExchangeStoreDriver.DeliveryAttempts.RawValue = this.deliveryAttemptsCounter.CalculateAverage();
				if (!calculateOnly)
				{
					this.recipientLevelPercentFailedDeliveries.AddDenominator(1L);
					this.recipientLevelPercentTemporaryFailedDeliveries.AddDenominator(1L);
				}
			}
		}

		public void IncrementDeliveryFailure(bool isPermanentFailure, bool calculateOnly = false)
		{
			if (!calculateOnly)
			{
				MSExchangeStoreDriver.FailedDeliveries.Increment();
				this.deliveryFailuresCounter.AddValue(1L);
			}
			lock (this.deliveryFailureSyncObject)
			{
				MSExchangeStoreDriver.DeliveryFailures.RawValue = this.deliveryFailuresCounter.CalculateAverage();
				if (!calculateOnly)
				{
					if (isPermanentFailure)
					{
						this.recipientLevelPercentFailedDeliveries.AddNumerator(1L);
					}
					else
					{
						this.recipientLevelPercentTemporaryFailedDeliveries.AddNumerator(1L);
					}
				}
				MSExchangeStoreDriver.RecipientLevelPercentFailedDeliveries.RawValue = (long)((int)this.recipientLevelPercentFailedDeliveries.GetSlidingPercentage());
				MSExchangeStoreDriver.RecipientLevelPercentTemporaryFailedDeliveries.RawValue = (long)((int)this.recipientLevelPercentTemporaryFailedDeliveries.GetSlidingPercentage());
			}
		}

		public void AddDeliveryLatencySample(TimeSpan latency, bool calculateOnly = false)
		{
			lock (this.deliveryLatencySyncObject)
			{
				if (!calculateOnly)
				{
					this.deliveryLatencyPerRecipient.AddValue((long)latency.Milliseconds);
				}
				MSExchangeStoreDriver.DeliveryLatencyPerRecipientMilliseconds.RawValue = this.deliveryLatencyPerRecipient.CalculateAverage();
			}
		}

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingBucketLength = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan SlidingSequenceWindowLength = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan SlidingSequenceBucketLength = TimeSpan.FromSeconds(2.0);

		private static readonly StoreDriverDeliveryPerfCounters DefaultInstance = new StoreDriverDeliveryPerfCounters();

		private readonly SlidingAverageCounter deliveryAttemptsCounter = new SlidingAverageCounter(StoreDriverDeliveryPerfCounters.SlidingWindowLength, StoreDriverDeliveryPerfCounters.SlidingBucketLength);

		private readonly SlidingAverageCounter deliveryFailuresCounter = new SlidingAverageCounter(StoreDriverDeliveryPerfCounters.SlidingWindowLength, StoreDriverDeliveryPerfCounters.SlidingBucketLength);

		private readonly SlidingPercentageCounter recipientLevelPercentFailedDeliveries = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), true);

		private readonly SlidingPercentageCounter recipientLevelPercentTemporaryFailedDeliveries = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), true);

		private readonly AverageSlidingSequence deliveryLatencyPerRecipient = new AverageSlidingSequence(StoreDriverDeliveryPerfCounters.SlidingSequenceWindowLength, StoreDriverDeliveryPerfCounters.SlidingSequenceBucketLength);

		private readonly object deliveryAttemptSyncObject = new object();

		private readonly object deliveryFailureSyncObject = new object();

		private readonly object deliveryLatencySyncObject = new object();
	}
}
