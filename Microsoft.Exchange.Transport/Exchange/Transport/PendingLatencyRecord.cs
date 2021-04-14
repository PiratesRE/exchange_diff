using System;

namespace Microsoft.Exchange.Transport
{
	internal struct PendingLatencyRecord
	{
		public PendingLatencyRecord(ushort componentId, long startTime)
		{
			this.componentId = componentId;
			this.startTime = startTime;
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

		public long StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public TimeSpan CalculatePendingLatency(long currentTime)
		{
			return LatencyTracker.TimeSpanFromTicks(this.startTime, currentTime);
		}

		private ushort componentId;

		private long startTime;
	}
}
