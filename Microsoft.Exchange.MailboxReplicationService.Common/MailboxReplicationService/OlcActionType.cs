using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum OlcActionType
	{
		NonDeferred = -1,
		DeferredByAge,
		DeferredByCount
	}
}
