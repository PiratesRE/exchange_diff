using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryProcessorFactory
	{
		public static IDeliveryProcessor Create(MailItemDeliver mailItemDeliver)
		{
			if (DeliveryProcessorFactory.InstanceBuilder != null)
			{
				return DeliveryProcessorFactory.InstanceBuilder();
			}
			bool value = mailItemDeliver.Recipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", false);
			if (value)
			{
				return new RetryAgentMessageSubmissionProcessor(mailItemDeliver);
			}
			return new DeliveryProcessorBase(mailItemDeliver);
		}

		public static DeliveryProcessorFactory.DeliverProcessorBuilder InstanceBuilder;

		public delegate IDeliveryProcessor DeliverProcessorBuilder();
	}
}
