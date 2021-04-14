using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetSizeStreamResultFactory : StandardResultFactory
	{
		internal SetSizeStreamResultFactory() : base(RopId.SetSizeStream)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetSizeStream, ErrorCode.None);
		}
	}
}
