using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class GroupEscalationAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new GroupEscalationAgent(this.processedMessages);
		}

		public ProcessedMessageTracker ProcessedMessages
		{
			get
			{
				return this.processedMessages;
			}
		}

		private ProcessedMessageTracker processedMessages = new ProcessedMessageTracker();
	}
}
