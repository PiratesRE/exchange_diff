using System;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IDeliveryProcessor
	{
		void Initialize();

		DeliverableItem CreateSession();

		void CreateMessage(DeliverableItem item);

		void DeliverMessage();
	}
}
