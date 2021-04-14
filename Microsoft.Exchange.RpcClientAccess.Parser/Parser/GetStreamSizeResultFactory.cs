using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetStreamSizeResultFactory : StandardResultFactory
	{
		internal GetStreamSizeResultFactory() : base(RopId.GetStreamSize)
		{
		}

		public RopResult CreateSuccessfulResult(uint streamSize)
		{
			return new SuccessfulGetStreamSizeResult(streamSize);
		}
	}
}
