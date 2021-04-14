using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	public sealed class DeliveryThrottlingAgentFactory : SmtpReceiveAgentFactory
	{
		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new DeliveryThrottlingAgent();
		}
	}
}
