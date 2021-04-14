using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class AuthSmtpInCommand : AuthSmtpInCommandBase
	{
		public AuthSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override AuthCommand AuthCommandType
		{
			get
			{
				return AuthCommand.Auth;
			}
		}

		protected override AuthenticationContext CreateAuthenticationContext()
		{
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Login:
				if (!this.sessionState.ReceiveConnector.LiveCredentialEnabled)
				{
					return new AuthenticationContext(new ExternalLoginProcessing(base.ExtractClearTextCredentialForLogin));
				}
				return new AuthenticationContext(new ExternalLoginAuthentication(base.ExtractClearTextCredentialForLiveId));
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				return new AuthenticationContext(this.sessionState.ExtendedProtectionConfig, this.sessionState.NetworkConnection.ChannelBindingToken);
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unexpected authentication mechanism: {0}", this.authParseOutput.AuthenticationMechanism));
			}
		}

		protected override SecurityStatus InitializeAuthenticationContext(AuthenticationContext authenticationContext)
		{
			switch (this.authParseOutput.AuthenticationMechanism)
			{
			case SmtpAuthenticationMechanism.Login:
				return authenticationContext.InitializeForInboundNegotiate(AuthenticationMechanism.Login);
			case SmtpAuthenticationMechanism.Gssapi:
				return authenticationContext.InitializeForInboundNegotiate(AuthenticationMechanism.Gssapi);
			case SmtpAuthenticationMechanism.Ntlm:
				return authenticationContext.InitializeForInboundNegotiate(AuthenticationMechanism.Ntlm);
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unexpected authentication mechanism: {0}", this.authParseOutput.AuthenticationMechanism));
			}
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationSuccessfulResult(byte[] lastNegotiateSecurityContextResponse)
		{
			return AuthSmtpInCommand.CommandComplete;
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationFailedResult(SmtpResponse? customSmtpResponse = null, bool disconnectClient = false)
		{
			if (this.sessionState.IsMaxLogonFailuresExceeded)
			{
				this.sessionState.DisconnectReason = DisconnectReason.TooManyErrors;
				return AuthSmtpInCommandBase.TooManyAuthenticationErrorsDisconnecting;
			}
			if (disconnectClient)
			{
				this.sessionState.DisconnectReason = DisconnectReason.Local;
			}
			if (customSmtpResponse != null)
			{
				return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, customSmtpResponse.Value, disconnectClient ? SmtpInStateMachineEvents.SendResponseAndDisconnectClient : SmtpInStateMachineEvents.CommandFailed, false);
			}
			if (!disconnectClient)
			{
				return AuthSmtpInCommandBase.AuthFailed;
			}
			return AuthSmtpInCommandBase.AuthFailedDisconnecting;
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> CommandComplete = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthSuccessful, SmtpInStateMachineEvents.AuthProcessed, false);
	}
}
