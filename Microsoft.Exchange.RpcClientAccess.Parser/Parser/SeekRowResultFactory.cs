using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeekRowResultFactory : StandardResultFactory
	{
		internal SeekRowResultFactory() : base(RopId.SeekRow)
		{
		}

		public RopResult CreateSuccessfulResult(bool soughtLessThanRequested, int rowsSought)
		{
			return new SuccessfulSeekRowResult(soughtLessThanRequested, rowsSought);
		}
	}
}
