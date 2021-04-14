using System;

namespace Microsoft.Exchange.Transport
{
	internal struct LatencyRecord
	{
		public LatencyRecord(ushort componentId, TimeSpan latency)
		{
			this.componentId = componentId;
			this.latency = (uint)LatencyRecord.ConstrainToLimits(latency).TotalMilliseconds;
		}

		public ushort ComponentId
		{
			get
			{
				return this.componentId;
			}
		}

		public string ComponentShortName
		{
			get
			{
				return LatencyTracker.GetShortName(this.componentId);
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return TimeSpan.FromMilliseconds(this.latency);
			}
		}

		private static TimeSpan ConstrainToLimits(TimeSpan latency)
		{
			if (latency < TimeSpan.Zero)
			{
				return TimeSpan.Zero;
			}
			if (latency > LatencyRecord.MaxLatency)
			{
				return LatencyRecord.MaxLatency;
			}
			return latency;
		}

		public static readonly LatencyRecord Empty = new LatencyRecord(0, TimeSpan.Zero);

		private static readonly TimeSpan MaxLatency = TransportAppConfig.LatencyTrackerConfig.MaxLatency;

		private readonly ushort componentId;

		private readonly uint latency;
	}
}
