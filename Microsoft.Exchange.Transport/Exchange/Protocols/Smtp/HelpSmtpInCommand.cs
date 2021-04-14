using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class HelpSmtpInCommand : SmtpInCommandBase
	{
		public HelpSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			string helpArg;
			ParseResult result = HelpSmtpCommandParser.Parse(commandContext, this.sessionState, out helpArg);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnHelpCommand";
				agentEventArgs = new HelpCommandEventArgs(this.sessionState, helpArg);
			}
			return result;
		}

		protected override async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			await this.TarpitAsync(this.sessionState.ReceiveConnector.TarpitInterval, cancellationToken);
			base.OnAwaitCompleted(cancellationToken);
			return HelpSmtpInCommand.HelpCompleteTask;
		}

		protected virtual Task TarpitAsync(EnhancedTimeSpan tarpitInterval, CancellationToken cancellationToken)
		{
			return Task.Delay(tarpitInterval, cancellationToken);
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> HelpCompleteTask = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Help, SmtpInStateMachineEvents.HelpProcessed, false);
	}
}
