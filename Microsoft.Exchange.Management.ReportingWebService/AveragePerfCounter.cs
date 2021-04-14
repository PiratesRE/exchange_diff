using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class AveragePerfCounter
	{
		public AveragePerfCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase)
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
