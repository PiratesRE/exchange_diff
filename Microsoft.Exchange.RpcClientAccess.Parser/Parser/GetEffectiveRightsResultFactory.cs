using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetEffectiveRightsResultFactory : StandardResultFactory
	{
		internal GetEffectiveRightsResultFactory() : base(RopId.GetEffectiveRights)
		{
		}

		public RopResult CreateSuccessfulResult(Rights effectiveRights)
		{
			return new SuccessfulGetEffectiveRightsResult(effectiveRights);
		}
	}
}
