using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AbortSubmitResultFactory : StandardResultFactory
	{
		internal AbortSubmitResultFactory() : base(RopId.AbortSubmit)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.AbortSubmit, ErrorCode.None);
		}
	}
}
