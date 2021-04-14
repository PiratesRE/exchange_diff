using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LongTermIdFromIdResultFactory : StandardResultFactory
	{
		internal LongTermIdFromIdResultFactory() : base(RopId.LongTermIdFromId)
		{
		}

		public RopResult CreateSuccessfulResult(StoreLongTermId longTermId)
		{
			return new SuccessfulLongTermIdFromIdResult(longTermId);
		}
	}
}
