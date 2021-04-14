using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenFolderResultFactory : StandardResultFactory
	{
		internal OpenFolderResultFactory() : base(RopId.OpenFolder)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject folder, bool hasRules, ReplicaServerInfo? replicaInfo)
		{
			return new SuccessfulOpenFolderResult(folder, hasRules, replicaInfo);
		}
	}
}
