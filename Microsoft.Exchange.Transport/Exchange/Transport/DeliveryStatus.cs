using System;

namespace Microsoft.Exchange.Transport
{
	internal enum DeliveryStatus : byte
	{
		Enqueued,
		InDelivery,
		DequeuedAndDeferred,
		PendingResubmit,
		Complete
	}
}
