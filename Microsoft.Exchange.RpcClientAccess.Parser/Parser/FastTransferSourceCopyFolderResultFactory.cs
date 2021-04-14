using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceCopyFolderResultFactory : StandardResultFactory
	{
		internal FastTransferSourceCopyFolderResultFactory() : base(RopId.FastTransferSourceCopyFolder)
		{
		}

		public SuccessfulFastTransferSourceCopyFolderResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulFastTransferSourceCopyFolderResult(serverObject);
		}
	}
}
