using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPermissionsTableResultFactory : StandardResultFactory
	{
		internal GetPermissionsTableResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetPermissionsTable)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulGetPermissionsTableResult(serverObject);
		}
	}
}
