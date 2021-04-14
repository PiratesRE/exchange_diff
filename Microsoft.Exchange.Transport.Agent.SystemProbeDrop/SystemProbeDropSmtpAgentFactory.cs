using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.SystemProbeDrop
{
	public sealed class SystemProbeDropSmtpAgentFactory : SmtpReceiveAgentFactory
	{
		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new SystemProbeDropSmtpAgent();
		}
	}
}
