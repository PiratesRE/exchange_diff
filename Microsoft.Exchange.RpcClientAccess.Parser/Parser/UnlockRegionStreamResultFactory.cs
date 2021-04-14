using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UnlockRegionStreamResultFactory : StandardResultFactory
	{
		internal UnlockRegionStreamResultFactory() : base(RopId.UnlockRegionStream)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.UnlockRegionStream, ErrorCode.None);
		}
	}
}
