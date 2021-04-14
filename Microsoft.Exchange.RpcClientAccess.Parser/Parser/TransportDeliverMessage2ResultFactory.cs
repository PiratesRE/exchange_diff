using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportDeliverMessage2ResultFactory : StandardResultFactory
	{
		internal TransportDeliverMessage2ResultFactory() : base(RopId.TransportDeliverMessage2)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId messageId)
		{
			return new SuccessfulTransportDeliverMessage2Result(messageId);
		}
	}
}
