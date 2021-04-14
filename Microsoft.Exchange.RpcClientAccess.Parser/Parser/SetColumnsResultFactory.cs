using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetColumnsResultFactory : StandardResultFactory
	{
		internal SetColumnsResultFactory() : base(RopId.SetColumns)
		{
		}

		public RopResult CreateSuccessfulResult(TableStatus tableStatus)
		{
			return new SuccessfulSetColumnsResult(tableStatus);
		}
	}
}
