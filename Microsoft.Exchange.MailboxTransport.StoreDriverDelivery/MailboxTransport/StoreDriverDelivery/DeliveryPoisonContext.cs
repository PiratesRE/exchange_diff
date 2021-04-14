using System;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryPoisonContext : MessageContext
	{
		public DeliveryPoisonContext(string internetMessageId) : base(0L, internetMessageId, MessageProcessingSource.StoreDriverLocalDelivery)
		{
		}

		public override string ToString()
		{
			return base.InternetMessageId;
		}
	}
}
