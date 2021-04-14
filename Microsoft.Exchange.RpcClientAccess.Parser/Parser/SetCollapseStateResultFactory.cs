using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetCollapseStateResultFactory : StandardResultFactory
	{
		internal SetCollapseStateResultFactory() : base(RopId.SetCollapseState)
		{
		}

		public RopResult CreateSuccessfulResult(byte[] bookmark)
		{
			return new SuccessfulSetCollapseStateResult(bookmark);
		}
	}
}
