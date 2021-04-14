using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetMessageFlagsResultFactory : StandardResultFactory
	{
		internal SetMessageFlagsResultFactory() : base(RopId.SetMessageFlags)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetMessageFlags, ErrorCode.None);
		}
	}
}
