using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	public sealed class FactoryInbound : SmtpReceiveAgentFactory
	{
		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new AgentInbound(server);
		}
	}
}
