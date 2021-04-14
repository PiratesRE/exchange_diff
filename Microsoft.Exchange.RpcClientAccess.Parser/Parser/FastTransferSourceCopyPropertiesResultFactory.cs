using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceCopyPropertiesResultFactory : StandardResultFactory
	{
		internal FastTransferSourceCopyPropertiesResultFactory() : base(RopId.FastTransferSourceCopyProperties)
		{
		}

		public SuccessfulFastTransferSourceCopyPropertiesResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulFastTransferSourceCopyPropertiesResult(serverObject);
		}
	}
}
