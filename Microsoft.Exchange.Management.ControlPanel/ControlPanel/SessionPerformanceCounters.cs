using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class SessionPerformanceCounters
	{
		public SessionPerformanceCounters(PerfCounterGroup sessionsCounters, PerfCounterGroup requestsCounters)
		{
			this.sessionsCounter = sessionsCounters;
			this.requestsCounter = requestsCounters;
		}

		public virtual void IncreaseSessionCounter()
		{
			this.sessionsCounter.Increment();
		}

		public virtual void DecreaseSessionCounter()
		{
			this.sessionsCounter.Decrement();
		}

		public virtual void IncreaseRequestCounter()
		{
			this.requestsCounter.Increment();
		}

		public virtual void DecreaseRequestCounter()
		{
			this.requestsCounter.Decrement();
		}

		private PerfCounterGroup sessionsCounter;

		private PerfCounterGroup requestsCounter;
	}
}
