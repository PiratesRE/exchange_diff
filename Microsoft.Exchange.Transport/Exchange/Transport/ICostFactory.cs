using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal interface ICostFactory
	{
		CostIndex CreateIndex(Cost[] costIndices, int maxConditionsPerBucket, GetCost getCost, IsBelowCost isBelowCost, ShouldAddToIndex shouldAddToIndex, Trace tracer);

		CostMap CreateMap(CostConfiguration costConfig, IsLocked isLocked, IsLockedOnQueue isLockedOnQueue, Trace tracer);
	}
}
