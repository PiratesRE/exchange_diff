using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateDeferredActionMessagesResultFactory : StandardResultFactory
	{
		internal UpdateDeferredActionMessagesResultFactory() : base(RopId.UpdateDeferredActionMessages)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.UpdateDeferredActionMessages, ErrorCode.None);
		}
	}
}
