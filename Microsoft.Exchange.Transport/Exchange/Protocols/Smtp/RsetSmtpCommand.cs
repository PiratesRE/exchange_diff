using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RsetSmtpCommand : SmtpCommand
	{
		public RsetSmtpCommand(ISmtpSession session) : base(session, "RSET", "OnRsetCommand", LatencyComponent.None)
		{
			base.IsResponseBuffered = true;
			this.CommandEventArgs = new RsetCommandEventArgs();
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RsetInboundParseCommand);
			ParseResult parseResult = RsetSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession));
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RsetInboundProcessCommand);
			if (smtpInSession.TarpitRset)
			{
				base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
			}
			smtpInSession.TarpitRset = true;
			smtpInSession.AbortMailTransaction();
			base.SmtpResponse = SmtpResponse.Reset;
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "RSET";
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (!smtpOutSession.BetweenMessagesRset)
			{
				throw new InvalidOperationException("Error, unexpected call to RSET");
			}
			smtpOutSession.BetweenMessagesRset = false;
			if (smtpOutSession.RoutedMailItem == null)
			{
				throw new InvalidOperationException("Must call PrepareForNextMessage when issuing RSET between messages");
			}
			smtpOutSession.NextState = SmtpOutSession.SessionState.MessageStart;
			ExTraceGlobals.ServiceTracer.TraceDebug((long)this.GetHashCode(), "Setting Next State: MessageStart");
		}
	}
}
