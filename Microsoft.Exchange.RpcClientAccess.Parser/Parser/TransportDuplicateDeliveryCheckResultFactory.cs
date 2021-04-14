using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportDuplicateDeliveryCheckResultFactory : StandardResultFactory
	{
		internal TransportDuplicateDeliveryCheckResultFactory() : base(RopId.TransportDuplicateDeliveryCheck)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.TransportDuplicateDeliveryCheck, ErrorCode.None);
		}
	}
}
