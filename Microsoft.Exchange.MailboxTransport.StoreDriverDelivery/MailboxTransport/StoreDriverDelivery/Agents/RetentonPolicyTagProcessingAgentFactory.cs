using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class RetentonPolicyTagProcessingAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new RetentonPolicyTagProcessingAgent();
		}
	}
}
