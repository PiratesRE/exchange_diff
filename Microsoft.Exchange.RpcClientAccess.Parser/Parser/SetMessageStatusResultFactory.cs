using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetMessageStatusResultFactory : StandardResultFactory
	{
		internal SetMessageStatusResultFactory() : base(RopId.SetMessageStatus)
		{
		}

		public RopResult CreateSuccessfulResult(MessageStatusFlags oldStatus)
		{
			return new SuccessfulSetMessageStatusResult(oldStatus);
		}
	}
}
