using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetStoreStateResultFactory : StandardResultFactory
	{
		internal GetStoreStateResultFactory() : base(RopId.GetStoreState)
		{
		}

		public RopResult CreateSuccessfulResult(StoreState storeState)
		{
			return new SuccessfulGetStoreStateResult(storeState);
		}
	}
}
