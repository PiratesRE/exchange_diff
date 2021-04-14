using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class QuitSmtpProxyCommand : QuitSmtpCommand
	{
		public QuitSmtpProxyCommand(ISmtpSession session) : base(session)
		{
		}

		internal override void InboundParseCommand()
		{
			throw new NotImplementedException();
		}

		internal override void InboundProcessCommand()
		{
			throw new NotImplementedException();
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Inititating Disconnect with remote host");
			smtpOutProxySession.Disconnect();
		}
	}
}
