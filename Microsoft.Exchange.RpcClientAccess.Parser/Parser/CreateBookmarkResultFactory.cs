using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateBookmarkResultFactory : StandardResultFactory
	{
		internal CreateBookmarkResultFactory() : base(RopId.CreateBookmark)
		{
		}

		public RopResult CreateSuccessfulResult(byte[] bookmark)
		{
			return new SuccessfulCreateBookmarkResult(bookmark);
		}
	}
}
