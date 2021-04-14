using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal enum AccessCheckResult
	{
		Allowed,
		NotAllowedAnonymous,
		NotAllowedAuthenticated,
		NotAllowedInternalSystemError
	}
}
