using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PerfCounterWithAverageRate
	{
		public PerfCounterWithAverageRate(ExPerformanceCounter normalCounter, ExPerformanceCounter rateCounter, ExPerformanceCounter baseCounter, int counterUnit, TimeSpan timeUnit)
		{
			this.normalCounter = normalCounter;
			this.rateCounter = rateCounter;
			this.baseCounter = baseCounter;
			this.counterUnit = counterUnit;
			this.timeUnitSeconds = (int)timeUnit.TotalSeconds;
			this.lastTimestamp = ExDateTime.UtcNow;
		}

		public void IncrementBy(long delta)
		{
			if (this.normalCounter != null)
			{
				this.normalCounter.IncrementBy(delta);
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			TimeSpan timeSpan = utcNow - this.lastTimestamp;
			this.lastTimestamp = utcNow;
			int num = (int)(timeSpan.TotalSeconds * (double)this.counterUnit);
			this.rateCounter.IncrementBy(delta * (long)this.timeUnitSeconds);
			this.baseCounter.IncrementBy((long)num);
		}

		private ExPerformanceCounter normalCounter;

		private ExPerformanceCounter rateCounter;

		private ExPerformanceCounter baseCounter;

		private int counterUnit;

		private int timeUnitSeconds;

		private ExDateTime lastTimestamp;
	}
}
