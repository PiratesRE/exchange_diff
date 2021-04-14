using System;

namespace Microsoft.Exchange.Transport
{
	internal struct LatencyRecordPlus
	{
		public LatencyRecordPlus(ushort componentId, long startTime)
		{
			this = default(LatencyRecordPlus);
			this.latency = 0U;
			this.ComponentId = componentId;
			this.StartTime = startTime;
			this.IsComplete = false;
		}

		public LatencyRecordPlus(ushort componentId, TimeSpan latency)
		{
			this = default(LatencyRecordPlus);
			this.latency = (uint)LatencyRecordPlus.ConstrainToLimits(latency).TotalMilliseconds;
			this.ComponentId = componentId;
			this.StartTime = 0L;
			this.IsComplete = true;
		}

		public long StartTime { get; private set; }

		public ushort ComponentId { get; private set; }

		public bool IsComplete { get; private set; }

		public bool IsPending
		{
			get
			{
				return !this.IsComplete;
			}
		}

		public bool IsImplicitlyComplete { get; private set; }

		public TimeSpan Complete(long endTime, bool implictCompletion = false)
		{
			return this.Complete(endTime, this.ComponentId, implictCompletion);
		}

		public TimeSpan Complete(long endTime, ushort newComponentId, bool implicitCompletion = false)
		{
			TimeSpan result = LatencyRecordPlus.ConstrainToLimits(LatencyTracker.TimeSpanFromTicks(this.StartTime, endTime));
			this.latency = (uint)result.TotalMilliseconds;
			this.ComponentId = newComponentId;
			this.IsComplete = true;
			this.IsImplicitlyComplete = implicitCompletion;
			return result;
		}

		public TimeSpan CalculateLatency(long currentTime)
		{
			if (!this.IsComplete)
			{
				return LatencyTracker.TimeSpanFromTicks(this.StartTime, currentTime);
			}
			return TimeSpan.FromMilliseconds(this.latency);
		}

		public TimeSpan CalculateLatency()
		{
			return this.CalculateLatency(LatencyTracker.StopwatchProvider());
		}

		internal LatencyRecord AsCompletedRecord()
		{
			return new LatencyRecord(this.ComponentId, this.CalculateLatency());
		}

		internal PendingLatencyRecord AsPendingRecord()
		{
			return new PendingLatencyRecord(this.ComponentId, this.StartTime);
		}

		private static TimeSpan ConstrainToLimits(TimeSpan latency)
		{
			if (latency < TimeSpan.Zero)
			{
				return TimeSpan.Zero;
			}
			if (latency > LatencyRecordPlus.MaxLatency)
			{
				return LatencyRecordPlus.MaxLatency;
			}
			return latency;
		}

		private static readonly TimeSpan MaxLatency = TransportAppConfig.LatencyTrackerConfig.MaxLatency;

		private uint latency;
	}
}
