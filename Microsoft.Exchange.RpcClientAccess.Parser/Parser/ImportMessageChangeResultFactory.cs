using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportMessageChangeResultFactory : StandardResultFactory
	{
		internal ImportMessageChangeResultFactory() : base(RopId.ImportMessageChange)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId messageId)
		{
			return new SuccessfulImportMessageChangeResult(serverObject, messageId);
		}
	}
}
