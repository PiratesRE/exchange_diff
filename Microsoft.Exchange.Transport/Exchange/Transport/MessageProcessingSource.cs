using System;

namespace Microsoft.Exchange.Transport
{
	internal enum MessageProcessingSource
	{
		Unknown,
		SmtpReceive,
		Pickup,
		StoreDriverSubmit,
		Categorizer,
		Routing,
		SmtpSend,
		StoreDriverLocalDelivery,
		DsnGenerator,
		Agents,
		NonSmtpGateway,
		BootLoader,
		DeliveryAgent,
		Queue
	}
}
