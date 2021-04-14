using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ResetTableResultFactory : StandardResultFactory
	{
		internal ResetTableResultFactory() : base(RopId.ResetTable)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.ResetTable, ErrorCode.None);
		}
	}
}
