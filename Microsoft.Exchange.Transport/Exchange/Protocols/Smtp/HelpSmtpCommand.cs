using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class HelpSmtpCommand : SmtpCommand
	{
		public HelpSmtpCommand(ISmtpSession session) : base(session, "HELP", "OnHelpCommand", LatencyComponent.None)
		{
			this.helpCommandEventArgs = new HelpCommandEventArgs();
			this.CommandEventArgs = this.helpCommandEventArgs;
		}

		internal string HelpArg
		{
			get
			{
				return this.helpCommandEventArgs.HelpArgument;
			}
			set
			{
				this.helpCommandEventArgs.HelpArgument = value;
			}
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!base.VerifyNoOngoingBdat())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			string helpArg;
			ParseResult parseResult = HelpSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession), out helpArg);
			if (!parseResult.IsFailed)
			{
				this.HelpArg = helpArg;
			}
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
			base.SmtpResponse = SmtpResponse.Help;
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

		private readonly HelpCommandEventArgs helpCommandEventArgs;
	}
}
