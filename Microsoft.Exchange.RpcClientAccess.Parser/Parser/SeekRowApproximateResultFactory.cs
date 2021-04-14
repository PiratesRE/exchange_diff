using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeekRowApproximateResultFactory : StandardResultFactory
	{
		internal SeekRowApproximateResultFactory() : base(RopId.SeekRowApproximate)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SeekRowApproximate, ErrorCode.None);
		}
	}
}
