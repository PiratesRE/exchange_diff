using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportDeliverMessageResultFactory : StandardResultFactory
	{
		internal TransportDeliverMessageResultFactory() : base(RopId.TransportDeliverMessage)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.TransportDeliverMessage, ErrorCode.None);
		}
	}
}
