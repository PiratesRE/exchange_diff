using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class TransportNoopExPerformanceCounter : IExPerformanceCounter
	{
		public long RawValue
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Increment()
		{
			return 0L;
		}

		public long Decrement()
		{
			return 0L;
		}

		public void Reset()
		{
		}

		public long IncrementBy(long incrementValue)
		{
			return 0L;
		}

		public static readonly TransportNoopExPerformanceCounter Instance = new TransportNoopExPerformanceCounter();
	}
}
