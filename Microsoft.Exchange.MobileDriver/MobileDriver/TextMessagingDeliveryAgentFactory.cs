using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	public class TextMessagingDeliveryAgentFactory : DeliveryAgentFactory<TextMessagingDeliveryAgentManager>
	{
		public TextMessagingDeliveryAgentFactory()
		{
			this.session = new MobileSession();
		}

		public override DeliveryAgent CreateAgent(SmtpServer server)
		{
			return new TextMessagingDeliveryAgent(this.session);
		}

		private MobileSession session;
	}
}
