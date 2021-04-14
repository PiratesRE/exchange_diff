using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferGetIncrementalStateResultFactory : StandardResultFactory
	{
		internal FastTransferGetIncrementalStateResultFactory() : base(RopId.FastTransferGetIncrementalState)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject fastTransferDownloadObject)
		{
			return new SuccessfulFastTransferGetIncrementalStateResult(fastTransferDownloadObject);
		}
	}
}
