using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetOwningServersResultFactory : StandardResultFactory
	{
		internal GetOwningServersResultFactory() : base(RopId.GetOwningServers)
		{
		}

		public RopResult CreateSuccessfulResult(ReplicaServerInfo replicaInfo)
		{
			return new SuccessfulGetOwningServersResult(replicaInfo);
		}
	}
}
