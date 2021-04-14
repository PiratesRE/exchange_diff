using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal enum MDBThrottleType
	{
		DynamicMDBThrottleDisabled,
		PendingConnections,
		ConnectionAcquireTimeout
	}
}
