using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SafeCounter
	{
		public SafeCounter(ExPerformanceCounter wrappedCounter)
		{
			this.m_perfCounter = wrappedCounter;
			this.m_perfCounter.RawValue = 0L;
		}

		public void Reset()
		{
			Thread.VolatileWrite(ref this.m_value, 0L);
			this.m_perfCounter.RawValue = 0L;
		}

		public long Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				long num = Interlocked.Exchange(ref this.m_value, value);
				long incrementValue = value - num;
				this.m_perfCounter.IncrementBy(incrementValue);
			}
		}

		private long m_value;

		private ExPerformanceCounter m_perfCounter;
	}
}
