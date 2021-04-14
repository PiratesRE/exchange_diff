using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class CostFactory : ICostFactory
	{
		public CostIndex CreateIndex(Cost[] costIndices, int maxConditionsPerBucket, GetCost getCost, IsBelowCost isBelowCost, ShouldAddToIndex shouldAddToIndex, Trace tracer)
		{
			return new CostIndex(costIndices, maxConditionsPerBucket, getCost, isBelowCost, shouldAddToIndex, tracer);
		}

		public CostMap CreateMap(CostConfiguration costConfig, IsLocked isLocked, IsLockedOnQueue isLockedOnQueue, Trace tracer)
		{
			return new CostMap(costConfig, this, isLocked, isLockedOnQueue, tracer);
		}
	}
}
