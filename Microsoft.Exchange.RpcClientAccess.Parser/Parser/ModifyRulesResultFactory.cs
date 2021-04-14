using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ModifyRulesResultFactory : StandardResultFactory
	{
		internal ModifyRulesResultFactory() : base(RopId.ModifyRules)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.ModifyRules, ErrorCode.None);
		}
	}
}
