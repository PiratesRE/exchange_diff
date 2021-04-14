using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.LiveIdAuth
{
	public sealed class LiveIdAuthAgentFactory : SmtpReceiveAgentFactory
	{
		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new LiveIdAuthAgent();
		}
	}
}
