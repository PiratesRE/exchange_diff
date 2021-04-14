using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver.Agents
{
	internal class AveragePerformanceCounterWrapper
	{
		internal AveragePerformanceCounterWrapper(ExPerformanceCounter performanceCounter)
		{
			this.performanceCounter = performanceCounter;
			this.slidingAverage = new SlidingAverageCounter(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(1.0));
		}

		internal void Update(long milliseconds)
		{
			this.slidingAverage.AddValue(milliseconds);
			lock (this.syncObject)
			{
				this.performanceCounter.RawValue = this.slidingAverage.CalculateAverage();
			}
		}

		private SlidingAverageCounter slidingAverage;

		private ExPerformanceCounter performanceCounter;

		private object syncObject = new object();
	}
}
