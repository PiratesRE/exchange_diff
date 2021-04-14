using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetStatusResultFactory : StandardResultFactory
	{
		internal GetStatusResultFactory() : base(RopId.GetStatus)
		{
		}

		public RopResult CreateSuccessfulResult(TableStatus tableStatus)
		{
			return new SuccessfulGetStatusResult(tableStatus);
		}
	}
}
