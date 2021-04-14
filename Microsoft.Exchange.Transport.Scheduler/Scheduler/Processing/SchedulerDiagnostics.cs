using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class SchedulerDiagnostics : RefreshableComponent, ISchedulerDiagnostics
	{
		public SchedulerDiagnostics(TimeSpan queueLogsRefreshTimeSpan, ISchedulerMetering metering, IQueueLogWriter queueLogWriter, Func<DateTime> timeProvider) : base(queueLogsRefreshTimeSpan, timeProvider)
		{
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			ArgumentValidator.ThrowIfNull("queueLogWriter", queueLogWriter);
			ArgumentValidator.ThrowIfNull("metering", metering);
			TimeSpan slidingWindowLength = TimeSpan.FromSeconds(60.0);
			TimeSpan bucketLength = TimeSpan.FromSeconds(5.0);
			this.queueLogWriter = queueLogWriter;
			this.metering = metering;
			this.receivedCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
			this.dispatchedCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
			this.throttledCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
			this.processedCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
			this.createdQueueCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
			this.deletedQueueCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, timeProvider);
		}

		public SchedulerDiagnostics(TimeSpan queueLogsRefreshTimeSpan, ISchedulerMetering metering, IQueueLogWriter queueLogWriter) : this(queueLogsRefreshTimeSpan, metering, queueLogWriter, () => DateTime.UtcNow)
		{
		}

		public SchedulerDiagnostics(Func<DateTime> timeProvider) : this(SchedulerDiagnostics.DefaultQueueLogRefreshTimeSpan, SchedulerDiagnostics.DefaultNoOpMetering, SchedulerDiagnostics.DefaultNoOpQueueLogWriter, timeProvider)
		{
		}

		public SchedulerDiagnostics() : this(() => DateTime.UtcNow)
		{
		}

		public void RegisterQueueLogging(IQueueLogProvider logProvider)
		{
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			lock (this.syncRoot)
			{
				this.queueLogProviders.Add(logProvider);
			}
		}

		public void Received()
		{
			Interlocked.Increment(ref this.receivedTotal);
			this.receivedCounter.AddValue(1L);
		}

		public void Dispatched()
		{
			this.dispatchedTotal += 1L;
			this.dispatchedCounter.AddValue(1L);
		}

		public void Throttled()
		{
			this.throttledTotal += 1L;
			this.throttledCounter.AddValue(1L);
		}

		public void Processed()
		{
			this.processedTotal += 1L;
			this.processedCounter.AddValue(1L);
		}

		public void ScopedQueueCreated(int count)
		{
			this.createdQueueCounter.AddValue((long)count);
		}

		public void ScopedQueueDestroyed(int count)
		{
			this.deletedQueueCounter.AddValue((long)count);
		}

		public void VisitCurrentScopedQueues(IDictionary<IMessageScope, ScopedQueue> currentQueues)
		{
			DateTime t = DateTime.MaxValue;
			DateTime t2 = DateTime.MaxValue;
			long num = 0L;
			if (currentQueues != null)
			{
				foreach (KeyValuePair<IMessageScope, ScopedQueue> keyValuePair in currentQueues)
				{
					ScopedQueue value = keyValuePair.Value;
					if (value.CreateDateTime < t)
					{
						t = value.CreateDateTime;
					}
					if (!value.IsEmpty && value.Locked && value.LockDateTime < t2)
					{
						t2 = value.LockDateTime;
					}
				}
				num = (long)currentQueues.Count;
			}
			this.oldestScopedQueueCreateTime = t;
			this.oldestScopedLockTimeStamp = t2;
			this.totalScopedQueues = num;
		}

		public SchedulerDiagnosticsInfo GetDiagnosticsInfo()
		{
			return new SchedulerDiagnosticsInfo
			{
				Dispatched = this.dispatchedTotal,
				DispatchRate = this.dispatchedCounter.Sum,
				Received = this.receivedTotal,
				ReceiveRate = this.receivedCounter.Sum,
				Throttled = this.throttledTotal,
				ThrottleRate = this.throttledCounter.Sum,
				Processed = this.processedTotal,
				ProcessRate = this.processedCounter.Sum,
				TotalScopedQueues = this.totalScopedQueues,
				ScopedQueuesCreatedRate = this.createdQueueCounter.Sum,
				ScopedQueuesDestroyedRate = this.deletedQueueCounter.Sum,
				OldestLockTimeStamp = this.oldestScopedLockTimeStamp,
				OldestScopedQueueCreateTime = this.oldestScopedQueueCreateTime
			};
		}

		protected override void Refresh(DateTime timestamp)
		{
			List<QueueLogInfo> list = new List<QueueLogInfo>();
			int count = this.queueLogProviders.Count;
			for (int i = 0; i < count; i++)
			{
				list.AddRange(this.queueLogProviders[i].FlushLogs(timestamp, this.metering));
			}
			this.lastQueueLogEntries = list;
			this.WriteCurrentLogEntries();
		}

		private void WriteCurrentLogEntries()
		{
			List<QueueLogInfo> list = this.lastQueueLogEntries;
			foreach (QueueLogInfo logInfo in list)
			{
				this.queueLogWriter.Write(logInfo);
			}
		}

		public static readonly TimeSpan DefaultQueueLogRefreshTimeSpan = TimeSpan.FromMinutes(15.0);

		public static readonly IQueueLogWriter DefaultNoOpQueueLogWriter = new NoOpQueueLogWriter();

		public static readonly ISchedulerMetering DefaultNoOpMetering = new NoOpMetering();

		private readonly SlidingTotalCounter receivedCounter;

		private readonly SlidingTotalCounter dispatchedCounter;

		private readonly SlidingTotalCounter throttledCounter;

		private readonly SlidingTotalCounter processedCounter;

		private readonly SlidingTotalCounter createdQueueCounter;

		private readonly SlidingTotalCounter deletedQueueCounter;

		private readonly List<IQueueLogProvider> queueLogProviders = new List<IQueueLogProvider>();

		private readonly object syncRoot = new object();

		private readonly IQueueLogWriter queueLogWriter;

		private readonly ISchedulerMetering metering;

		private List<QueueLogInfo> lastQueueLogEntries = new List<QueueLogInfo>();

		private long receivedTotal;

		private long dispatchedTotal;

		private long throttledTotal;

		private long processedTotal;

		private long totalScopedQueues;

		private DateTime oldestScopedQueueCreateTime = DateTime.MaxValue;

		private DateTime oldestScopedLockTimeStamp = DateTime.MaxValue;
	}
}
