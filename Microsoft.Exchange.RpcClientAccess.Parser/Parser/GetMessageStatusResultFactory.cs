using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetMessageStatusResultFactory : StandardResultFactory
	{
		internal GetMessageStatusResultFactory() : base(RopId.SetMessageStatus)
		{
		}

		public RopResult CreateSuccessfulResult(MessageStatusFlags messageStatus)
		{
			return new SuccessfulGetMessageStatusResult(messageStatus);
		}
	}
}
