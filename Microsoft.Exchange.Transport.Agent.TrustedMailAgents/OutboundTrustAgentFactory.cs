using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	public sealed class OutboundTrustAgentFactory : RoutingAgentFactory
	{
		internal string ComputerName
		{
			get
			{
				return this.computerName;
			}
		}

		public OutboundTrustAgentFactory()
		{
			this.computerName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new OutboundTrustAgent(this, server, TrustedMailUtils.TrustedMailAgentsEnabled);
		}

		private readonly string computerName;
	}
}
