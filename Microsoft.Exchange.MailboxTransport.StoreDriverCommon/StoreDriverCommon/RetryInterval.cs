using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal enum RetryInterval
	{
		None,
		FastRetry,
		QuarantinedRetry
	}
}
