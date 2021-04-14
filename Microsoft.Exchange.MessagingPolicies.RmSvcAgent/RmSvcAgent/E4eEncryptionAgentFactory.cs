using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class E4eEncryptionAgentFactory : RoutingAgentFactory
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
			return new E4eEncryptionAgent(this);
		}

		public E4eEncryptionAgentFactory()
		{
			E4eAgentPerfCounters.EncryptionSuccessCount.RawValue = 0L;
			E4eAgentPerfCounters.AfterEncryptionSuccessCount.RawValue = 0L;
			E4eAgentPerfCounters.ReEncryptionSuccessCount.RawValue = 0L;
			E4eAgentPerfCounters.EncryptionFailureCount.RawValue = 0L;
			E4eAgentPerfCounters.AfterEncryptionFailureCount.RawValue = 0L;
			E4eAgentPerfCounters.ReEncryptionFailureCount.RawValue = 0L;
			AgentInstanceController.Initialize();
		}
	}
}
