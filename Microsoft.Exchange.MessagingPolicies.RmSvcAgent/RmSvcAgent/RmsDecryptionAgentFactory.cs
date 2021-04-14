using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class RmsDecryptionAgentFactory : RoutingAgentFactory
	{
		public RmsDecryptionAgentFactory()
		{
			RmsDecryptionAgentPerfCounters.MessageDecrypted.RawValue = 0L;
			RmsDecryptionAgentPerfCounters.MessageFailedToDecrypt.RawValue = 0L;
			AgentInstanceController.Initialize();
		}

		internal AgentInstanceController InstanceController
		{
			get
			{
				return AgentInstanceController.Instance;
			}
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new RmsDecryptionAgent(AgentInstanceController.Instance);
		}
	}
}
