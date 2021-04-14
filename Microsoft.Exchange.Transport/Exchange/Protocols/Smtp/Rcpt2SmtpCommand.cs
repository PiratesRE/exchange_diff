using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class Rcpt2SmtpCommand : SmtpCommand
	{
		public Rcpt2SmtpCommand(ISmtpInSession smtpInSession) : base(smtpInSession, "RCPT2", "OnRcpt2Command", LatencyComponent.SmtpReceiveOnRcpt2Command)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			this.rcpt2CommandEventArgs = new Rcpt2CommandEventArgs(smtpInSession.SessionSource);
			this.rcpt2CommandEventArgs.MailItem = smtpInSession.TransportMailItemWrapper;
			this.CommandEventArgs = this.rcpt2CommandEventArgs;
			base.IsResponseBuffered = true;
		}

		internal Dictionary<string, string> ConsumerMailOptionalArguments
		{
			get
			{
				return this.rcpt2CommandEventArgs.ConsumerMailOptionalArguments;
			}
			set
			{
				this.rcpt2CommandEventArgs.ConsumerMailOptionalArguments = value;
			}
		}

		internal RoutingAddress RecipientAddress
		{
			get
			{
				return this.rcpt2CommandEventArgs.RecipientAddress;
			}
			set
			{
				this.rcpt2CommandEventArgs.RecipientAddress = value;
			}
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.Rcpt2InboundParseCommand);
			if (!SmtpInSessionUtils.ShouldAllowConsumerMail(smtpInSession.Capabilities))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.Rcpt2NotAuthorized);
				base.SmtpResponse = SmtpResponse.NotAuthorized;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (this.VerifyRcpt2ToAlreadyReceived())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.Rcpt2AlreadyReceived);
				return;
			}
			if (!base.VerifyRcptToReceived())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			CommandContext commandContext = CommandContext.FromSmtpCommand(this);
			Rcpt2ParseOutput rcpt2ParseOutput;
			ParseResult parseResult;
			using (SmtpInSessionState smtpInSessionState = SmtpInSessionState.FromSmtpInSession(smtpInSession))
			{
				parseResult = Rcpt2SmtpCommandParser.Parse(commandContext, smtpInSessionState, smtpInSession.TransportMailItemWrapper.Recipients[0].Address, smtpInSession.IsDataRedactionNecessary, out rcpt2ParseOutput);
			}
			switch (parseResult.ParsingStatus)
			{
			case ParsingStatus.Complete:
				this.RecipientAddress = rcpt2ParseOutput.RecipientAddress;
				this.ConsumerMailOptionalArguments = rcpt2ParseOutput.ConsumerMailOptionalArguments;
				base.CurrentOffset = commandContext.Offset;
				smtpInSession.SeenRcpt2 = true;
				break;
			case ParsingStatus.IgnorableProtocolError:
				base.SmtpResponse = parseResult.SmtpResponse;
				smtpInSession.SeenRcpt2 = true;
				break;
			default:
				base.SmtpResponse = parseResult.SmtpResponse;
				break;
			}
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			base.SmtpResponse = SmtpResponse.Rcpt2ToOk;
		}

		internal override void OutboundCreateCommand()
		{
			throw new NotImplementedException();
		}

		internal override void OutboundFormatCommand()
		{
			throw new NotImplementedException();
		}

		internal override void OutboundProcessResponse()
		{
			throw new NotImplementedException();
		}

		private bool VerifyRcpt2ToAlreadyReceived()
		{
			ISmtpInSession smtpInSession = base.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.TransportMailItem != null && smtpInSession.SeenRcpt2)
			{
				base.SmtpResponse = SmtpResponse.Rcpt2AlreadyReceived;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return true;
			}
			return false;
		}

		private readonly Rcpt2CommandEventArgs rcpt2CommandEventArgs;
	}
}
