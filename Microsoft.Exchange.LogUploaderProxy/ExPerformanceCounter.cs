using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class ExPerformanceCounter
	{
		internal ExPerformanceCounter(string categoryName, string counterName, string instanceName, ExPerformanceCounter totalInstanceCounter, params ExPerformanceCounter[] autoUpdateCounters)
		{
			this.exPerfCounterImpl = new ExPerformanceCounter(categoryName, counterName, instanceName, (totalInstanceCounter == null) ? null : totalInstanceCounter.exPerfCounterImpl, (from c in autoUpdateCounters
			select c.exPerfCounterImpl).ToArray<ExPerformanceCounter>());
		}

		public long RawValue
		{
			get
			{
				return this.exPerfCounterImpl.RawValue;
			}
			set
			{
				this.exPerfCounterImpl.RawValue = value;
			}
		}

		public long Decrement()
		{
			return this.exPerfCounterImpl.Decrement();
		}

		public long Increment()
		{
			return this.exPerfCounterImpl.Increment();
		}

		public long IncrementBy(long incrementValue)
		{
			return this.exPerfCounterImpl.IncrementBy(incrementValue);
		}

		public void Close()
		{
			this.exPerfCounterImpl.Close();
		}

		public virtual void Reset()
		{
			this.exPerfCounterImpl.Reset();
		}

		private readonly ExPerformanceCounter exPerfCounterImpl;
	}
}
