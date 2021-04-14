using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal enum MessageAction
	{
		Success,
		Retry,
		RetryQueue,
		NDR,
		Reroute,
		Throw,
		LogDuplicate,
		LogProcess,
		Skip,
		RetryMailboxServer
	}
}
