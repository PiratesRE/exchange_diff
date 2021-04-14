using System;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskPerformanceRecord : IDisposable
	{
		private static ulong NextCorrelationId
		{
			get
			{
				return (ulong)Interlocked.Increment(ref TaskPerformanceRecord.correlationIdSequence);
			}
		}

		public TaskPerformanceRecord(string taskName, LatencyDetectionContextFactory latencyDetectionContextFactory, ExEventLog.EventTuple startTuple, ExEventLog.EventTuple endTuple, ExEventLog eventLog)
		{
			this.TaskName = taskName;
			this.latencyDetectionContextFactory = latencyDetectionContextFactory;
			this.startTuple = startTuple;
			this.endTuple = endTuple;
			this.eventLog = eventLog;
			this.correlationId = TaskPerformanceRecord.NextCorrelationId.ToString();
		}

		public TaskPerformanceRecord(string taskName, LatencyDetectionContextFactory latencyDetectionContextFactory, ExEventLog.EventTuple startTuple, ExEventLog.EventTuple endTuple, ExEventLog eventLog, params IPerformanceDataProvider[] performanceDataProviders) : this(taskName, latencyDetectionContextFactory, startTuple, endTuple, eventLog)
		{
			this.Start(performanceDataProviders);
		}

		void IDisposable.Dispose()
		{
			this.Stop();
		}

		public void Start(params IPerformanceDataProvider[] performanceDataProviders)
		{
			this.eventLog.LogEvent(this.startTuple, null, new object[]
			{
				this.TaskName,
				this.correlationId
			});
			this.latencyDetectionContext = this.latencyDetectionContextFactory.CreateContext("15.00.1497.015", this.TaskName, performanceDataProviders);
		}

		public TimeSpan Stop()
		{
			if (this.IsCollecting)
			{
				this.performanceData = this.latencyDetectionContext.StopAndFinalizeCollection();
				object obj = string.Empty;
				if (this.eventLog.IsEventCategoryEnabled(this.endTuple.CategoryId, ExEventLog.EventLevel.High))
				{
					string text = this.latencyDetectionContext.ToString("s");
					if (text.Length > 32766)
					{
						text = text.Substring(0, 32766);
					}
					obj = text;
				}
				this.eventLog.LogEvent(this.endTuple, null, new object[]
				{
					this.TaskName,
					this.latencyDetectionContext.Elapsed,
					obj,
					this.correlationId
				});
			}
			return this.latencyDetectionContext.Elapsed;
		}

		public string TaskName { get; private set; }

		public bool IsCollecting
		{
			get
			{
				return this.latencyDetectionContext != null && this.performanceData == null;
			}
		}

		public PerformanceData this[int providerIndex]
		{
			get
			{
				if (this.performanceData != null)
				{
					return this.performanceData[providerIndex].Difference;
				}
				return PerformanceData.Zero;
			}
		}

		private static long correlationIdSequence;

		private LatencyDetectionContextFactory latencyDetectionContextFactory;

		private LatencyDetectionContext latencyDetectionContext;

		private TaskPerformanceData[] performanceData;

		private ExEventLog.EventTuple startTuple;

		private ExEventLog.EventTuple endTuple;

		private ExEventLog eventLog;

		private string correlationId;
	}
}
