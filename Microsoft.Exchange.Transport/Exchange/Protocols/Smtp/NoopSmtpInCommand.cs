using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class NoopSmtpInCommand : SmtpInCommandBase
	{
		public NoopSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = NoopSmtpCommandParser.Parse(commandContext, this.sessionState);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnNoopCommand";
				agentEventArgs = new NoopCommandEventArgs(this.sessionState);
			}
			return result;
		}

		protected override async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			await this.TarpitAsync(this.sessionState.ReceiveConnector.TarpitInterval, cancellationToken);
			base.OnAwaitCompleted(cancellationToken);
			return NoopSmtpInCommand.ProcessCompletedTask;
		}

		protected virtual Task TarpitAsync(EnhancedTimeSpan tarpitInterval, CancellationToken cancellationToken)
		{
			return Task.Delay(tarpitInterval, cancellationToken);
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> ProcessCompletedTask = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.NoopOk, SmtpInStateMachineEvents.NoopProcessed, false);
	}
}
