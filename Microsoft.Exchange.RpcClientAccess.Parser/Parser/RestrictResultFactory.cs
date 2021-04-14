using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RestrictResultFactory : StandardResultFactory
	{
		internal RestrictResultFactory() : base(RopId.Restrict)
		{
		}

		public RopResult CreateSuccessfulResult(TableStatus tableStatus)
		{
			return new SuccessfulRestrictResult(tableStatus);
		}
	}
}
