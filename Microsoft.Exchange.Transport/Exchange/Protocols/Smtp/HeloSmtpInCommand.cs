using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class HeloSmtpInCommand : HeloSmtpInCommandBase
	{
		public HeloSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override void OnParseComplete(out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			HeloCommandEventArgs heloCommandEventArgs = new HeloCommandEventArgs(this.sessionState);
			if (!string.IsNullOrEmpty(this.parseOutput.HeloDomain))
			{
				this.sessionState.HelloDomain = this.parseOutput.HeloDomain;
				heloCommandEventArgs.Domain = this.parseOutput.HeloDomain;
			}
			agentEventTopic = "OnHeloCommand";
			agentEventArgs = heloCommandEventArgs;
		}

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsyncInternal(CancellationToken cancellationToken)
		{
			return Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Helo(this.sessionState.AdvertisedEhloOptions.AdvertisedFQDN, this.networkConnection.RemoteEndPoint.Address), SmtpInStateMachineEvents.HeloProcessed, false));
		}

		protected override HeloOrEhlo Command
		{
			get
			{
				return HeloOrEhlo.Helo;
			}
		}
	}
}
