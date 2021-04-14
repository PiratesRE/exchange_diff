using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyPerformanceCounters : IShadowRedundancyPerformanceCounters
	{
		public ShadowRedundancyPerformanceCounters()
		{
			this.performanceCounters = new ExPerformanceCounter[]
			{
				ShadowRedundancyPerformanceCounters.perfCounters.RedundantMessageDiscardEvents,
				ShadowRedundancyPerformanceCounters.perfCounters.RedundantMessageDiscardEventsExpired,
				ShadowRedundancyPerformanceCounters.queuingPerfCounters.AggregateShadowQueueLength,
				ShadowRedundancyPerformanceCounters.queuingPerfCounters.ShadowQueueAutoDiscardsTotal
			};
			this.refreshSlidingCountersTimer = new GuardedTimer(delegate(object state)
			{
				this.RefreshSlidingCounters();
			}, null, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0));
		}

		public long RedundantMessageDiscardEvents
		{
			get
			{
				return ShadowRedundancyPerformanceCounters.perfCounters.RedundantMessageDiscardEvents.RawValue;
			}
			set
			{
				ShadowRedundancyPerformanceCounters.perfCounters.RedundantMessageDiscardEvents.RawValue = value;
			}
		}

		public bool IsValid(ShadowRedundancyCounterId shadowRedundancyCounterName)
		{
			if (shadowRedundancyCounterName.Equals(ShadowRedundancyCounterId.RedundantMessageDiscardEvents) || shadowRedundancyCounterName.Equals(ShadowRedundancyCounterId.RedundantMessageDiscardEventsExpired))
			{
				return ShadowRedundancyPerformanceCounters.perfCounters != null;
			}
			if (shadowRedundancyCounterName.Equals(ShadowRedundancyCounterId.AggregateShadowQueueLength) || shadowRedundancyCounterName.Equals(ShadowRedundancyCounterId.ShadowQueueAutoDiscardsTotal))
			{
				return ShadowRedundancyPerformanceCounters.queuingPerfCounters != null;
			}
			throw new ArgumentOutOfRangeException("shadowRedundancyCounterName");
		}

		public void IncrementCounter(ShadowRedundancyCounterId shadowRedundancyCounterName)
		{
			if (this.IsValid(shadowRedundancyCounterName))
			{
				this.performanceCounters[(int)shadowRedundancyCounterName].Increment();
			}
		}

		public void IncrementCounterBy(ShadowRedundancyCounterId shadowRedundancyCounterName, long value)
		{
			if (this.IsValid(shadowRedundancyCounterName))
			{
				this.performanceCounters[(int)shadowRedundancyCounterName].IncrementBy(value);
			}
		}

		public void DecrementCounter(ShadowRedundancyCounterId shadowRedundancyCounterName)
		{
			if (this.IsValid(shadowRedundancyCounterName))
			{
				this.performanceCounters[(int)shadowRedundancyCounterName].Decrement();
			}
		}

		public void DecrementCounterBy(ShadowRedundancyCounterId shadowRedundancyCounterName, long value)
		{
			if (this.IsValid(shadowRedundancyCounterName))
			{
				this.performanceCounters[(int)shadowRedundancyCounterName].IncrementBy(-value);
			}
		}

		public void DelayedAckExpired(long messageCount)
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				ShadowRedundancyPerformanceCounters.perfCounters.CurrentMessagesAckBeforeRelayCompleted.IncrementBy(messageCount);
			}
		}

		public void DelayedAckDeliveredAfterExpiry(long messageCount)
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				ShadowRedundancyPerformanceCounters.perfCounters.CurrentMessagesAckBeforeRelayCompleted.IncrementBy(-messageCount);
			}
		}

		public void UpdateShadowQueueLength(string hostname, int changeAmount)
		{
			ShadowRedundancyPerformanceCounters.shadowQueueLengthCounters.Get(hostname).IncrementBy((long)changeAmount);
		}

		public ITimerCounter ShadowSelectionLatencyCounter()
		{
			return new AverageTimeCounter(ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostSelectionAverageTime, ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostSelectionAverageTimeBase);
		}

		public ITimerCounter ShadowNegotiationLatencyCounter()
		{
			return new AverageTimeCounter(ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostNegotiationAverageTime, ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostNegotiationAverageTimeBase);
		}

		public IAverageCounter ShadowSuccessfulNegotiationLatencyCounter()
		{
			return new AverageCounter(ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostSuccessfulNegotiationAverageTime, ShadowRedundancyPerformanceCounters.perfCounters.ShadowHostSuccessfulNegotiationAverageTimeBase);
		}

		public ITimerCounter ShadowHeartbeatLatencyCounter(string hostname)
		{
			return new AverageTimeCounter(ShadowRedundancyPerformanceCounters.shadowHeartbeatLatencyCounters.Get(hostname), ShadowRedundancyPerformanceCounters.shadowHeartbeatLatencyBaseCounters.Get(hostname));
		}

		public void ShadowFailure(string hostname)
		{
			ShadowRedundancyPerformanceCounters.shadowFailureCounters.Get(hostname).Increment();
		}

		public void HeartbeatFailure(string hostname)
		{
			ShadowRedundancyPerformanceCounters.heartbeatFailureCounters.Get(hostname).Increment();
		}

		public void TrackMessageMadeRedundant(bool success)
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				if (!success)
				{
					ShadowRedundancyPerformanceCounters.perfCounters.MessagesFailedToBeMadeRedundant.RawValue = ShadowRedundancyPerformanceCounters.failureToMakeMessageRedundantCounter.AddValue(1L);
				}
				ShadowRedundancyPerformanceCounters.perfCounters.MessagesFailedToBeMadeRedundantPercentage.RawValue = (long)ShadowRedundancyPerformanceCounters.messagesFailedToBeMadeRedundantPercentageCounter.Add(success ? 0L : 1L, 1L);
			}
		}

		public void SubmitMessagesFromShadowQueue(string hostname, int count)
		{
			ShadowRedundancyPerformanceCounters.resubmittedMessageCounters.Get(hostname).IncrementBy((long)count);
		}

		public void SmtpTimeout()
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				ShadowRedundancyPerformanceCounters.perfCounters.TotalSmtpTimeouts.RawValue = ShadowRedundancyPerformanceCounters.totalSmtpTimeoutsCounter.AddValue(1L);
			}
		}

		public void SmtpClientFailureAfterAccept()
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				ShadowRedundancyPerformanceCounters.perfCounters.ClientAckFailureCount.RawValue = ShadowRedundancyPerformanceCounters.clientFailureAfterAcceptCounter.AddValue(1L);
			}
		}

		public void MessageShadowed(string shadowServer, bool remote)
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				double value = ShadowRedundancyPerformanceCounters.localRemoteShadowPercentageCounter.Add(remote ? 0L : 1L, 1L);
				ShadowRedundancyPerformanceCounters.perfCounters.LocalSiteShadowPercentage.RawValue = Convert.ToInt64(value);
			}
			ShadowRedundancyPerformanceCounters.shadowedMessageCounters.Get(shadowServer).Increment();
		}

		private void RefreshSlidingCounters()
		{
			if (ShadowRedundancyPerformanceCounters.perfCounters != null)
			{
				ShadowRedundancyPerformanceCounters.perfCounters.TotalSmtpTimeouts.RawValue = ShadowRedundancyPerformanceCounters.totalSmtpTimeoutsCounter.AddValue(0L);
				ShadowRedundancyPerformanceCounters.perfCounters.ClientAckFailureCount.RawValue = ShadowRedundancyPerformanceCounters.clientFailureAfterAcceptCounter.AddValue(0L);
				ShadowRedundancyPerformanceCounters.perfCounters.MessagesFailedToBeMadeRedundant.RawValue = ShadowRedundancyPerformanceCounters.failureToMakeMessageRedundantCounter.AddValue(0L);
				ShadowRedundancyPerformanceCounters.perfCounters.LocalSiteShadowPercentage.RawValue = this.GetCounterValue(ShadowRedundancyPerformanceCounters.localRemoteShadowPercentageCounter);
				ShadowRedundancyPerformanceCounters.perfCounters.MessagesFailedToBeMadeRedundantPercentage.RawValue = this.GetCounterValue(ShadowRedundancyPerformanceCounters.messagesFailedToBeMadeRedundantPercentageCounter);
			}
		}

		private long GetCounterValue(SlidingPercentageCounter counter)
		{
			double slidingPercentage = counter.GetSlidingPercentage();
			if (counter.Denominator == 0L)
			{
				return 0L;
			}
			return Convert.ToInt64(slidingPercentage);
		}

		private const string PerfCounterInstanceName = "_total";

		private static readonly TimeSpan slidingWindowLength = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan bucketLength = TimeSpan.FromSeconds(10.0);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> shadowQueueLengthCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ShadowQueueLength);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> shadowHeartbeatLatencyCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ShadowHeartbeatLatencyAverageTime);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> shadowHeartbeatLatencyBaseCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ShadowHeartbeatLatencyAverageTimeBase);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> shadowFailureCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ShadowFailureCount);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> heartbeatFailureCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).HeartbeatFailureCount);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> shadowedMessageCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ShadowedMessageCount);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> resubmittedMessageCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => ShadowRedundancyInstancePerfCounters.GetInstance(name).ResubmittedMessageCount);

		private static readonly SlidingTotalCounter totalSmtpTimeoutsCounter = new SlidingTotalCounter(ShadowRedundancyPerformanceCounters.slidingWindowLength, ShadowRedundancyPerformanceCounters.bucketLength);

		private static readonly SlidingTotalCounter clientFailureAfterAcceptCounter = new SlidingTotalCounter(ShadowRedundancyPerformanceCounters.slidingWindowLength, ShadowRedundancyPerformanceCounters.bucketLength);

		private static readonly SlidingTotalCounter failureToMakeMessageRedundantCounter = new SlidingTotalCounter(ShadowRedundancyPerformanceCounters.slidingWindowLength, ShadowRedundancyPerformanceCounters.bucketLength);

		private static readonly SlidingPercentageCounter localRemoteShadowPercentageCounter = new SlidingPercentageCounter(ShadowRedundancyPerformanceCounters.slidingWindowLength, ShadowRedundancyPerformanceCounters.bucketLength);

		private static readonly SlidingPercentageCounter messagesFailedToBeMadeRedundantPercentageCounter = new SlidingPercentageCounter(ShadowRedundancyPerformanceCounters.slidingWindowLength, ShadowRedundancyPerformanceCounters.bucketLength);

		private static ShadowRedundancyPerfCountersInstance perfCounters = ShadowRedundancyPerfCounters.GetInstance("_total");

		private static QueuingPerfCountersInstance queuingPerfCounters = QueueManager.GetTotalPerfCounters();

		private ExPerformanceCounter[] performanceCounters;

		private GuardedTimer refreshSlidingCountersTimer;
	}
}
