using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class TlsSmtpInCommandBase : SmtpInCommandBase
	{
		protected TlsSmtpInCommandBase(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = StartTlsSmtpCommandParser.Parse(commandContext, this.sessionState, this.Command);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnStartTlsCommand";
				agentEventArgs = new StartTlsCommandEventArgs(this.sessionState);
			}
			return result;
		}

		protected override async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			ParseAndProcessResult<SmtpInStateMachineEvents> result;
			if (this.ShouldThrottleConnection)
			{
				this.sessionState.ReceivePerfCounters.TlsConnectionsRejectedDueToRateExceeded.Increment();
				result = TlsSmtpInCommandBase.TlsConnectionThrottledResult;
			}
			else
			{
				this.sessionState.AbortMailTransaction();
				object error = await base.WriteToClientAsync(SmtpResponse.StartTlsReadyToNegotiate);
				base.OnAwaitCompleted(cancellationToken);
				if (error != null)
				{
					this.sessionState.HandleNetworkError(error);
					result = TlsSmtpInCommandBase.TlsNegotiationFailureResult;
				}
				else
				{
					this.sessionState.SecureState = this.Command;
					this.sessionState.ProtocolLogSession.LogCertificate("Sending certificate", this.LocalCertificate);
					error = await this.networkConnection.NegotiateTlsAsServerAsync(this.LocalCertificate, this.ShouldRequestClientCertificate);
					base.OnAwaitCompleted(cancellationToken);
					if (error != null)
					{
						this.sessionState.ReceivePerfCounters.TlsNegotiationsFailed.Increment();
						this.sessionState.HandleNetworkError(error);
						this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "TLS negotiation failed with error " + error);
						result = TlsSmtpInCommandBase.TlsNegotiationFailureResult;
					}
					else
					{
						ConnectionInfo tlsConnectionInfo = this.networkConnection.TlsConnectionInfo;
						Util.LogTlsSuccessResult(this.sessionState.ProtocolLogSession, tlsConnectionInfo, this.sessionState.NetworkConnection.RemoteCertificate);
						if (this.networkConnection.RemoteCertificate != null)
						{
							this.sessionState.TlsRemoteCertificateInternal = this.networkConnection.RemoteCertificate;
							this.sessionState.ProtocolLogSession.LogCertificateThumbprint("Received certificate", this.networkConnection.RemoteCertificate);
							this.OnClientCertificateReceived(this.networkConnection.RemoteCertificate);
						}
						if (this.networkConnection.IsTls)
						{
							this.sessionState.ReceivePerfCounters.TlsConnectionsCurrent.Increment();
						}
						this.sessionState.AdvertisedEhloOptions.StartTLS = false;
						this.sessionState.AdvertisedEhloOptions.AnonymousTLS = false;
						result = this.CommandCompletedResult;
					}
				}
			}
			return result;
		}

		protected abstract SecureState Command { get; }

		protected abstract IX509Certificate2 LocalCertificate { get; }

		protected abstract bool ShouldRequestClientCertificate { get; }

		protected abstract ParseAndProcessResult<SmtpInStateMachineEvents> CommandCompletedResult { get; }

		protected virtual void OnClientCertificateReceived(IX509Certificate2 remoteCertificate)
		{
		}

		protected virtual bool ShouldThrottleConnection
		{
			get
			{
				return SmtpInSessionUtils.ShouldThrottleIncomingTLSConnections(this.sessionState.ServerState.InboundTlsIPConnectionTable, this.sessionState.Configuration.TransportConfiguration.IsReceiveTlsThrottlingEnabled);
			}
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> TlsDisabledResult = new ParseAndProcessResult<SmtpInStateMachineEvents>(StartTlsSmtpCommandParser.UnrecognizedCommand, SmtpInStateMachineEvents.CommandFailed);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> TlsConnectionThrottledResult = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.StartTlsTempReject, SmtpInStateMachineEvents.CommandFailed, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> TlsNegotiationFailureResult = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);
	}
}
