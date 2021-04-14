using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class MemoryCounter
	{
		internal MemoryCounter(string counterName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("counterName", counterName);
			this.counterName = counterName;
			this.counter = 0L;
		}

		internal long RawValue
		{
			get
			{
				return this.counter;
			}
			set
			{
				this.counter = value;
			}
		}

		internal string CounterName
		{
			get
			{
				return this.counterName;
			}
		}

		internal long Increment()
		{
			return Interlocked.Increment(ref this.counter);
		}

		internal long IncrementBy(long val)
		{
			return Interlocked.Add(ref this.counter, val);
		}

		private readonly string counterName;

		private long counter;
	}
}
