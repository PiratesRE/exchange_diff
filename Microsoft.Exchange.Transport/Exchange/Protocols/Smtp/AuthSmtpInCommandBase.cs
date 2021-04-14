using System;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class AuthSmtpInCommandBase : SmtpInCommandBase
	{
		protected AuthSmtpInCommandBase(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = AuthSmtpCommandParser.ParseAuthMechanism(commandContext, this.sessionState, this.AuthCommandType, out this.authParseOutput);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnAuthCommand";
				agentEventArgs = new AuthCommandEventArgs(this.sessionState, this.authParseOutput.AuthenticationMechanism.ToString());
			}
			return result;
		}

		protected override async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			ParseAndProcessResult<SmtpInStateMachineEvents> result2;
			using (AuthenticationContext authenticationContext = this.CreateAuthenticationContext())
			{
				SecurityStatus securityStatus = this.InitializeAuthenticationContext(authenticationContext);
				if (SspiContext.IsSecurityStatusFailure(securityStatus))
				{
					this.LogInitializeAuthenticationContextFailure(securityStatus);
					result2 = AuthSmtpInCommandBase.AuthFailed;
				}
				else
				{
					CommandContext commandContext = this.authParseOutput.InitialBlob;
					if (this.authParseOutput.InitialBlob.Length == 0 && (this.IsNtlmAuthentication || this.IsGssapiAuthentication))
					{
						await base.WriteToClientAsync(this.NtlmOrGssapiSupportedResponse);
						base.OnAwaitCompleted(cancellationToken);
						NetworkConnection.LazyAsyncResultWithTimeout readResult = await Util.ReadLineAsync(this.sessionState);
						base.OnAwaitCompleted(cancellationToken);
						if (Util.IsReadFailure(readResult))
						{
							this.sessionState.HandleNetworkError(readResult.Result);
							return AuthSmtpInCommandBase.ClientDisconnected;
						}
						commandContext = CommandContext.FromAsyncResult(readResult);
						if (AuthCommandHelpers.IsCancelAuthBlob(commandContext))
						{
							return AuthSmtpInCommandBase.AuthCancelled;
						}
					}
					byte[] smtpResponseBlob;
					NetworkConnection.LazyAsyncResultWithTimeout readResult2;
					for (;;)
					{
						securityStatus = authenticationContext.NegotiateSecurityContext(commandContext.Command, commandContext.Offset, commandContext.Length, out smtpResponseBlob);
						if (SspiContext.IsSecurityStatusFailure(securityStatus))
						{
							break;
						}
						if (securityStatus == SecurityStatus.ContinueNeeded)
						{
							await base.WriteToClientAsync(SmtpResponse.AuthBlob(smtpResponseBlob));
							base.OnAwaitCompleted(cancellationToken);
							readResult2 = await Util.ReadLineAsync(this.sessionState);
							base.OnAwaitCompleted(cancellationToken);
							if (Util.IsReadFailure(readResult2))
							{
								goto Block_10;
							}
							commandContext = CommandContext.FromAsyncResult(readResult2);
							if (AuthCommandHelpers.IsCancelAuthBlob(commandContext))
							{
								goto Block_11;
							}
						}
						if (securityStatus != SecurityStatus.ContinueNeeded)
						{
							goto Block_12;
						}
					}
					this.LogNegotiateSecurityContextFailure(securityStatus);
					return AuthSmtpInCommandBase.AuthFailed;
					Block_10:
					this.sessionState.HandleNetworkError(readResult2.Result);
					return AuthSmtpInCommandBase.ClientDisconnected;
					Block_11:
					return AuthSmtpInCommandBase.AuthCancelled;
					Block_12:
					if (securityStatus == SecurityStatus.CompleteNeeded)
					{
						ProcessAuthenticationEventArgs eventArgs = new ProcessAuthenticationEventArgs(this.sessionState, this.loginUsername, this.loginPassword);
						ParseAndProcessResult<SmtpInStateMachineEvents> processAuthenticationResult = await base.RaiseAgentEventAsync("OnProcessAuthentication", eventArgs, commandContext, ParseResult.ParsingComplete, cancellationToken, null);
						base.OnAwaitCompleted(cancellationToken);
						if (!processAuthenticationResult.SmtpResponse.IsEmpty)
						{
							return processAuthenticationResult;
						}
						if (eventArgs.Identity == null)
						{
							this.LogProcessAuthenticationEventFailure(eventArgs.AuthResult, eventArgs.AuthErrorDetails);
							AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, this.sessionState);
							return this.GetAuthenticationFailedResult(null, false);
						}
						authenticationContext.Identity = eventArgs.Identity;
					}
					ParseAndProcessResult<SmtpInStateMachineEvents> result = AuthCommandHelpers.OnAuthenticationComplete(this.authParseOutput, authenticationContext, this.sessionState, this.loginUsername, smtpResponseBlob, new AuthCommandHelpers.GetAuthenticationSuccessfulResult(this.GetAuthenticationSuccessfulResult), new AuthCommandHelpers.GetAuthenticationFailureResult(this.GetAuthenticationFailedResult));
					if (result.SmtpResponse.SmtpResponseType == SmtpResponseType.Success)
					{
						ParseAndProcessResult<SmtpInStateMachineEvents> endOfAuthenticationResult = await base.RaiseAgentEventAsync("OnEndOfAuthentication", new EndOfAuthenticationEventArgs(this.sessionState, this.authParseOutput.AuthenticationMechanism.ToString(), this.sessionState.RemoteIdentityName), commandContext, ParseResult.ParsingComplete, cancellationToken, EndOfAuthenticationEventSourceImpl.Create(this.sessionState));
						base.OnAwaitCompleted(cancellationToken);
						if (!endOfAuthenticationResult.SmtpResponse.IsEmpty)
						{
							return endOfAuthenticationResult;
						}
					}
					result2 = result;
				}
			}
			return result2;
		}

		protected abstract AuthCommand AuthCommandType { get; }

		protected abstract AuthenticationContext CreateAuthenticationContext();

		protected abstract SecurityStatus InitializeAuthenticationContext(AuthenticationContext authenticationContext);

		protected abstract ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationSuccessfulResult(byte[] lastNegotiateSecurityContextResponse);

		protected abstract ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationFailedResult(SmtpResponse? customSmtpResponse = null, bool disconnectClient = false);

		protected bool IsNtlmAuthentication
		{
			get
			{
				return this.authParseOutput.AuthenticationMechanism == SmtpAuthenticationMechanism.Ntlm;
			}
		}

		protected bool IsGssapiAuthentication
		{
			get
			{
				return this.authParseOutput.AuthenticationMechanism == SmtpAuthenticationMechanism.Gssapi;
			}
		}

		private SmtpResponse NtlmOrGssapiSupportedResponse
		{
			get
			{
				if (this.authParseOutput.AuthenticationMechanism != SmtpAuthenticationMechanism.Ntlm)
				{
					return SmtpResponse.GssapiSupported;
				}
				return SmtpResponse.NtlmSupported;
			}
		}

		protected void ExtractClearTextCredentialForLogin(byte[] domain, byte[] username, byte[] password)
		{
			this.loginUsername = AuthCommandHelpers.UsernameFromDomainAndUsername(domain, username);
			this.loginPassword = AuthCommandHelpers.SecureStringFromBytes(password);
		}

		protected SecurityStatus ExtractClearTextCredentialForLiveId(byte[] username, byte[] password, out WindowsIdentity windowsIdentity, out IAccountValidationContext accountValidationContext)
		{
			windowsIdentity = null;
			accountValidationContext = null;
			this.loginUsername = username;
			this.loginPassword = AuthCommandHelpers.SecureStringFromBytes(password);
			return SecurityStatus.CompleteNeeded;
		}

		protected override void LogCommandReceived(CommandContext command)
		{
		}

		private void LogInitializeAuthenticationContextFailure(SecurityStatus status)
		{
			this.sessionState.Tracer.TraceError<SecurityStatus>((long)this.GetHashCode(), "InitializeForInboundNegotiate failed: {0}", status);
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Unable to initialize inbound AUTH because of " + status);
			this.sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, this.networkConnection.RemoteEndPoint.Address.ToString(), new object[]
			{
				status,
				this.sessionState.ReceiveConnector.Name,
				this.authParseOutput.AuthenticationMechanism,
				this.networkConnection.RemoteEndPoint.Address
			});
		}

		private void LogNegotiateSecurityContextFailure(SecurityStatus status)
		{
			this.sessionState.Tracer.TraceDebug<SecurityStatus>((long)this.GetHashCode(), "NegotiateSecurityContext() failed: {0}", status);
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication failed because of " + status);
			this.sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailed, this.networkConnection.RemoteEndPoint.Address.ToString(), new object[]
			{
				status,
				this.sessionState.ReceiveConnector.Name,
				this.authParseOutput.AuthenticationMechanism,
				this.networkConnection.RemoteEndPoint.Address
			});
		}

		private void LogProcessAuthenticationEventFailure(object authResult, string errorDetails)
		{
			if (authResult is LiveIdAuthResult)
			{
				LiveIdAuthResult liveIdAuthResult = (LiveIdAuthResult)authResult;
				if (liveIdAuthResult == LiveIdAuthResult.LiveServerUnreachable || liveIdAuthResult == LiveIdAuthResult.OperationTimedOut || liveIdAuthResult == LiveIdAuthResult.CommunicationFailure)
				{
					this.sessionState.UpdateAvailabilityPerfCounters(LegitimateSmtpAvailabilityCategory.RejectDueToWLIDDown);
				}
			}
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Custom (generally LiveID) Authentication failed with reason: " + authResult.ToString() + ", Details: " + AuthCommandHelpers.RedactUserNameInErrorDetails(errorDetails, Util.IsDataRedactionNecessary()));
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> AuthFailed = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthUnsuccessful, SmtpInStateMachineEvents.CommandFailed, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> ClientDisconnected = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.NetworkError, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> AuthFailedDisconnecting = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthUnsuccessful, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> TooManyAuthenticationErrorsDisconnecting = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.TooManyAuthenticationErrors, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> AuthTempFailureDisconnecting = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthTempFailure, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> AuthCancelled = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.AuthCancelled, SmtpInStateMachineEvents.CommandFailed, false);

		private byte[] loginUsername;

		private SecureString loginPassword;

		protected AuthParseOutput authParseOutput;
	}
}
