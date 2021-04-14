using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AbortResultFactory : StandardResultFactory
	{
		internal AbortResultFactory() : base(RopId.Abort)
		{
		}

		public RopResult CreateSuccessfulResult(TableStatus status)
		{
			return new SuccessfulAbortResult(status);
		}
	}
}
