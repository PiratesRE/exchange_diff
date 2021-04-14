using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetAttachmentTableResultFactory : StandardResultFactory
	{
		internal GetAttachmentTableResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.GetAttachmentTable)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulGetAttachmentTableResult(serverObject);
		}
	}
}
