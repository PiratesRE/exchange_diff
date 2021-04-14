using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeekRowBookmarkResultFactory : StandardResultFactory
	{
		internal SeekRowBookmarkResultFactory() : base(RopId.SeekRowBookmark)
		{
		}

		public RopResult CreateSuccessfulResult(bool positionChanged, bool soughtLessThanRequested, int rowsSought)
		{
			return new SuccessfulSeekRowBookmarkResult(positionChanged, soughtLessThanRequested, rowsSought);
		}
	}
}
