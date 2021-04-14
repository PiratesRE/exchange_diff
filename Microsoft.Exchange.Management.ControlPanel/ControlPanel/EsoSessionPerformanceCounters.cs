using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EsoSessionPerformanceCounters : SessionPerformanceCounters
	{
		public EsoSessionPerformanceCounters(PerfCounterGroup sessionsCounters, PerfCounterGroup requestsCounters, PerfCounterGroup esoSessionsCounters, PerfCounterGroup esoRequestsCounters) : base(sessionsCounters, requestsCounters)
		{
			this.esoSessionsCounter = esoSessionsCounters;
			this.esoRequestsCounter = esoRequestsCounters;
		}

		public override void IncreaseSessionCounter()
		{
			base.IncreaseSessionCounter();
			this.esoSessionsCounter.Increment();
		}

		public override void DecreaseSessionCounter()
		{
			base.DecreaseSessionCounter();
			this.esoSessionsCounter.Decrement();
		}

		public override void IncreaseRequestCounter()
		{
			base.IncreaseRequestCounter();
			this.esoRequestsCounter.Increment();
		}

		public override void DecreaseRequestCounter()
		{
			base.DecreaseRequestCounter();
			this.esoRequestsCounter.Decrement();
		}

		private PerfCounterGroup esoSessionsCounter;

		private PerfCounterGroup esoRequestsCounter;
	}
}
