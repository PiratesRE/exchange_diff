using System;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	public class TextMessagingDeliveryAgent : DeliveryAgent
	{
		internal TextMessagingDeliveryAgent(MobileSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.session = session;
			base.OnOpenConnection += this.TextMessagingOpenConnectionEventHandler;
			base.OnDeliverMailItem += this.TextMessagingDeliverMailItemEventHandler;
		}

		internal static void TextMessagingDeliverMailItemEventCompletionHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			e.Item.AgentWrapper.CompleteAsyncEvent();
		}

		public void TextMessagingOpenConnectionEventHandler(OpenConnectionEventSource source, OpenConnectionEventArgs e)
		{
			source.RegisterConnection("text.messaging.delivery.external", new SmtpResponse("250", string.Empty, new string[]
			{
				"OK"
			}));
		}

		public void TextMessagingDeliverMailItemEventHandler(DeliverMailItemEventSource source, DeliverMailItemEventArgs e)
		{
			TransportAgentWrapper agentWrapper = new TransportAgentWrapper(base.GetAgentAsyncContext(), source, e);
			this.session.Send(agentWrapper, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(TextMessagingDeliveryAgent.TextMessagingDeliverMailItemEventCompletionHandler));
		}

		private MobileSession session;
	}
}
