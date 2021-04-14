using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	public sealed class InboundTrustAgentFactory : SmtpReceiveAgentFactory
	{
		internal bool IsFrontEndTransport
		{
			get
			{
				return this.isFrontEndTransport;
			}
		}

		internal string ComputerName
		{
			get
			{
				return this.computerName;
			}
		}

		public InboundTrustAgentFactory()
		{
			this.computerName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
			this.isFrontEndTransport = (Components.Configuration.ProcessTransportRole == ProcessTransportRole.FrontEnd);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new InboundTrustAgent(server, this, TrustedMailUtils.TrustedMailAgentsEnabled);
		}

		private readonly bool isFrontEndTransport;

		private readonly string computerName;
	}
}
