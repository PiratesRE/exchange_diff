using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CollapseRowResultFactory : StandardResultFactory
	{
		internal CollapseRowResultFactory() : base(RopId.CollapseRow)
		{
		}

		public RopResult CreateSuccessfulResult(int collapsedRowCount)
		{
			return new SuccessfulCollapseRowResult(collapsedRowCount);
		}
	}
}
