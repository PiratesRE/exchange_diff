using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RegisterSynchronizationNotificationsResultFactory : StandardResultFactory
	{
		internal RegisterSynchronizationNotificationsResultFactory() : base(RopId.RegisterSynchronizationNotifications)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.RegisterSynchronizationNotifications, ErrorCode.None);
		}
	}
}
