using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetLocalReplicationIdsResultFactory : StandardResultFactory
	{
		internal GetLocalReplicationIdsResultFactory() : base(RopId.GetLocalReplicationIds)
		{
		}

		public RopResult CreateSuccessfulResult(StoreLongTermId localReplicationId)
		{
			return new SuccessfulGetLocalReplicationIdsResult(localReplicationId);
		}
	}
}
