using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SpoolerRulesResultFactory : StandardResultFactory
	{
		internal SpoolerRulesResultFactory() : base(RopId.SpoolerRules)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId? folderId)
		{
			return new SuccessfulSpoolerRulesResult(folderId);
		}
	}
}
