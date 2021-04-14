using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.MessagingPolicies.Redirection
{
	public class RedirectionAgentFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new RedirectionAgent();
		}
	}
}
