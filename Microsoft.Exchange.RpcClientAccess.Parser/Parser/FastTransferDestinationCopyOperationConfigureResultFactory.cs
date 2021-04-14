using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferDestinationCopyOperationConfigureResultFactory : StandardResultFactory
	{
		internal FastTransferDestinationCopyOperationConfigureResultFactory() : base(RopId.FastTransferDestinationCopyOperationConfigure)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulFastTransferDestinationCopyOperationConfigureResult(serverObject);
		}
	}
}
