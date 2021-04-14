using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateMessageExtendedResultFactory : StandardResultFactory
	{
		internal CreateMessageExtendedResultFactory() : base(RopId.CreateMessageExtended)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId? messageId)
		{
			return new SuccessfulCreateMessageExtendedResult(serverObject, messageId);
		}
	}
}
