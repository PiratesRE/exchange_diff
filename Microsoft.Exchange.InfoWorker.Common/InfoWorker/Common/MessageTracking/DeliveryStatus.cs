using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	public enum DeliveryStatus
	{
		Unsuccessful,
		Pending,
		Delivered,
		Transferred,
		Read
	}
}
