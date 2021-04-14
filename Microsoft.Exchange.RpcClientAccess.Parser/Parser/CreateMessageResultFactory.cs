using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateMessageResultFactory : StandardResultFactory
	{
		internal CreateMessageResultFactory() : base(RopId.CreateMessage)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId? messageId)
		{
			return new SuccessfulCreateMessageResult(serverObject, messageId);
		}
	}
}
