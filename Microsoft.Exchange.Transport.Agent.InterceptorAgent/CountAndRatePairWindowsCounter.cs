using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class CountAndRatePairWindowsCounter : ICountAndRatePairCounter
	{
		internal CountAndRatePairWindowsCounter(ExPerformanceCounter runningCount, ExPerformanceCounter average, TimeSpan trackingLength, TimeSpan rateDuration, ICountAndRatePairCounter totalPairForAutoUpdate)
		{
			ArgumentValidator.ThrowIfNull("runningCount", runningCount);
			ArgumentValidator.ThrowIfNull("average", average);
			this.runningCount = runningCount;
			this.average = average;
			this.slidingAverage = new SlidingAverageCounter(trackingLength, rateDuration);
			this.totalPairForAutoUpdate = totalPairForAutoUpdate;
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
			this.average.RawValue = this.slidingAverage.CalculateAverage();
		}

		public void GetDiagnosticInfo(XElement parent)
		{
			parent.Add(new XElement(ExPerformanceCounter.GetEncodedName(this.runningCount.CounterName), this.runningCount.NextValue()));
			parent.Add(new XElement(ExPerformanceCounter.GetEncodedName(this.average.CounterName), this.average.NextValue()));
		}

		private readonly ExPerformanceCounter runningCount;

		private readonly ExPerformanceCounter average;

		private readonly SlidingAverageCounter slidingAverage;

		private readonly ICountAndRatePairCounter totalPairForAutoUpdate;
	}
}
