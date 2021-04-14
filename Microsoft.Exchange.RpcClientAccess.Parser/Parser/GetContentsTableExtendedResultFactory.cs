using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetContentsTableExtendedResultFactory : StandardResultFactory
	{
		internal GetContentsTableExtendedResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetContentsTableExtended)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject table, int rowCount)
		{
			return new SuccessfulGetContentsTableExtendedResult(table, rowCount);
		}
	}
}
