using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetLocalReplicaMidsetDeletedResultFactory : StandardResultFactory
	{
		internal SetLocalReplicaMidsetDeletedResultFactory() : base(RopId.SetLocalReplicaMidsetDeleted)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetLocalReplicaMidsetDeleted, ErrorCode.None);
		}
	}
}
