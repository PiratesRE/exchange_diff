using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RcptSmtpInCommand : SmtpInCommandBase
	{
		public RcptSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyComponent.SmtpReceiveOnRcptCommand;
			}
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = RcptSmtpCommandParser.Parse(commandContext, this.sessionState, this.sessionState.ServerState.IsDataRedactionNecessary, this.sessionState.Configuration, this.sessionState.Configuration.TransportConfiguration.SmtpAcceptAnyRecipient, out this.rcptParseOutput);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnRcptCommand";
				agentEventArgs = new RcptCommandEventArgs(this.sessionState)
				{
					MailItem = this.sessionState.TransportMailItemWrapper,
					Notify = EnumConverter.InternalToPublic(this.rcptParseOutput.Notify),
					OriginalRecipient = this.rcptParseOutput.ORcpt,
					RecipientAddress = this.rcptParseOutput.RecipientAddress
				};
			}
			return result;
		}

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			if (!this.sessionState.RecipientCorrelator.Contains(this.rcptParseOutput.RecipientAddress.ToString()))
			{
				this.mailRecipient = this.sessionState.TransportMailItem.Recipients.Add(this.rcptParseOutput.RecipientAddress.ToString());
				this.mailRecipient.ORcpt = this.rcptParseOutput.ORcpt;
				this.mailRecipient.DsnRequested = this.rcptParseOutput.Notify;
				if (this.rcptParseOutput.Orar != RoutingAddress.Empty)
				{
					OrarGenerator.SetOrarAddress(this.mailRecipient, this.rcptParseOutput.Orar);
				}
				if (this.rcptParseOutput.RDst != null)
				{
					this.mailRecipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.RoutingOverride", this.rcptParseOutput.RDst);
				}
				if (SmtpInSessionUtils.IsPeerShadowSession(this.sessionState.PeerSessionPrimaryServer))
				{
					ShadowRedundancyManager.PrepareRecipientForShadowing(this.mailRecipient, this.sessionState.PeerSessionPrimaryServer);
				}
				this.sessionState.RecipientCorrelator.Add(this.mailRecipient);
			}
			else if (this.rcptParseOutput.Orar != RoutingAddress.Empty)
			{
				MailRecipient recipient = this.sessionState.RecipientCorrelator.Find(this.rcptParseOutput.RecipientAddress);
				if (!OrarGenerator.ContainsOrar(recipient))
				{
					OrarGenerator.SetOrarAddress(recipient, this.rcptParseOutput.Orar);
				}
			}
			return RcptSmtpInCommand.RcptCompleteTask;
		}

		protected override void LogCommandReceived(CommandContext command)
		{
		}

		public static readonly Task<ParseAndProcessResult<SmtpInStateMachineEvents>> RcptCompleteTask = Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.RcptToOk, SmtpInStateMachineEvents.RcptProcessed, false));

		private RcptParseOutput rcptParseOutput;

		protected MailRecipient mailRecipient;
	}
}
