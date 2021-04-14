using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ManagerPerfCounterHandler
	{
		private ManagerPerfCounterHandler()
		{
			this.expiryGranularity = TimeSpan.FromSeconds(ContentAggregationConfig.PCExpiryInterval.TotalSeconds / (double)ContentAggregationConfig.SLAExpiryBuckets);
			this.maxProcessingTimeInSeconds = (long)TimeSpan.FromHours(1.0).TotalSeconds;
			this.typeToString = new Dictionary<AggregationSubscriptionType, string>(4);
			this.processingTimePercentileCounters = new Dictionary<AggregationSubscriptionType, PercentileCounter>(4);
			this.subscriptionsCompletingSyncCounters = new Dictionary<AggregationSubscriptionType, SlidingPercentageCounter>(4);
			this.slaSecondsToString = new Dictionary<long, string>(3);
			this.slaPercentileCounters = new Dictionary<long, PercentileCounter>(3);
			this.totalInstanceForProtocolCounters = MsExchangeTransportSyncManagerByProtocolPerf.GetInstance("All");
			MsExchangeTransportSyncManagerByProtocolPerf.ResetInstance("All");
			this.typeToString.Add(AggregationSubscriptionType.All, "All");
			SlidingPercentageCounter value = new SlidingPercentageCounter(ContentAggregationConfig.PCExpiryInterval, this.expiryGranularity);
			this.subscriptionsCompletingSyncCounters.Add(AggregationSubscriptionType.All, value);
			this.InitializeMultiInstanceCounter(AggregationSubscriptionType.DeltaSyncMail);
			this.InitializeMultiInstanceCounter(AggregationSubscriptionType.IMAP);
			this.InitializeMultiInstanceCounter(AggregationSubscriptionType.Facebook);
			this.InitializeMultiInstanceCounter(AggregationSubscriptionType.Pop);
			this.InitializeMultiInstanceCounter(AggregationSubscriptionType.LinkedIn);
			this.GetSlaPercentileCounter(ContentAggregationConfig.AggregationInitialSyncInterval);
			this.GetSlaPercentileCounter(ContentAggregationConfig.AggregationIncrementalSyncInterval);
			this.GetSlaPercentileCounter(ContentAggregationConfig.MigrationInitialSyncInterval);
			this.GetSlaPercentileCounter(ContentAggregationConfig.MigrationIncrementalSyncInterval);
			this.databaseGuidToString = new Dictionary<Guid, string>(30);
			this.syncIntervalToString = new Dictionary<TimeSpan, string>(5);
			this.totalInstanceForSubscriptionCounters = MsExchangeTransportSyncManagerByDatabasePerf.GetInstance("All");
			MsExchangeTransportSyncManagerByDatabasePerf.ResetInstance("All");
			this.ResetSingleInstanceCounters();
		}

		public static ManagerPerfCounterHandler Instance
		{
			get
			{
				if (ManagerPerfCounterHandler.instance == null)
				{
					lock (ManagerPerfCounterHandler.InitializationLock)
					{
						if (ManagerPerfCounterHandler.instance == null)
						{
							ManagerPerfCounterHandler.instance = new ManagerPerfCounterHandler();
						}
					}
				}
				return ManagerPerfCounterHandler.instance;
			}
		}

		public void StartUpdatingCounters()
		{
			if (this.timer == null)
			{
				this.timer = new GuardedTimer(new TimerCallback(this.UpdateCounters), null, ContentAggregationConfig.SLAPerfCounterUpdateInterval);
			}
		}

		public void StopUpdatingCounters()
		{
			if (this.timer != null)
			{
				this.timer.Dispose(true);
				this.timer = null;
			}
		}

		internal void SetWaitToGetSubscriptionsCacheToken(long valueInMilliSeconds)
		{
			MsExchangeTransportSyncManagerPerf.LastWaitToGetSubscriptionsCacheToken.RawValue = valueInMilliSeconds;
		}

		internal void IncrementMailboxesInSubscriptionCachesBy(long countOfNewMailboxes)
		{
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesInSubscriptionCaches.IncrementBy(countOfNewMailboxes);
		}

		internal void IncrementCacheMessagesRebuilt()
		{
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesRebuiltInSubscriptionCaches.Increment();
		}

		internal void IncrementCacheMessagesRebuildRepaired()
		{
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesRepairRebuiltInSubscriptionCaches.Increment();
		}

		internal void IncrementMailboxesToBeRebuilt()
		{
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesToBeRebuiltInSubscriptionCaches.Increment();
		}

		internal void DecrementMailboxesToBeRebuilt()
		{
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesToBeRebuiltInSubscriptionCaches.Decrement();
		}

		internal void OnReportSyncQueueDispatchLagTimeEvent(object sender, SyncQueueEventArgs e)
		{
			MsExchangeTransportSyncManagerPerf.SubscriptionQueueDispatchLag.RawValue = (long)e.DispatchLagTime.TotalSeconds;
		}

		internal void IncrementSubscriptionsQueued(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.SubscriptionsQueued.Increment();
			multiInstanceCounter.SubscriptionsQueued.Increment();
		}

		internal void DecrementSubscriptionsQueued(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.SubscriptionsQueued.Decrement();
			multiInstanceCounter.SubscriptionsQueued.Decrement();
		}

		internal void IncrementSyncNowSubscriptionsQueued(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.SyncNowSubscriptionsQueued.Increment();
			multiInstanceCounter.SyncNowSubscriptionsQueued.Increment();
			this.IncrementSubscriptionsQueued(type);
		}

		internal void DecrementSyncNowSubscriptionsQueued(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.SyncNowSubscriptionsQueued.Decrement();
			multiInstanceCounter.SyncNowSubscriptionsQueued.Decrement();
			this.DecrementSubscriptionsQueued(type);
		}

		internal void IncrementSuccessfulSubmissions(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.SuccessfulSubmissions.Increment();
			multiInstanceCounter.SuccessfulSubmissions.Increment();
			this.IncrementSubscriptionsDispatched(multiInstanceCounter);
			this.subscriptionsCompletingSyncCounters[type].AddDenominator(1L);
			this.subscriptionsCompletingSyncCounters[AggregationSubscriptionType.All].AddDenominator(1L);
		}

		internal void IncrementDuplicateSubmissions(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.DuplicateSubmissions.Increment();
			multiInstanceCounter.DuplicateSubmissions.Increment();
			this.IncrementSubscriptionsDispatched(multiInstanceCounter);
		}

		internal void IncrementTemporarySubmissionFailures(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.TemporarySubmissionFailures.Increment();
			multiInstanceCounter.TemporarySubmissionFailures.Increment();
			this.IncrementSubscriptionsDispatched(multiInstanceCounter);
		}

		internal void IncrementAverageDispatchTimeBy(AggregationSubscriptionType type, long valueInMilliSeconds)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.AverageDispatchTime.IncrementBy(valueInMilliSeconds);
			multiInstanceCounter.AverageDispatchTime.IncrementBy(valueInMilliSeconds);
		}

		internal void IncrementAverageDispatchTimeBase(AggregationSubscriptionType type)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			this.totalInstanceForProtocolCounters.AverageDispatchTimeBase.Increment();
			multiInstanceCounter.AverageDispatchTimeBase.Increment();
		}

		internal void SetTimeToCompleteLastDispatch(AggregationSubscriptionType type, long valueInMilliSeconds)
		{
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			multiInstanceCounter.LastDispatchTime.RawValue = valueInMilliSeconds;
		}

		internal void SetLastSubscriptionProcessingTime(AggregationSubscriptionType type, long valueInMilliSeconds)
		{
			long value = Convert.ToInt64(valueInMilliSeconds / 1000L);
			MsExchangeTransportSyncManagerByProtocolPerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(type);
			multiInstanceCounter.LastSubscriptionProcessingTime.RawValue = valueInMilliSeconds;
			this.processingTimePercentileCounters[type].AddValue(value);
			this.subscriptionsCompletingSyncCounters[type].AddNumerator(1L);
			this.subscriptionsCompletingSyncCounters[AggregationSubscriptionType.All].AddNumerator(1L);
		}

		internal void AddSubscriptionSyncInterval(TimeSpan expectedSyncInterval, TimeSpan actualSyncInterval)
		{
			PercentileCounter slaPercentileCounter = this.GetSlaPercentileCounter(expectedSyncInterval);
			slaPercentileCounter.AddValue(Convert.ToInt64(actualSyncInterval.TotalSeconds));
		}

		internal void OnSubscriptionAddedOrRemovedEvent(object sender, SyncQueueEventArgs e)
		{
			MsExchangeTransportSyncManagerByDatabasePerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(e.DatabaseGuid);
			multiInstanceCounter.TotalSubscriptionsQueuedInDatabaseQueueManager.IncrementBy((long)e.NumberOfItemsChanged);
			this.totalInstanceForSubscriptionCounters.TotalSubscriptionsQueuedInDatabaseQueueManager.IncrementBy((long)e.NumberOfItemsChanged);
		}

		internal void OnSubscriptionSyncEnqueuedOrDequeuedEvent(object sender, SyncQueueEventArgs e)
		{
			MsExchangeTransportSyncManagerByDatabasePerfInstance multiInstanceCounter = this.GetMultiInstanceCounter(e.DatabaseGuid);
			multiInstanceCounter.TotalSubscriptionInstancesQueuedInDatabaseQueueManager.IncrementBy((long)e.NumberOfItemsChanged);
			MsExchangeTransportSyncManagerByDatabasePerfInstance multiInstanceCounter2 = this.GetMultiInstanceCounter(e.SyncInterval);
			multiInstanceCounter2.TotalSubscriptionInstancesQueuedInDatabaseQueueManager.IncrementBy((long)e.NumberOfItemsChanged);
			this.totalInstanceForSubscriptionCounters.TotalSubscriptionInstancesQueuedInDatabaseQueueManager.IncrementBy((long)e.NumberOfItemsChanged);
		}

		private MsExchangeTransportSyncManagerByDatabasePerfInstance GetMultiInstanceCounter(Guid databaseGuid)
		{
			MsExchangeTransportSyncManagerByDatabasePerfInstance result = null;
			string text = null;
			lock (this.databaseGuidToString)
			{
				if (!this.databaseGuidToString.TryGetValue(databaseGuid, out text))
				{
					text = databaseGuid.ToString();
					this.databaseGuidToString.Add(databaseGuid, text);
					result = MsExchangeTransportSyncManagerByDatabasePerf.GetInstance(text);
					MsExchangeTransportSyncManagerByDatabasePerf.ResetInstance(text);
				}
				else
				{
					result = MsExchangeTransportSyncManagerByDatabasePerf.GetInstance(text);
				}
			}
			return result;
		}

		private MsExchangeTransportSyncManagerByDatabasePerfInstance GetMultiInstanceCounter(TimeSpan syncInterval)
		{
			MsExchangeTransportSyncManagerByDatabasePerfInstance result = null;
			string text = null;
			lock (this.syncIntervalToString)
			{
				if (!this.syncIntervalToString.TryGetValue(syncInterval, out text))
				{
					text = string.Format(CultureInfo.InvariantCulture, syncInterval.ToString(), new object[0]);
					this.syncIntervalToString.Add(syncInterval, text);
					result = MsExchangeTransportSyncManagerByDatabasePerf.GetInstance(text);
					MsExchangeTransportSyncManagerByDatabasePerf.ResetInstance(text);
				}
				else
				{
					result = MsExchangeTransportSyncManagerByDatabasePerf.GetInstance(text);
				}
			}
			return result;
		}

		private void ResetSingleInstanceCounters()
		{
			MsExchangeTransportSyncManagerPerf.LastWaitToGetSubscriptionsCacheToken.RawValue = 0L;
			MsExchangeTransportSyncManagerPerf.SubscriptionQueueDispatchLag.RawValue = 0L;
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesInSubscriptionCaches.RawValue = 0L;
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesRebuiltInSubscriptionCaches.RawValue = 0L;
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesRepairRebuiltInSubscriptionCaches.RawValue = 0L;
			MsExchangeTransportSyncManagerPerf.TotalNumberOfMailboxesToBeRebuiltInSubscriptionCaches.RawValue = 0L;
		}

		private void UpdateCounters(object state)
		{
			lock (this.slaSyncObject)
			{
				foreach (KeyValuePair<long, PercentileCounter> keyValuePair in this.slaPercentileCounters)
				{
					MsExchangeTransportSyncManagerBySLAPerfInstance msExchangeTransportSyncManagerBySLAPerfInstance = MsExchangeTransportSyncManagerBySLAPerf.GetInstance(this.slaSecondsToString[keyValuePair.Key]);
					long rawValue = keyValuePair.Value.PercentileQuery(95.0);
					msExchangeTransportSyncManagerBySLAPerfInstance.SubscriptionsPollingFrequency95Percent.RawValue = rawValue;
				}
			}
			foreach (KeyValuePair<AggregationSubscriptionType, PercentileCounter> keyValuePair2 in this.processingTimePercentileCounters)
			{
				MsExchangeTransportSyncManagerByProtocolPerfInstance msExchangeTransportSyncManagerByProtocolPerfInstance = MsExchangeTransportSyncManagerByProtocolPerf.GetInstance(this.typeToString[keyValuePair2.Key]);
				long rawValue2 = keyValuePair2.Value.PercentileQuery(95.0);
				msExchangeTransportSyncManagerByProtocolPerfInstance.ProcessingTimeToSyncSubscription95Percent.RawValue = rawValue2;
			}
			foreach (KeyValuePair<AggregationSubscriptionType, SlidingPercentageCounter> keyValuePair3 in this.subscriptionsCompletingSyncCounters)
			{
				MsExchangeTransportSyncManagerByProtocolPerfInstance msExchangeTransportSyncManagerByProtocolPerfInstance2 = MsExchangeTransportSyncManagerByProtocolPerf.GetInstance(this.typeToString[keyValuePair3.Key]);
				double slidingPercentage = keyValuePair3.Value.GetSlidingPercentage();
				long rawValue3;
				if (slidingPercentage == 1.7976931348623157E+308)
				{
					rawValue3 = long.MaxValue;
				}
				else if (slidingPercentage == -1.7976931348623157E+308)
				{
					rawValue3 = long.MinValue;
				}
				else
				{
					rawValue3 = Convert.ToInt64(slidingPercentage);
				}
				msExchangeTransportSyncManagerByProtocolPerfInstance2.SubscriptionsCompletingSync.RawValue = rawValue3;
			}
		}

		private MsExchangeTransportSyncManagerByProtocolPerfInstance GetMultiInstanceCounter(AggregationSubscriptionType type)
		{
			if (!this.typeToString.ContainsKey(type))
			{
				throw new InvalidOperationException("Unexpected aggregation type encountered for which a counter hasnt been established yet: " + type);
			}
			return MsExchangeTransportSyncManagerByProtocolPerf.GetInstance(this.typeToString[type]);
		}

		private void InitializeMultiInstanceCounter(AggregationSubscriptionType type)
		{
			string text = type.ToString();
			this.typeToString.Add(type, text);
			MsExchangeTransportSyncManagerByProtocolPerf.GetInstance(text);
			MsExchangeTransportSyncManagerByProtocolPerf.ResetInstance(text);
			PercentileCounter value = new PercentileCounter(ContentAggregationConfig.PCExpiryInterval, this.expiryGranularity, 1L, this.maxProcessingTimeInSeconds);
			SlidingPercentageCounter value2 = new SlidingPercentageCounter(ContentAggregationConfig.PCExpiryInterval, this.expiryGranularity);
			this.processingTimePercentileCounters.Add(type, value);
			this.subscriptionsCompletingSyncCounters.Add(type, value2);
		}

		private PercentileCounter GetSlaPercentileCounter(TimeSpan syncInterval)
		{
			long num = Convert.ToInt64(syncInterval.TotalSeconds);
			PercentileCounter percentileCounter;
			lock (this.slaSyncObject)
			{
				if (!this.slaPercentileCounters.TryGetValue(num, out percentileCounter))
				{
					string text = string.Format(CultureInfo.InvariantCulture, syncInterval.ToString(), new object[0]);
					this.slaSecondsToString.Add(num, text);
					MsExchangeTransportSyncManagerBySLAPerf.GetInstance(text);
					MsExchangeTransportSyncManagerBySLAPerf.ResetInstance(text);
					long num2 = num + 7200L;
					long valueGranularity = Convert.ToInt64(num2 / (long)ContentAggregationConfig.SLADataBuckets);
					percentileCounter = new PercentileCounter(ContentAggregationConfig.PCExpiryInterval, this.expiryGranularity, valueGranularity, num2);
					this.slaPercentileCounters.Add(num, percentileCounter);
				}
			}
			return percentileCounter;
		}

		private void IncrementSubscriptionsDispatched(MsExchangeTransportSyncManagerByProtocolPerfInstance perfInstance)
		{
			this.totalInstanceForProtocolCounters.SubscriptionsDispatched.Increment();
			perfInstance.SubscriptionsDispatched.Increment();
		}

		private const string TotalInstanceName = "All";

		private const int SubscriptionTypeCounters = 4;

		private const int SlaCounters = 3;

		private const int DefaultNumberOfDatabasesPerMailboxServer = 30;

		private const int PercentileCounterPercentage = 95;

		private const int ProcessingTimeDataGranularity = 1;

		private const int NumberOfSyncIntervals = 5;

		private const int TwoHoursInSecs = 7200;

		private static readonly object InitializationLock = new object();

		private readonly object slaSyncObject = new object();

		private readonly TimeSpan expiryGranularity;

		private readonly long maxProcessingTimeInSeconds;

		private static ManagerPerfCounterHandler instance;

		private MsExchangeTransportSyncManagerByDatabasePerfInstance totalInstanceForSubscriptionCounters;

		private Dictionary<Guid, string> databaseGuidToString;

		private Dictionary<TimeSpan, string> syncIntervalToString;

		private MsExchangeTransportSyncManagerByProtocolPerfInstance totalInstanceForProtocolCounters;

		private Dictionary<AggregationSubscriptionType, string> typeToString;

		private Dictionary<AggregationSubscriptionType, PercentileCounter> processingTimePercentileCounters;

		private Dictionary<AggregationSubscriptionType, SlidingPercentageCounter> subscriptionsCompletingSyncCounters;

		private Dictionary<long, string> slaSecondsToString;

		private Dictionary<long, PercentileCounter> slaPercentileCounters;

		private GuardedTimer timer;
	}
}
