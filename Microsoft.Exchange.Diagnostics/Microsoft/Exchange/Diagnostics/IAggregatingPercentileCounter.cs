using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IAggregatingPercentileCounter : IPercentileCounter
	{
		void IncrementValue(ref long value, long increment);
	}
}
