using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetRulesTableResultFactory : StandardResultFactory
	{
		internal GetRulesTableResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetRulesTable)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulGetRulesTableResult(serverObject);
		}
	}
}
