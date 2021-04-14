using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XExpsSmtpInCommand : AuthSmtpInCommandBase
	{
		public XExpsSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		public static ParseAndProcessResult<SmtpInStateMachineEvents> CreateExchangeAuthSuccessfulResult(byte[] lastNegotiateSecurityContextResponse)
		{
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.ExchangeAuthSuccessful(lastNegotiateSecurityContextResponse), SmtpInStateMachineEvents.XExpsProcessed, false);
		}

		public override void LogSmtpResponse(SmtpResponse smtpResponse)
		{
			if (smtpResponse.IsEmpty)
			{
				return;
			}
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				this.sessionState.ProtocolLogSession.LogSend(smtpResponse.ToByteArray());
				return;
			case SmtpAuthenticationMechanism.ExchangeAuth:
				this.sessionState.ProtocolLogSession.LogSend(XExpsSmtpInCommand.ExchangeAuthSuccessProtocolLogLine);
				return;
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unexpected authentication mechanism: {0}", this.authParseOutput.AuthenticationMechanism));
			}
		}

		protected override AuthCommand AuthCommandType
		{
			get
			{
				return AuthCommand.XExps;
			}
		}

		protected override AuthenticationContext CreateAuthenticationContext()
		{
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				return new AuthenticationContext(this.sessionState.ExtendedProtectionConfig, this.sessionState.NetworkConnection.ChannelBindingToken);
			case SmtpAuthenticationMechanism.ExchangeAuth:
				return new AuthenticationContext();
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unexpected authentication mechanism: {0}", this.authParseOutput.AuthenticationMechanism));
			}
		}

		protected override SecurityStatus InitializeAuthenticationContext(AuthenticationContext authenticationContext)
		{
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Gssapi:
				return authenticationContext.InitializeForInboundNegotiate(AuthenticationMechanism.Negotiate);
			case SmtpAuthenticationMechanism.Ntlm:
				return authenticationContext.InitializeForInboundNegotiate(AuthenticationMechanism.Ntlm);
			case SmtpAuthenticationMechanism.ExchangeAuth:
				return this.InitializeExchangeAuthentication(authenticationContext);
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unexpected authentication mechanism: {0}", this.authParseOutput.AuthenticationMechanism));
			}
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationSuccessfulResult(byte[] lastNegotiateSecurityContextResponse)
		{
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				return XExpsSmtpInCommand.CommandComplete;
			case SmtpAuthenticationMechanism.ExchangeAuth:
				return XExpsSmtpInCommand.CreateExchangeAuthSuccessfulResult(lastNegotiateSecurityContextResponse);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationFailedResult(SmtpResponse? customSmtpResponse = null, bool disconnectClient = false)
		{
			this.sessionState.DisconnectReason = DisconnectReason.DroppedSession;
			if (customSmtpResponse == null)
			{
				return AuthSmtpInCommandBase.AuthTempFailureDisconnecting;
			}
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, customSmtpResponse.Value, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);
		}

		private SecurityStatus InitializeExchangeAuthentication(AuthenticationContext authenticationContext)
		{
			if (!this.sessionState.NetworkConnection.IsTls)
			{
				this.LogInitializeExchangeAuthenticationFailure(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, "Connection is not TLS", "InitializeExchangeAuthentication failed: Connection is not TLS", "Inbound ExchangeAuth negotiation failed because connection is not TLS");
				return SecurityStatus.InternalError;
			}
			byte[] publicKey;
			Exception ex;
			if (!AuthCommandHelpers.TryGetLocalCertificatePublicKey(this.sessionState.NetworkConnection, out publicKey, out ex))
			{
				this.LogInitializeExchangeAuthenticationFailure(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, ex.ToString(), "InitializeExchangeAuthentication failed: " + ex.Message, "Inbound ExchangeAuth negotiation failed because " + ex.Message);
				return SecurityStatus.CertUnknown;
			}
			SecurityStatus securityStatus = authenticationContext.InitializeForInboundExchangeAuth(this.authParseOutput.ExchangeAuthHashAlgorithm, "SMTPSVC/" + this.sessionState.HelloDomain, publicKey, this.sessionState.NetworkConnection.TlsEapKey);
			if (SspiContext.IsSecurityStatusFailure(securityStatus))
			{
				this.LogInitializeExchangeAuthenticationFailure(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, securityStatus.ToString(), "InitializeForInboundExchangeAuth failed: " + securityStatus, "Inbound ExchangeAuth negotiation failed because " + securityStatus);
				return securityStatus;
			}
			return securityStatus;
		}

		private void LogInitializeExchangeAuthenticationFailure(ExEventLog.EventTuple eventTuple, string eventMsg, string traceMsg, string logMsg)
		{
			this.sessionState.Tracer.TraceError((long)this.GetHashCode(), traceMsg);
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, logMsg);
			this.sessionState.EventLog.LogEvent(eventTuple, this.sessionState.NetworkConnection.RemoteEndPoint.Address.ToString(), new object[]
			{
				eventMsg,
				this.sessionState.ReceiveConnector.Name,
				this.authParseOutput.AuthenticationMechanism,
				this.sessionState.NetworkConnection.RemoteEndPoint.Address
			});
		}

		protected const string SmtpSpnPrefix = "SMTPSVC/";

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> CommandComplete = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthSuccessful, SmtpInStateMachineEvents.XExpsProcessed, false);

		private static readonly byte[] ExchangeAuthSuccessProtocolLogLine = Encoding.ASCII.GetBytes("235 <authentication response>");
	}
}
