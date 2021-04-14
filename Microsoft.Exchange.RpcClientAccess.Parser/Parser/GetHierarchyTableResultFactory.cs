using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetHierarchyTableResultFactory : StandardResultFactory
	{
		internal GetHierarchyTableResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetHierarchyTable)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject serverObject, int rowCount)
		{
			return new SuccessfulGetHierarchyTableResult(serverObject, rowCount);
		}
	}
}
