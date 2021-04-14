using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class HeloSmtpInCommandBase : SmtpInCommandBase
	{
		protected HeloSmtpInCommandBase(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = HeloSmtpCommandParser.Parse(commandContext, this.sessionState, this.Command, out this.parseOutput);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				this.OnParseComplete(out agentEventTopic, out agentEventArgs);
			}
			return result;
		}

		protected abstract void OnParseComplete(out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs);

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			if (this.sessionState.SecureState == SecureState.StartTls)
			{
				this.sessionState.TlsDomainCapabilities = new SmtpReceiveCapabilities?(this.parseOutput.TlsDomainCapabilities);
				this.sessionState.AddSessionPermissions(this.sessionState.Capabilities);
			}
			this.sessionState.AbortMailTransaction();
			return this.ProcessAsyncInternal(cancellationToken);
		}

		protected abstract Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsyncInternal(CancellationToken cancellationToken);

		protected abstract HeloOrEhlo Command { get; }

		protected HeloEhloParseOutput parseOutput;
	}
}
