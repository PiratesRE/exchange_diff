using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class QuitSmtpInCommand : SmtpInCommandBase
	{
		public QuitSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			agentEventTopic = null;
			agentEventArgs = null;
			return QuitSmtpCommandParser.Parse(commandContext, this.sessionState);
		}

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			this.sessionState.DisconnectReason = DisconnectReason.QuitVerb;
			return QuitSmtpInCommand.ProcessCompletedTask;
		}

		public static readonly Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessCompletedTask = Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Quit, SmtpInStateMachineEvents.QuitProcessed, false));
	}
}
