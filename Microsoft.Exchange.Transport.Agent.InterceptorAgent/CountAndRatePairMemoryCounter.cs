using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class CountAndRatePairMemoryCounter : ICountAndRatePairCounter
	{
		public CountAndRatePairMemoryCounter(string counterName, string averageCounterName, TimeSpan trackingLength, TimeSpan rateDuration, ICountAndRatePairCounter totalPairCounters = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty(counterName, counterName);
			ArgumentValidator.ThrowIfNullOrEmpty(averageCounterName, averageCounterName);
			this.averageCounterName = averageCounterName;
			this.runningCount = new MemoryCounter(counterName);
			this.slidingAverage = new SlidingAverageCounter(trackingLength, rateDuration);
			this.totalPairForAutoUpdate = totalPairCounters;
		}

		public void AddValue(long value)
		{
			this.runningCount.IncrementBy(value);
			this.slidingAverage.AddValue(value);
			if (this.totalPairForAutoUpdate != null)
			{
				this.totalPairForAutoUpdate.AddValue(value);
			}
		}

		public void UpdateAverage()
		{
		}

		public void GetDiagnosticInfo(XElement parent)
		{
			parent.Add(new XElement(ExPerformanceCounter.GetEncodedName(this.runningCount.CounterName), this.runningCount.RawValue));
			parent.Add(new XElement(ExPerformanceCounter.GetEncodedName(this.averageCounterName), this.slidingAverage.CalculateAverage()));
		}

		private readonly SlidingAverageCounter slidingAverage;

		private readonly ICountAndRatePairCounter totalPairForAutoUpdate;

		private readonly MemoryCounter runningCount;

		private readonly string averageCounterName;
	}
}
