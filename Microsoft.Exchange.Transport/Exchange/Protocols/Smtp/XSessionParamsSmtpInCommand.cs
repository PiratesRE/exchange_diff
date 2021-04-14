using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XSessionParamsSmtpInCommand : SmtpInCommandBase
	{
		public XSessionParamsSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			XSessionParams xsessionParams;
			ParseResult result = XSessionParamsSmtpCommandParser.Parse(commandContext, this.sessionState, out xsessionParams);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnXSessionParamsCommand";
				agentEventArgs = new XSessionParamsCommandEventArgs(this.sessionState, xsessionParams.MdbGuid, xsessionParams.Type);
			}
			return result;
		}

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			return XSessionParamsSmtpInCommand.XSessionParamsCompleteTask;
		}

		public static readonly Task<ParseAndProcessResult<SmtpInStateMachineEvents>> XSessionParamsCompleteTask = Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.XSessionParamsOk, SmtpInStateMachineEvents.XSessionParamsProcessed, false));
	}
}
