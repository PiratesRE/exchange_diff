using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.Prioritization
{
	public sealed class PrioritizationAgentFactory : RoutingAgentFactory
	{
		public PrioritizationAgentFactory()
		{
			if (PrioritizationAgentFactory.prioritizationEnabled)
			{
				PrioritizationAgentFactory.prioritization = new MessagePrioritization();
			}
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new PrioritizationAgent(PrioritizationAgentFactory.prioritization);
		}

		internal RoutingAgent CreateAgent(MessagePrioritization prioritization)
		{
			return new PrioritizationAgent(prioritization);
		}

		private static bool prioritizationEnabled = Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.PrioritizationEnabled;

		private static MessagePrioritization prioritization;
	}
}
