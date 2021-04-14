using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IdFromLongTermIdResultFactory : StandardResultFactory
	{
		internal IdFromLongTermIdResultFactory() : base(RopId.IdFromLongTermId)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId storeId)
		{
			return new SuccessfulIdFromLongTermIdResult(storeId);
		}
	}
}
