using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubmitMessageResultFactory : StandardResultFactory
	{
		internal SubmitMessageResultFactory() : base(RopId.SubmitMessage)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SubmitMessage, ErrorCode.None);
		}
	}
}
