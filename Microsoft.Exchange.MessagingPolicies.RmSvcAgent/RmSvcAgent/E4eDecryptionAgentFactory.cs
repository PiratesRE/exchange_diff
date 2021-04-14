using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class E4eDecryptionAgentFactory : RoutingAgentFactory
	{
		internal AgentInstanceController InstanceController
		{
			get
			{
				return AgentInstanceController.Instance;
			}
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new E4eDecryptionAgent(this);
		}

		public E4eDecryptionAgentFactory()
		{
			E4eAgentPerfCounters.DecryptionSuccessCount.RawValue = 0L;
			E4eAgentPerfCounters.DecryptionFailureCount.RawValue = 0L;
			AgentInstanceController.Initialize();
		}
	}
}
