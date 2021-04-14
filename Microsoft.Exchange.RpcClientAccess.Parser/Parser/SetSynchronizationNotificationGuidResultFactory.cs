using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetSynchronizationNotificationGuidResultFactory : StandardResultFactory
	{
		internal SetSynchronizationNotificationGuidResultFactory() : base(RopId.SetSynchronizationNotificationGuid)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetSynchronizationNotificationGuid, ErrorCode.None);
		}
	}
}
