using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	public enum BrokerStatus
	{
		None = -1,
		Success,
		UnknownError,
		Cancelled
	}
}
