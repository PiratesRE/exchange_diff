using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class AverageCounter : IAverageCounter
	{
		public AverageCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase)
		{
			if (averageCount == null)
			{
				throw new ArgumentNullException("averageCount");
			}
			if (averageBase == null)
			{
				throw new ArgumentNullException("averageBase");
			}
			this.averageCount = averageCount;
			this.averageBase = averageBase;
		}

		public void AddSample(long sample)
		{
			this.averageCount.IncrementBy(sample);
			this.averageBase.Increment();
		}

		private ExPerformanceCounter averageCount;

		private ExPerformanceCounter averageBase;
	}
}
