using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueItem
	{
		DateTime DeferUntil { get; set; }

		DateTime Expiry { get; }

		DeliveryPriority Priority { get; set; }

		void Update();

		MessageContext GetMessageContext(MessageProcessingSource source);
	}
}
