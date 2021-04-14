using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportMessageMoveResultFactory : StandardResultFactory
	{
		internal ImportMessageMoveResultFactory() : base(RopId.ImportMessageMove)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId messageId)
		{
			return new SuccessfulImportMessageMoveResult(messageId);
		}
	}
}
