using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IExPerformanceCounter
	{
		long RawValue { get; set; }

		long Increment();

		long Decrement();

		void Reset();

		long IncrementBy(long incrementValue);
	}
}
