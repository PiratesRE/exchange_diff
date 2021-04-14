using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CommitStreamResultFactory : StandardResultFactory
	{
		internal CommitStreamResultFactory() : base(RopId.CommitStream)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.CommitStream, ErrorCode.None);
		}
	}
}
