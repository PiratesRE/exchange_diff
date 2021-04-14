using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.SystemProbeDrop
{
	internal sealed class SystemProbeDropSmtpAgent : SmtpReceiveAgent
	{
		public SystemProbeDropSmtpAgent()
		{
			base.OnEndOfHeaders += this.OnEndOfHeadersHandler;
		}

		public void OnEndOfHeadersHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs args)
		{
			if (SystemProbeDropHelper.IsAgentEnabled() && args.Headers != null && SystemProbeDropHelper.ShouldDropMessage(args.Headers, "OnEndOfHeaders"))
			{
				SystemProbeDropHelper.DiscardMessage(args.MailItem);
			}
		}
	}
}
