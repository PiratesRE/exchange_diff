using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.MailboxRules;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class MailboxRulesAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public MailboxRulesAgentFactory()
		{
			try
			{
				RuleConfig.Load();
			}
			catch (TransportComponentLoadFailedException ex)
			{
				throw new ExchangeConfigurationException(ex.Message, ex.InnerException);
			}
		}

		public override void Close()
		{
			RuleConfig.UnLoad();
		}

		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new MailboxRulesAgent(server);
		}
	}
}
