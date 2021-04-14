using System;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal enum MessageThrottlingReason
	{
		NotThrottled,
		IPAddressLimitExceeded,
		UserLimitExceeded
	}
}
