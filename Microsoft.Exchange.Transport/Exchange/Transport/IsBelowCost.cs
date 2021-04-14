using System;

namespace Microsoft.Exchange.Transport
{
	internal delegate IsBelowCostResult IsBelowCost(WaitCondition condition, Cost costIndex, object state, bool allowAboveThreshold);
}
