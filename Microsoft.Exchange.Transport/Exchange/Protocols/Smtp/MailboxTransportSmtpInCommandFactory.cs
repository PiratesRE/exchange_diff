using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class MailboxTransportSmtpInCommandFactory : ISmtpInCommandFactory<SmtpInStateMachineEvents>
	{
		public MailboxTransportSmtpInCommandFactory(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("awaitCompletedDelegate", awaitCompletedDelegate);
			this.sessionState = sessionState;
			this.awaitCompletedDelegate = awaitCompletedDelegate;
		}

		public ISmtpInCommand<SmtpInStateMachineEvents> CreateCommand(SmtpInCommand commandType)
		{
			switch (commandType)
			{
			case SmtpInCommand.AUTH:
				return new AuthSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.BDAT:
				return new BdatSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.DATA:
				return new DataSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.EHLO:
				return new EhloSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.EXPN:
				return new ExpnSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.HELO:
				return new HeloSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.HELP:
				return new HelpSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.MAIL:
				return new MailSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.NOOP:
				return new NoopSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.QUIT:
				return new QuitSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.RCPT:
				return new RcptSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.RSET:
				return new RsetSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.STARTTLS:
				return new StartTlsSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.VRFY:
				return new VrfySmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.XANONYMOUSTLS:
				return new AnonymousTlsSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.XEXPS:
				return new XExpsSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			case SmtpInCommand.XSESSIONPARAMS:
				return new XSessionParamsSmtpInCommand(this.sessionState, this.awaitCompletedDelegate);
			}
			return null;
		}

		private readonly SmtpInSessionState sessionState;

		private readonly AwaitCompletedDelegate awaitCompletedDelegate;
	}
}
