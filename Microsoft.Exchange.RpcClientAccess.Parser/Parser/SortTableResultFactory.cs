using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SortTableResultFactory : StandardResultFactory
	{
		internal SortTableResultFactory() : base(RopId.SortTable)
		{
		}

		public RopResult CreateSuccessfulResult(TableStatus tableStatus)
		{
			return new SuccessfulSortTableResult(tableStatus);
		}
	}
}
