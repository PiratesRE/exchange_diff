using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetReceiveFolderResultFactory : StandardResultFactory
	{
		internal GetReceiveFolderResultFactory() : base(RopId.GetReceiveFolder)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId receiveFolderId, string messageClass)
		{
			return new SuccessfulGetReceiveFolderResult(receiveFolderId, messageClass);
		}
	}
}
