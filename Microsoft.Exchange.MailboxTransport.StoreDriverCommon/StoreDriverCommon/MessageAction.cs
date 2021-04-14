using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
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
