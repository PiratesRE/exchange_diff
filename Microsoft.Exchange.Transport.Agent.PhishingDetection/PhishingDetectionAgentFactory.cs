using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.PhishingDetection
{
	public sealed class PhishingDetectionAgentFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new PhishingDetectionAgent(server);
		}
	}
}
