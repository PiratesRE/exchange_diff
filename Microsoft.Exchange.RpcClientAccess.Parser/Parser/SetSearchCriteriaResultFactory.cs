using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetSearchCriteriaResultFactory : StandardResultFactory
	{
		internal SetSearchCriteriaResultFactory() : base(RopId.SetSearchCriteria)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetSearchCriteria, ErrorCode.None);
		}
	}
}
