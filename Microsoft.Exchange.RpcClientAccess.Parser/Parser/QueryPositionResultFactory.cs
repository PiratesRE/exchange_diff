using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryPositionResultFactory : StandardResultFactory
	{
		internal QueryPositionResultFactory() : base(RopId.QueryPosition)
		{
		}

		public RopResult CreateSuccessfulResult(uint numerator, uint denominator)
		{
			return new SuccessfulQueryPositionResult(numerator, denominator);
		}
	}
}
