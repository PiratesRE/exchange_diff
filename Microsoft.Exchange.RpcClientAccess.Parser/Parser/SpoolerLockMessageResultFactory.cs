using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SpoolerLockMessageResultFactory : StandardResultFactory
	{
		internal SpoolerLockMessageResultFactory() : base(RopId.SpoolerLockMessage)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SpoolerLockMessage, ErrorCode.None);
		}
	}
}
