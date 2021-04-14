using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderIsGhostedResultFactory : StandardResultFactory
	{
		internal PublicFolderIsGhostedResultFactory() : base(RopId.PublicFolderIsGhosted)
		{
		}

		public RopResult CreateSuccessfulResult(ReplicaServerInfo? replicaInfo)
		{
			return new SuccessfulPublicFolderIsGhostedResult(replicaInfo);
		}
	}
}
