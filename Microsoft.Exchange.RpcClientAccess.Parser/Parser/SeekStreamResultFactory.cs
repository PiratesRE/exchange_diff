using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeekStreamResultFactory : StandardResultFactory
	{
		internal SeekStreamResultFactory() : base(RopId.SeekStream)
		{
		}

		public RopResult CreateSuccessfulResult(ulong resultOffset)
		{
			return new SuccessfulSeekStreamResult(resultOffset);
		}
	}
}
