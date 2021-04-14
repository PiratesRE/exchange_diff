using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class HistoricalSlidingTotalCounter
	{
		public HistoricalSlidingTotalCounter(TimeSpan slidingWindowLength, TimeSpan bucketLength, DateTime trackingStartTime)
		{
			this.eventTimeStamp = trackingStartTime;
			this.slidingTotalCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, new Func<DateTime>(this.CurrentTimeProvider));
		}

		public long AddValue(long value, DateTime eventTimeStamp)
		{
			long result;
			lock (this.syncObject)
			{
				this.eventTimeStamp = eventTimeStamp;
				result = this.slidingTotalCounter.AddValue(value);
			}
			return result;
		}

		public long SumAt(DateTime timeStamp)
		{
			return this.AddValue(0L, timeStamp);
		}

		private DateTime CurrentTimeProvider()
		{
			return this.eventTimeStamp;
		}

		private readonly SlidingTotalCounter slidingTotalCounter;

		private readonly object syncObject = new object();

		private DateTime eventTimeStamp;
	}
}
