using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RegisterNotificationResultFactory : StandardResultFactory
	{
		internal RegisterNotificationResultFactory(ServerObjectHandle serverObjectHandle) : base(RopId.RegisterNotification)
		{
			this.ServerObjectHandle = serverObjectHandle;
		}

		public ServerObjectHandle ServerObjectHandle { get; private set; }

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulRegisterNotificationResult(serverObject);
		}
	}
}
