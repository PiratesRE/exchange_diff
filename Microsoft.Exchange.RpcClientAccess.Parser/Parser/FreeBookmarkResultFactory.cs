using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FreeBookmarkResultFactory : StandardResultFactory
	{
		internal FreeBookmarkResultFactory() : base(RopId.FreeBookmark)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.FreeBookmark, ErrorCode.None);
		}
	}
}
