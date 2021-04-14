using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetCollapseStateResultFactory : StandardResultFactory
	{
		internal GetCollapseStateResultFactory() : base(RopId.GetCollapseState)
		{
		}

		public RopResult CreateSuccessfulResult(byte[] collapseState)
		{
			return new SuccessfulGetCollapseStateResult(collapseState);
		}
	}
}
