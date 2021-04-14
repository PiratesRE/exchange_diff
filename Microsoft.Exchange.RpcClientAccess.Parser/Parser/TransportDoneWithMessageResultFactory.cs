using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportDoneWithMessageResultFactory : StandardResultFactory
	{
		internal TransportDoneWithMessageResultFactory() : base(RopId.TransportDoneWithMessage)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.TransportDoneWithMessage, ErrorCode.None);
		}
	}
}
