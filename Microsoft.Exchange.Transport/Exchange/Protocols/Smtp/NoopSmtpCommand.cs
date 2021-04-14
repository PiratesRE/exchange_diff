using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class NoopSmtpCommand : SmtpCommand
	{
		public NoopSmtpCommand(ISmtpSession session) : base(session, "NOOP", "OnNoopCommand", LatencyComponent.None)
		{
			this.CommandEventArgs = new NoopCommandEventArgs();
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession session = (ISmtpInSession)base.SmtpSession;
			ParseResult parseResult = NoopSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(session));
			base.ParsingStatus = parseResult.ParsingStatus;
			base.SmtpResponse = parseResult.SmtpResponse;
		}

		internal override void InboundProcessCommand()
		{
			base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
			base.SmtpResponse = SmtpResponse.NoopOk;
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
		}

		internal override void OutboundProcessResponse()
		{
		}
	}
}
