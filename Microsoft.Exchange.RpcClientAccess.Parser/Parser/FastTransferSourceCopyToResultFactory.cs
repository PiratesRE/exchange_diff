using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceCopyToResultFactory : StandardResultFactory
	{
		internal FastTransferSourceCopyToResultFactory() : base(RopId.FastTransferSourceCopyTo)
		{
		}

		public SuccessfulFastTransferSourceCopyToResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulFastTransferSourceCopyToResult(serverObject);
		}
	}
}
