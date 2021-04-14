using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class QuitSmtpCommand : SmtpCommand
	{
		public QuitSmtpCommand(ISmtpSession session) : base(session, "QUIT", null, LatencyComponent.None)
		{
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.QuitInboundParseCommand);
			ParseResult parseResult = QuitSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession));
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.QuitInboundProcessCommand);
			base.SmtpResponse = SmtpResponse.Quit;
			smtpInSession.Disconnect(DisconnectReason.QuitVerb);
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "QUIT";
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command: {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Inititating Disconnect with remote host");
			smtpOutSession.Disconnect();
		}
	}
}
