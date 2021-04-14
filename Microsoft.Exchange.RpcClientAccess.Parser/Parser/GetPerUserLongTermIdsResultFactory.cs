using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPerUserLongTermIdsResultFactory : StandardResultFactory
	{
		internal GetPerUserLongTermIdsResultFactory() : base(RopId.GetPerUserLongTermIds)
		{
		}

		public RopResult CreateSuccessfulResult(StoreLongTermId[] longTermIds)
		{
			return new SuccessfulGetPerUserLongTermIdsResult(longTermIds);
		}
	}
}
