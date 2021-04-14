using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateFolderResultFactory : StandardResultFactory
	{
		internal CreateFolderResultFactory() : base(RopId.CreateFolder)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId folderId, bool existed, bool hasRules, ReplicaServerInfo? replicaInfo)
		{
			return new SuccessfulCreateFolderResult(serverObject, folderId, existed, hasRules, replicaInfo);
		}
	}
}
