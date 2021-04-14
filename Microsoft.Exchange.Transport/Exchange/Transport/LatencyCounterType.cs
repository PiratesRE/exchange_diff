using System;

namespace Microsoft.Exchange.Transport
{
	internal enum LatencyCounterType
	{
		None,
		Percentile,
		PrioritizedPercentile,
		LongRangePercentile
	}
}
