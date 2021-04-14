using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetContentsTableResultFactory : StandardResultFactory
	{
		internal GetContentsTableResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetContentsTable)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject table, int rowCount)
		{
			return new SuccessfulGetContentsTableResult(table, rowCount);
		}
	}
}
