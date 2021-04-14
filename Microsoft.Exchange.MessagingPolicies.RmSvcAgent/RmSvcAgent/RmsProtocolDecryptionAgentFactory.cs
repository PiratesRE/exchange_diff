using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class RmsProtocolDecryptionAgentFactory : SmtpReceiveAgentFactory
	{
		public RmsProtocolDecryptionAgentFactory()
		{
			RmsDecryptionAgentPerfCounters.MessageDecrypted.RawValue = 0L;
			RmsDecryptionAgentPerfCounters.MessageFailedToDecrypt.RawValue = 0L;
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new RmsProtocolDecryptionAgent();
		}
	}
}
