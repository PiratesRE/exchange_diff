using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetTransportResultFactory : StandardResultFactory
	{
		internal SetTransportResultFactory() : base(RopId.SetTransport)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId transportQueueFolderId)
		{
			return new SuccessfulSetTransportResult(transportQueueFolderId);
		}
	}
}
