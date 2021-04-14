using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class NullPerformanceCounter : IExPerformanceCounter
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

		public long Decrement()
		{
			return 0L;
		}

		public long Increment()
		{
			return 0L;
		}

		public long IncrementBy(long incrementValue)
		{
			return 0L;
		}

		public void Reset()
		{
		}

		public static readonly NullPerformanceCounter Instance = new NullPerformanceCounter();
	}
}
