using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PrereadMessagesResultFactory : StandardResultFactory
	{
		internal PrereadMessagesResultFactory() : base(RopId.PrereadMessages)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.PrereadMessages, ErrorCode.None);
		}
	}
}
