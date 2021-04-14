using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportMessageChangePartialResultFactory : StandardResultFactory
	{
		internal ImportMessageChangePartialResultFactory() : base(RopId.ImportMessageChangePartial)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId messageId)
		{
			return new SuccessfulImportMessageChangePartialResult(serverObject, messageId);
		}
	}
}
