using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceCopyMessagesResultFactory : StandardResultFactory
	{
		internal FastTransferSourceCopyMessagesResultFactory() : base(RopId.FastTransferSourceCopyMessages)
		{
		}

		public SuccessfulFastTransferSourceCopyMessagesResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulFastTransferSourceCopyMessagesResult(serverObject);
		}
	}
}
