using System;
using System.Threading;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Optics
{
	internal class PerformanceCounterAccessor : IPerformanceCounterAccessor
	{
		public PerformanceCounterAccessor(string instanceName)
		{
			this.counters = TaskDistributionFabricPerfCounters.GetInstance(instanceName);
			SlidingTotalCounter counter = new SlidingTotalCounter(PerformanceCounterAccessor.SlidingCounterInterval, PerformanceCounterAccessor.SlidingCounterPrecision);
			this.successfulRequests = new PerformanceCounterAccessor.PeriodicSlidingTotalCounter(PerformanceCounterAccessor.BatchDuration, counter);
			SlidingTotalCounter counter2 = new SlidingTotalCounter(PerformanceCounterAccessor.SlidingCounterInterval, PerformanceCounterAccessor.SlidingCounterPrecision);
			this.failedRequests = new PerformanceCounterAccessor.PeriodicSlidingTotalCounter(PerformanceCounterAccessor.BatchDuration, counter2);
			PercentileCounter counter3 = new PercentileCounter(PerformanceCounterAccessor.PercentileCounterInterval, PerformanceCounterAccessor.PercentileCounterPrecision, 1L, 1000L);
			this.queueLength = new PerformanceCounterAccessor.PeriodicPercentileCounter(PerformanceCounterAccessor.BatchDuration, counter3);
			PercentileCounter counter4 = new PercentileCounter(PerformanceCounterAccessor.PercentileCounterInterval, PerformanceCounterAccessor.PercentileCounterPrecision, 10L, 10000L);
			this.queueLatency = new PerformanceCounterAccessor.PeriodicPercentileCounter(PerformanceCounterAccessor.BatchDuration, counter4);
			PercentileCounter counter5 = new PercentileCounter(PerformanceCounterAccessor.PercentileCounterInterval, PerformanceCounterAccessor.PercentileCounterPrecision, 5L, 5000L);
			this.processorLatency = new PerformanceCounterAccessor.PeriodicPercentileCounter(PerformanceCounterAccessor.BatchDuration, counter5);
		}

		public void AddQueueEvent(QueueEvent qEvent)
		{
			switch (qEvent)
			{
			case QueueEvent.Enqueue:
				this.counters.CurrentQueueLength.Increment();
				break;
			case QueueEvent.Dequeue:
				this.counters.CurrentQueueLength.Decrement();
				break;
			}
			if (this.queueLength.AddValue(this.counters.CurrentQueueLength.RawValue))
			{
				this.UpdateQueueSizePercentileCounters();
			}
		}

		public void AddDequeueLatency(long latency)
		{
			if (this.queueLatency.AddValue(latency))
			{
				this.UpdateQueueLatencyPercentileCounters();
			}
		}

		public void AddProcessingCompletionEvent(ProcessingCompletionEvent pEvent, long latency)
		{
			switch (pEvent)
			{
			case ProcessingCompletionEvent.Success:
				if (this.successfulRequests.AddValue(1L))
				{
					this.counters.RecentSuccessfulRequestsCount.RawValue = this.successfulRequests.Sum;
				}
				break;
			case ProcessingCompletionEvent.Failure:
				if (this.failedRequests.AddValue(1L))
				{
					this.counters.RecentFailedRequestsCount.RawValue = this.failedRequests.Sum;
				}
				break;
			}
			if (this.processorLatency.AddValue(latency))
			{
				this.UpdateProcessorLatencyPercentileCounters();
			}
		}

		public void AddProcessorEvent(ProcessorEvent pEvent)
		{
			switch (pEvent)
			{
			case ProcessorEvent.StartProcessing:
				this.counters.CurrentRequests.Increment();
				return;
			case ProcessorEvent.EndProcessing:
				this.counters.CurrentRequests.Decrement();
				return;
			default:
				return;
			}
		}

		public void UpdateCounters()
		{
			this.queueLength.SubmitBatch();
			this.queueLatency.SubmitBatch();
			this.UpdateQueueSizePercentileCounters();
			this.UpdateQueueLatencyPercentileCounters();
			this.successfulRequests.SubmitBatch();
			this.failedRequests.SubmitBatch();
			this.processorLatency.SubmitBatch();
			this.counters.RecentSuccessfulRequestsCount.RawValue = this.successfulRequests.Sum;
			this.counters.RecentFailedRequestsCount.RawValue = this.failedRequests.Sum;
			this.UpdateProcessorLatencyPercentileCounters();
		}

		private void UpdateQueueSizePercentileCounters()
		{
			this.counters.QueueSize75Percentile.RawValue = this.queueLength.PercentileQuery(75.0);
			this.counters.QueueSize90Percentile.RawValue = this.queueLength.PercentileQuery(90.0);
			this.counters.QueueSize99Percentile.RawValue = this.queueLength.PercentileQuery(99.0);
		}

		private void UpdateQueueLatencyPercentileCounters()
		{
			this.counters.QueueLatency75Percentile.RawValue = this.queueLatency.PercentileQuery(75.0);
			this.counters.QueueLatency90Percentile.RawValue = this.queueLatency.PercentileQuery(90.0);
			this.counters.QueueLatency99Percentile.RawValue = this.queueLatency.PercentileQuery(99.0);
		}

		private void UpdateProcessorLatencyPercentileCounters()
		{
			this.counters.ProcessorLatency75Percentile.RawValue = this.processorLatency.PercentileQuery(75.0);
			this.counters.ProcessorLatency90Percentile.RawValue = this.processorLatency.PercentileQuery(90.0);
			this.counters.ProcessorLatency99Percentile.RawValue = this.processorLatency.PercentileQuery(99.0);
		}

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromSeconds(15.0);

		private static readonly TimeSpan PercentileCounterInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan PercentileCounterPrecision = TimeSpan.FromSeconds(15.0);

		private static readonly TimeSpan BatchDuration = TimeSpan.FromSeconds(30.0);

		private TaskDistributionFabricPerfCountersInstance counters;

		private PerformanceCounterAccessor.PeriodicSlidingTotalCounter successfulRequests;

		private PerformanceCounterAccessor.PeriodicSlidingTotalCounter failedRequests;

		private PerformanceCounterAccessor.PeriodicPercentileCounter queueLength;

		private PerformanceCounterAccessor.PeriodicPercentileCounter queueLatency;

		private PerformanceCounterAccessor.PeriodicPercentileCounter processorLatency;

		private class PeriodicPercentileCounter
		{
			public PeriodicPercentileCounter(TimeSpan batchDuration, PercentileCounter counter)
			{
				this.batchDuration = batchDuration;
				this.counter = counter;
				this.syncObj = new object();
			}

			public bool AddValue(long value)
			{
				if (value < 0L)
				{
					value = 0L;
				}
				Interlocked.Add(ref this.batchSum, value);
				Interlocked.Increment(ref this.batchCount);
				if (DateTime.UtcNow.Subtract(this.lastSubmitTime) >= this.batchDuration)
				{
					this.SubmitBatch();
					return true;
				}
				return false;
			}

			public void SubmitBatch()
			{
				lock (this.syncObj)
				{
					long num = Interlocked.Read(ref this.batchSum);
					long num2 = Interlocked.Read(ref this.batchCount);
					if (num2 != 0L)
					{
						long value = num / num2;
						this.counter.AddValue(value);
						this.batchCount = 0L;
						this.batchSum = 0L;
					}
					this.lastSubmitTime = DateTime.UtcNow;
				}
			}

			public long PercentileQuery(double percentage)
			{
				return this.counter.PercentileQuery(percentage);
			}

			private readonly TimeSpan batchDuration;

			private long batchSum;

			private long batchCount;

			private DateTime lastSubmitTime;

			private object syncObj;

			private PercentileCounter counter;
		}

		private class PeriodicSlidingTotalCounter
		{
			public PeriodicSlidingTotalCounter(TimeSpan batchDuration, SlidingTotalCounter counter)
			{
				this.batchDuration = batchDuration;
				this.counter = counter;
				this.syncObj = new object();
			}

			public long Sum
			{
				get
				{
					return this.counter.Sum;
				}
			}

			public bool AddValue(long value)
			{
				Interlocked.Add(ref this.batchTotal, value);
				if (DateTime.UtcNow.Subtract(this.lastSubmitTime) >= this.batchDuration)
				{
					this.SubmitBatch();
					return true;
				}
				return false;
			}

			public void SubmitBatch()
			{
				lock (this.syncObj)
				{
					long value = Interlocked.Read(ref this.batchTotal);
					this.batchTotal = 0L;
					this.counter.AddValue(value);
					this.lastSubmitTime = DateTime.UtcNow;
				}
			}

			private readonly TimeSpan batchDuration;

			private long batchTotal;

			private DateTime lastSubmitTime;

			private object syncObj;

			private SlidingTotalCounter counter;
		}
	}
}
