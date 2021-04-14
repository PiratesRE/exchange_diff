using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal class MessageResubmissionPerfCounters : IMessageResubmissionPerfCounters
	{
		public MessageResubmissionPerfCounters()
		{
			this.refreshSlidingCountersTimer = new GuardedTimer(delegate(object state)
			{
				this.RefreshSlidingCounters();
			}, null, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0));
		}

		public static IMessageResubmissionPerfCounters Instance
		{
			get
			{
				return MessageResubmissionPerfCounters.defaultCounters;
			}
			set
			{
				MessageResubmissionPerfCounters.defaultCounters = value;
			}
		}

		public void ResetCounters()
		{
			MessageResubmissionPerfCounters.perfCounters.SafetyNetResubmissionCount.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.ShadowSafetyNetResubmissionCount.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.ResubmitLatencyAverageTime.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.ResubmitLatencyAverageTimeBase.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.ResubmitRequestCount.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.RecentResubmitRequestCount.RawValue = 0L;
			MessageResubmissionPerfCounters.perfCounters.RecentShadowResubmitRequestCount.RawValue = 0L;
		}

		public void UpdateResubmissionCount(int count, bool isShadowResubmit)
		{
			if (MessageResubmissionPerfCounters.perfCounters != null)
			{
				if (isShadowResubmit)
				{
					MessageResubmissionPerfCounters.perfCounters.ShadowSafetyNetResubmissionCount.IncrementBy((long)count);
					return;
				}
				MessageResubmissionPerfCounters.perfCounters.SafetyNetResubmissionCount.IncrementBy((long)count);
			}
		}

		public ITimerCounter ResubmitMessagesLatencyCounter()
		{
			return new AverageTimeCounter(MessageResubmissionPerfCounters.perfCounters.ResubmitLatencyAverageTime, MessageResubmissionPerfCounters.perfCounters.ResubmitLatencyAverageTimeBase);
		}

		public void UpdateResubmitRequestCount(ResubmitRequestState state, int changeAmount)
		{
			MessageResubmissionPerfCounters.resubmitRequestCountCounters.Get(state.ToString()).IncrementBy((long)changeAmount);
			MessageResubmissionPerfCounters.perfCounters.ResubmitRequestCount.IncrementBy((long)changeAmount);
		}

		public void ChangeResubmitRequestState(ResubmitRequestState oldState, ResubmitRequestState newState)
		{
			MessageResubmissionPerfCounters.resubmitRequestCountCounters.Get(oldState.ToString()).IncrementBy(-1L);
			MessageResubmissionPerfCounters.resubmitRequestCountCounters.Get(newState.ToString()).IncrementBy(1L);
		}

		public void IncrementRecentRequestCount(bool isShadowResubmit)
		{
			if (!isShadowResubmit)
			{
				MessageResubmissionPerfCounters.perfCounters.RecentResubmitRequestCount.RawValue = MessageResubmissionPerfCounters.recentResubmitRequestCounter.AddValue(1L);
				return;
			}
			MessageResubmissionPerfCounters.perfCounters.RecentShadowResubmitRequestCount.RawValue = MessageResubmissionPerfCounters.recentShadowResubmitRequestCounter.AddValue(1L);
		}

		public void RecordResubmitRequestTimeSpan(TimeSpan timeSpan)
		{
			MessageResubmissionPerfCounters.averageResubmitRequestTimeSpanCounter.AddValue((long)timeSpan.TotalSeconds);
			long num;
			MessageResubmissionPerfCounters.perfCounters.AverageResubmitRequestTimeSpan.RawValue = MessageResubmissionPerfCounters.averageResubmitRequestTimeSpanCounter.CalculateAverageAcrossAllSamples(out num);
		}

		private void RefreshSlidingCounters()
		{
			MessageResubmissionPerfCounters.perfCounters.RecentResubmitRequestCount.RawValue = MessageResubmissionPerfCounters.recentResubmitRequestCounter.AddValue(0L);
			MessageResubmissionPerfCounters.perfCounters.RecentShadowResubmitRequestCount.RawValue = MessageResubmissionPerfCounters.recentShadowResubmitRequestCounter.AddValue(0L);
			long num;
			MessageResubmissionPerfCounters.perfCounters.AverageResubmitRequestTimeSpan.RawValue = MessageResubmissionPerfCounters.averageResubmitRequestTimeSpanCounter.CalculateAverageAcrossAllSamples(out num);
		}

		private const string PerfCounterInstanceName = "_total";

		private static readonly TimeSpan slidingWindowLength = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan bucketLength = TimeSpan.FromSeconds(10.0);

		private static readonly AutoReadThroughCache<string, ExPerformanceCounter> resubmitRequestCountCounters = new AutoReadThroughCache<string, ExPerformanceCounter>((string name) => MessageResubmissionPerformanceCounters.GetInstance(name).ResubmitRequestCount);

		private static readonly SlidingTotalCounter recentResubmitRequestCounter = new SlidingTotalCounter(MessageResubmissionPerfCounters.slidingWindowLength, MessageResubmissionPerfCounters.bucketLength);

		private static readonly SlidingTotalCounter recentShadowResubmitRequestCounter = new SlidingTotalCounter(MessageResubmissionPerfCounters.slidingWindowLength, MessageResubmissionPerfCounters.bucketLength);

		private static readonly SlidingAverageCounter averageResubmitRequestTimeSpanCounter = new SlidingAverageCounter(MessageResubmissionPerfCounters.slidingWindowLength, MessageResubmissionPerfCounters.bucketLength);

		private static MessageResubmissionPerformanceCountersInstance perfCounters = MessageResubmissionPerformanceCounters.GetInstance("_total");

		private static IMessageResubmissionPerfCounters defaultCounters = new MessageResubmissionPerfCounters();

		private GuardedTimer refreshSlidingCountersTimer;
	}
}
