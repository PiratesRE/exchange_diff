using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LockRegionStreamResultFactory : StandardResultFactory
	{
		internal LockRegionStreamResultFactory() : base(RopId.LockRegionStream)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.LockRegionStream, ErrorCode.None);
		}
	}
}
