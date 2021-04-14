using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.SystemProbeDrop
{
	public sealed class SystemProbeDropRoutingAgentFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new SystemProbeDropRoutingAgent();
		}
	}
}
