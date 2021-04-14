using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class AuthSmtpCommand : SmtpCommand
	{
		public AuthSmtpCommand(ISmtpSession session, bool expsCommand, ITransportConfiguration transportConfiguration) : base(session, expsCommand ? "X-EXPS" : "AUTH", "OnAuthCommand", LatencyComponent.None)
		{
			this.expsCommand = expsCommand;
			this.authCommandEventArgs = new AuthCommandEventArgs();
			this.CommandEventArgs = this.authCommandEventArgs;
			this.transportConfiguration = transportConfiguration;
		}

		private bool HandlingIntegratedAuth
		{
			get
			{
				return (this.currentAuthMechanism == SmtpAuthenticationMechanism.Ntlm || this.currentAuthMechanism == SmtpAuthenticationMechanism.Gssapi) && !this.expsCommand;
			}
		}

		public override IAsyncResult BeginRaiseEvent(AsyncCallback callback, object state)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool flag = SmtpInSessionUtils.IsAuthenticated(smtpInSession.RemoteIdentity);
			if (!this.authEventRaised)
			{
				AsyncCallback callback2 = callback;
				if (flag)
				{
					callback2 = new AsyncCallback(this.OnAuthCallback);
					this.inboundCallback = callback;
				}
				this.authEventRaised = true;
				IAsyncResult asyncResult = base.BeginRaiseEvent(callback2, state);
				if (!asyncResult.CompletedSynchronously || !flag || !smtpInSession.SessionSource.SmtpResponse.Equals(SmtpResponse.Empty))
				{
					return asyncResult;
				}
				this.inboundCallback = null;
			}
			if (flag)
			{
				return this.RaiseEndOfAuthenticationEvent(callback, state);
			}
			if (this.completeNeeded)
			{
				this.completeNeeded = false;
				this.inboundCallback = callback;
				ProcessAuthenticationEventArgs processAuthenticationEventArgs = new ProcessAuthenticationEventArgs(smtpInSession.SessionSource, this.loginUserName, this.securePassword);
				object state2 = processAuthenticationEventArgs;
				IAsyncResult asyncResult2 = smtpInSession.AgentSession.BeginRaiseEvent("OnProcessAuthentication", ReceiveCommandEventSourceImpl.Create(smtpInSession.SessionSource), processAuthenticationEventArgs, new AsyncCallback(this.OnLiveIdAuthCallback), state2);
				if (asyncResult2.CompletedSynchronously)
				{
					if (processAuthenticationEventArgs.Identity == null)
					{
						this.LogAndHandleLiveIdAuthFailure(processAuthenticationEventArgs.AuthResult, processAuthenticationEventArgs.AuthErrorDetails);
					}
					else if (this.transportConfiguration.AppConfig.SmtpProxyConfiguration.SimulateUserNotInAdAuthError && smtpInSession.Connector.ProxyEnabled)
					{
						this.HandleAuthFailure();
					}
					else
					{
						this.authContext.Identity = processAuthenticationEventArgs.Identity;
						this.InboundAuthComplete();
					}
				}
				return asyncResult2;
			}
			return smtpInSession.AgentSession.BeginNoEvent(callback, state);
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!base.VerifyNoOngoingBdat())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			switch (this.currentAuthMechanism)
			{
			case SmtpAuthenticationMechanism.None:
				this.InboundParseMechanism();
				return;
			case SmtpAuthenticationMechanism.Login:
				this.InboundParseLogin(false);
				return;
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				this.InboundParseNtlmOrGssapi(false);
				return;
			case SmtpAuthenticationMechanism.ExchangeAuth:
				this.InboundParseExchangeAuth();
				return;
			default:
				return;
			}
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus == ParsingStatus.MoreDataRequired)
			{
				return;
			}
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (SmtpInSessionUtils.IsAuthenticated(smtpInSession.RemoteIdentity))
			{
				if (base.SmtpResponse.Equals(SmtpResponse.Empty))
				{
					base.SmtpResponse = SmtpResponse.AuthSuccessful;
				}
				smtpInSession.SessionSource.Properties["Microsoft.Exchange.IsAuthenticated"] = true;
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "Authentication successful");
				string input;
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.Login && this.loginUserName != null)
				{
					string @string = Encoding.UTF8.GetString(this.loginUserName);
					char[] trimChars = new char[1];
					input = @string.TrimEnd(trimChars);
				}
				else
				{
					input = smtpInSession.RemoteIdentityName;
				}
				string s = Util.RedactUserName(input);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "authenticated", bytes);
			}
		}

		internal override void OutboundCreateCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			string text = null;
			SecureString password = null;
			AuthenticationMechanism mechanism = AuthenticationMechanism.None;
			if (this.expsCommand)
			{
				if (Components.IsBridgehead)
				{
					if (smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
					{
						if (!AuthCommandHelpers.IsExchangeAuthHashSupported(smtpOutSession.AdvertisedEhloOptions))
						{
							AuthCommandHelpers.LogExchangeAuthHashNotSupported(smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer, this.GetHashCode(), smtpOutSession.LogSession, SmtpCommand.EventLog);
							return;
						}
						this.currentAuthMechanism = SmtpAuthenticationMechanism.ExchangeAuth;
					}
					else if (smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS GSSAPI") && (smtpOutSession.NextHopDeliveryType == DeliveryType.SmtpRelayToTiRg || smtpOutSession.CanDowngradeExchangeServerAuth))
					{
						this.currentAuthMechanism = SmtpAuthenticationMechanism.Gssapi;
						mechanism = AuthenticationMechanism.Negotiate;
					}
				}
			}
			else
			{
				text = smtpOutSession.AuthenticationUsername;
				password = smtpOutSession.AuthenticationPassword;
				if (smtpOutSession.Connector.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuth || smtpOutSession.Connector.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS)
				{
					this.currentAuthMechanism = SmtpAuthenticationMechanism.Login;
					mechanism = AuthenticationMechanism.Login;
				}
			}
			SecurityStatus securityStatus;
			switch (this.currentAuthMechanism)
			{
			case SmtpAuthenticationMechanism.Login:
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				this.authContext = new AuthenticationContext();
				securityStatus = this.authContext.InitializeForOutboundNegotiate(mechanism, "SMTPSVC/" + smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN, text, password);
				break;
			case SmtpAuthenticationMechanism.ExchangeAuth:
			{
				byte[] tlsEapKey = smtpOutSession.GetTlsEapKey();
				this.authContext = new AuthenticationContext();
				if (tlsEapKey != null)
				{
					securityStatus = this.authContext.InitializeForOutboundExchangeAuth("SHA256", "SMTPSVC/" + smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN, smtpOutSession.GetCertificatePublicKey(), tlsEapKey);
				}
				else
				{
					securityStatus = SecurityStatus.TLS1_0NotSupported;
				}
				break;
			}
			default:
				securityStatus = SecurityStatus.Unsupported;
				break;
			}
			if (securityStatus != SecurityStatus.OK)
			{
				string text2 = string.Format("Outbound Authentication failed to initialize with " + securityStatus.ToString(), new object[0]);
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), text2);
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAuthenticationInitializationFailed, null, new object[]
					{
						securityStatus,
						smtpOutSession.Connector.Name,
						this.currentAuthMechanism,
						"SMTPSVC/" + smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN,
						(text == null) ? current.Name : text
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendAuthenticationInitializationFailed", null, text2, ResultSeverityLevel.Error, false);
				}
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text2);
				this.currentAuthMechanism = SmtpAuthenticationMechanism.None;
			}
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			switch (this.currentAuthMechanism)
			{
			case SmtpAuthenticationMechanism.None:
				smtpOutSession.FailoverConnection(SmtpResponse.AuthTempFailure);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			case SmtpAuthenticationMechanism.Login:
			case SmtpAuthenticationMechanism.Gssapi:
			case SmtpAuthenticationMechanism.Ntlm:
				this.OutboundFormatNegotiateCommand(smtpOutSession.Connector.Name, smtpOutSession.LogSession, smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN);
				return;
			case SmtpAuthenticationMechanism.ExchangeAuth:
				this.OutboundFormatExchangeAuthCommand();
				return;
			default:
				return;
			}
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (this.cancelInProgress && statusCode[0] != '5')
			{
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "An error occurred during authentication, so we tried to cancel AUTH. However the server did not respond correctly to the cancel request, so the connection will be dropped. The server response was: {0}", base.SmtpResponse);
				throw new FormatException("Tried to cancel AUTH, but server did not exit out of of negotiation");
			}
			if (statusCode[0] == '5' || statusCode[0] == '4')
			{
				smtpOutSession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Authentication failed, the response was: {0}.  Attempting to failover connection.", base.SmtpResponse);
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAuthenticationFailed, null, new object[]
				{
					smtpOutSession.Connector.Name,
					smtpOutSession.RemoteEndPoint,
					base.SmtpResponse
				});
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.ExchangeAuth)
				{
					AuthCommandHelpers.TryFlushKerberosTicketCache(this.transportConfiguration.AppConfig.SmtpAvailabilityConfiguration.KerberosTicketCacheFlushMinInterval, smtpOutSession.LogSession);
				}
				smtpOutSession.FailoverConnection(base.SmtpResponse);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (statusCode[0] == '2')
			{
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.ExchangeAuth)
				{
					smtpOutSession.LogSession.LogReceive(SmtpResponse.GssapiSmtpResponseToLog235.ToByteArray());
					this.OutboundProcessExchangeAuthResponse(base.SmtpResponse.StatusText[0]);
					return;
				}
				smtpOutSession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
				smtpOutSession.IsAuthenticated = true;
				if (smtpOutSession is ShadowSmtpOutSession)
				{
					smtpOutSession.PrepareToSendXshadowOrMessage();
					return;
				}
				if (smtpOutSession is InboundProxySmtpOutSession && smtpOutSession.AdvertisedEhloOptions.XProxyFrom)
				{
					smtpOutSession.NextState = SmtpOutSession.SessionState.XProxyFrom;
					return;
				}
				smtpOutSession.PrepareNextStateForEstablishedSession();
				return;
			}
			else
			{
				if (base.SmtpResponse.StatusText.Length != 1 || !string.Equals(statusCode, "334", StringComparison.Ordinal))
				{
					smtpOutSession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
					throw new FormatException("AUTH response was illegally formatted");
				}
				base.ParsingStatus = ParsingStatus.MoreDataRequired;
				smtpOutSession.LogSession.LogReceive(SmtpResponse.GssapiSmtpResponseToLog334.ToByteArray());
				if (!this.authVerbSent)
				{
					this.authVerbSent = true;
					return;
				}
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.Gssapi || this.currentAuthMechanism == SmtpAuthenticationMechanism.Ntlm)
				{
					this.authBlob = Util.AsciiStringToBytes(base.SmtpResponse.StatusText[0]);
				}
				return;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.authContext != null)
					{
						this.authContext.Dispose();
						this.authContext = null;
					}
					if (this.securePassword != null)
					{
						this.securePassword.Dispose();
						this.securePassword = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void LogAndHandleLiveIdAuthFailure(object authResult, string errorDetails)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (authResult is LiveIdAuthResult)
			{
				LiveIdAuthResult liveIdAuthResult = (LiveIdAuthResult)authResult;
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "LiveID Authentication failed with reason: " + liveIdAuthResult.ToString() + ", Details: " + AuthCommandHelpers.RedactUserNameInErrorDetails(errorDetails, Util.IsDataRedactionNecessary()));
				if (liveIdAuthResult == LiveIdAuthResult.LiveServerUnreachable || liveIdAuthResult == LiveIdAuthResult.OperationTimedOut || liveIdAuthResult == LiveIdAuthResult.CommunicationFailure)
				{
					smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToWLIDDown);
				}
			}
			this.HandleAuthFailure();
		}

		private void InboundParseMechanism()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			int num;
			int num2;
			if (!base.GetNextArgOffsets(out num, out num2))
			{
				smtpInSession.LogSession.LogReceive(base.ProtocolCommand);
				base.SmtpResponse = SmtpResponse.AuthUnrecognized;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "No auth mechanism specified after auth or x-exps verb");
				return;
			}
			byte[] array = new byte[num2];
			Buffer.BlockCopy(base.ProtocolCommand, 0, array, 0, array.Length);
			smtpInSession.LogSession.LogReceive(array);
			if (!base.VerifyHelloReceived() || !base.VerifyNotAuthenticatedThroughAuthVerb() || !base.VerifyNotAuthenticatedForInboundClientProxy())
			{
				return;
			}
			if (smtpInSession.TransportMailItem != null)
			{
				base.SmtpResponse = SmtpResponse.BadCommandSequence;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "The AUTH command is not permitted during a mail transaction");
				return;
			}
			if (this.expsCommand && (smtpInSession.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) == AuthMechanisms.None)
			{
				base.SmtpResponse = SmtpResponse.AuthUnrecognized;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "EXPS command can only appear if Exchange Server auth mechanisms is set");
				return;
			}
			if (SmtpCommand.CompareArg(AuthSmtpCommandParser.Login, base.ProtocolCommand, num, num2 - num))
			{
				if (this.InboundBeginLogin())
				{
					this.InboundParseLogin(true);
					return;
				}
			}
			else
			{
				if (!base.VerifyNotAuthenticatedThroughAuthVerb())
				{
					return;
				}
				if (SmtpCommand.CompareArg(AuthSmtpCommandParser.ExchangeAuth, base.ProtocolCommand, num, num2 - num))
				{
					if (this.expsCommand && (smtpInSession.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None && smtpInSession.SmtpInServer.IsBridgehead && smtpInSession.SecureState == SecureState.AnonymousTls)
					{
						this.multilevelAuthMechanism = MultilevelAuthMechanism.MUTUALGSSAPI;
						this.currentAuthMechanism = SmtpAuthenticationMechanism.ExchangeAuth;
						this.InboundParseExchangeAuth();
						return;
					}
				}
				else if (SmtpCommand.CompareArg(AuthSmtpCommandParser.GSSAPI, base.ProtocolCommand, num, num2 - num))
				{
					bool flag = (smtpInSession.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None;
					if ((flag && this.expsCommand && smtpInSession.SmtpInServer.IsBridgehead) || (smtpInSession.SupportIntegratedAuth && smtpInSession.Connector.EnableAuthGSSAPI && !this.expsCommand))
					{
						this.multilevelAuthMechanism = MultilevelAuthMechanism.GSSAPI;
						this.currentAuthMechanism = SmtpAuthenticationMechanism.Gssapi;
						this.InboundParseNtlmOrGssapi(true);
						return;
					}
				}
				else if (SmtpCommand.CompareArg(AuthSmtpCommandParser.NTLM, base.ProtocolCommand, num, num2 - num))
				{
					bool flag2 = (smtpInSession.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None;
					if ((flag2 && this.expsCommand) || (smtpInSession.SupportIntegratedAuth && !this.expsCommand))
					{
						this.multilevelAuthMechanism = MultilevelAuthMechanism.NTLM;
						this.currentAuthMechanism = SmtpAuthenticationMechanism.Ntlm;
						this.InboundParseNtlmOrGssapi(true);
						return;
					}
				}
			}
			base.SmtpResponse = SmtpResponse.AuthUnrecognized;
			base.ParsingStatus = ParsingStatus.ProtocolError;
			Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "Auth mechanism not supported");
		}

		private bool InboundBeginLogin()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool flag = (smtpInSession.Connector.AuthMechanism & AuthMechanisms.BasicAuth) != AuthMechanisms.None;
			if (this.expsCommand || !flag)
			{
				return false;
			}
			bool flag2 = smtpInSession.SecureState == SecureState.StartTls || smtpInSession.SecureState == SecureState.AnonymousTls;
			bool flag3 = (smtpInSession.Connector.AuthMechanism & AuthMechanisms.BasicAuthRequireTLS) != AuthMechanisms.None;
			if (flag2)
			{
				this.multilevelAuthMechanism = MultilevelAuthMechanism.TLSAuthLogin;
			}
			else
			{
				if (flag3)
				{
					return false;
				}
				this.multilevelAuthMechanism = MultilevelAuthMechanism.Login;
			}
			this.currentAuthMechanism = SmtpAuthenticationMechanism.Login;
			return true;
		}

		private void InboundParseLogin(bool parsingAuthLoginLine)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			int inputOffset;
			int inputLength;
			if (!this.GetNextAuthBlob(out inputOffset, out inputLength))
			{
				return;
			}
			SecurityStatus securityStatus;
			if (parsingAuthLoginLine)
			{
				if (smtpInSession.Connector.LiveCredentialEnabled)
				{
					this.authContext = new AuthenticationContext(new ExternalLoginAuthentication(this.ExtractClearTextCredentialForLiveId));
				}
				else
				{
					this.authContext = new AuthenticationContext(new ExternalLoginProcessing(this.ExtractClearTextCredentialForLogin));
				}
				securityStatus = this.authContext.InitializeForInboundNegotiate(AuthenticationMechanism.Login);
				if (securityStatus != SecurityStatus.OK)
				{
					Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError<SecurityStatus>((long)this.GetHashCode(), "InitializeForInboundNegotiate failed: {0}", securityStatus);
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Unable to initialize inbound AUTH LOGIN because of " + securityStatus.ToString());
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, smtpInSession.RemoteEndPoint.Address.ToString(), new object[]
					{
						securityStatus,
						smtpInSession.Connector.Name,
						this.currentAuthMechanism,
						smtpInSession.RemoteEndPoint.Address
					});
					this.HandleAuthFailure();
					return;
				}
			}
			byte[] blob;
			securityStatus = this.authContext.NegotiateSecurityContext(base.ProtocolCommand, inputOffset, inputLength, out blob);
			SecurityStatus securityStatus2 = securityStatus;
			if (securityStatus2 == SecurityStatus.OK)
			{
				this.InboundAuthComplete();
				return;
			}
			switch (securityStatus2)
			{
			case SecurityStatus.ContinueNeeded:
				base.SmtpResponse = SmtpResponse.AuthBlob(blob);
				base.ParsingStatus = ParsingStatus.MoreDataRequired;
				return;
			case SecurityStatus.CompleteNeeded:
				base.ParsingStatus = ParsingStatus.Complete;
				this.completeNeeded = true;
				return;
			default:
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer.TraceDebug<SecurityStatus>((long)this.GetHashCode(), "NegotiateSecurityContext(LOGON) failed: {0}", securityStatus);
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound AUTH LOGIN failed because of " + securityStatus.ToString());
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailed, smtpInSession.RemoteEndPoint.Address.ToString(), new object[]
				{
					securityStatus,
					smtpInSession.Connector.Name,
					this.currentAuthMechanism,
					smtpInSession.RemoteEndPoint.Address
				});
				this.HandleAuthFailure();
				return;
			}
		}

		private void InboundParseNtlmOrGssapi(bool parsingAuthLine)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			int inputOffset;
			int num;
			if (!this.GetNextAuthBlob(out inputOffset, out num))
			{
				return;
			}
			SecurityStatus securityStatus;
			if (parsingAuthLine)
			{
				this.authContext = new AuthenticationContext(smtpInSession.ExtendedProtectionConfig, smtpInSession.ChannelBindingToken);
				AuthenticationMechanism mechanism;
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.Ntlm)
				{
					mechanism = AuthenticationMechanism.Ntlm;
				}
				else if (this.expsCommand)
				{
					mechanism = AuthenticationMechanism.Negotiate;
				}
				else
				{
					mechanism = AuthenticationMechanism.Gssapi;
				}
				securityStatus = this.authContext.InitializeForInboundNegotiate(mechanism);
				if (securityStatus != SecurityStatus.OK)
				{
					Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError<SecurityStatus>((long)this.GetHashCode(), "InitializeForInboundNegotiate failed: {0}", securityStatus);
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Failed to initialize inbound Negotiate because of " + securityStatus.ToString());
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, smtpInSession.RemoteEndPoint.Address.ToString(), new object[]
					{
						securityStatus,
						smtpInSession.Connector.Name,
						this.currentAuthMechanism,
						smtpInSession.RemoteEndPoint.Address
					});
					this.HandleAuthFailure();
					return;
				}
				if (num == 0)
				{
					if (this.currentAuthMechanism == SmtpAuthenticationMechanism.Ntlm)
					{
						base.SmtpResponse = SmtpResponse.NtlmSupported;
					}
					else
					{
						base.SmtpResponse = SmtpResponse.GssapiSupported;
					}
					base.ParsingStatus = ParsingStatus.MoreDataRequired;
					return;
				}
			}
			byte[] blob;
			securityStatus = this.authContext.NegotiateSecurityContext(base.ProtocolCommand, inputOffset, num, out blob);
			SecurityStatus securityStatus2 = securityStatus;
			if (securityStatus2 == SecurityStatus.OK)
			{
				this.InboundAuthComplete();
				return;
			}
			if (securityStatus2 != SecurityStatus.ContinueNeeded)
			{
				Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError<SecurityStatus>((long)this.GetHashCode(), "NegotiateSecurityContext failed: {0}", securityStatus);
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound Negotiate failed because of " + securityStatus.ToString());
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailed, smtpInSession.RemoteEndPoint.Address.ToString(), new object[]
				{
					securityStatus,
					smtpInSession.Connector.Name,
					this.currentAuthMechanism,
					smtpInSession.RemoteEndPoint.Address
				});
				this.HandleAuthFailure();
				return;
			}
			base.ParsingStatus = ParsingStatus.MoreDataRequired;
			base.SmtpResponse = SmtpResponse.AuthBlob(blob);
		}

		private void InboundParseExchangeAuth()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			string nextArg = base.GetNextArg();
			if (string.IsNullOrEmpty(nextArg))
			{
				this.HandleAuthInitFailure(smtpInSession, TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, "InitializeForInboundExchangeAuth failed: client did not specify hash algorithm", "Inbound ExchangeAuth negotiation failed because client did not specify hash algorithm", "Client did not specify hash algorithm");
				return;
			}
			int inputOffset;
			int inputLength;
			if (!this.GetNextAuthBlob(out inputOffset, out inputLength))
			{
				return;
			}
			this.authContext = new AuthenticationContext();
			if (!smtpInSession.IsTls)
			{
				this.HandleAuthInitFailure(smtpInSession, TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, "InitializeForInboundExchangeAuth failed: Connection is not TLS", "Inbound ExchangeAuth negotiation failed because connection is not TLS", "Connection is not TLS");
				return;
			}
			byte[] certificatePublicKey;
			try
			{
				certificatePublicKey = smtpInSession.GetCertificatePublicKey();
			}
			catch (CryptographicException ex)
			{
				this.HandleAuthInitFailure(smtpInSession, TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, "InitializeForInboundExchangeAuth failed: " + ex.Message, "Inbound ExchangeAuth negotiation failed because of " + ex.Message, ex.Message);
				return;
			}
			SecurityStatus securityStatus = this.authContext.InitializeForInboundExchangeAuth(nextArg, "SMTPSVC/" + smtpInSession.HelloSmtpDomain, certificatePublicKey, smtpInSession.TlsEapKey);
			if (securityStatus != SecurityStatus.OK)
			{
				string text = securityStatus.ToString();
				this.HandleAuthInitFailure(smtpInSession, TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationInitializationFailed, "InitializeForInboundExchangeAuth failed: " + text, "Inbound ExchangeAuth negotiation failed because of " + text, text);
			}
			else
			{
				byte[] bytes;
				SecurityStatus securityStatus2 = this.authContext.NegotiateSecurityContext(base.ProtocolCommand, inputOffset, inputLength, out bytes);
				if (securityStatus2 != SecurityStatus.OK)
				{
					string text2 = securityStatus2.ToString();
					this.HandleAuthInitFailure(smtpInSession, TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailed, "NegotiateSecurityContext for ExchangeAuth failed: " + text2, "Inbound ExchangeAuth negotiation failed because of " + text2, text2);
					return;
				}
				if (!this.InboundAuthComplete())
				{
					return;
				}
				base.SmtpResponse = new SmtpResponse("235", null, new string[]
				{
					Encoding.ASCII.GetString(bytes)
				});
				base.ParsingStatus = ParsingStatus.Complete;
				return;
			}
		}

		private bool InboundAuthComplete()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			int hashCode = smtpInSession.GetHashCode();
			if (AuthCommandHelpers.IsInactiveFrontend(smtpInSession.SmtpInServer.TargetRunningState, this.transportConfiguration.ProcessTransportRole) && !AuthCommandHelpers.IsSysProbeSession(smtpInSession.TlsDomainCapabilities))
			{
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "Rejecting the authentication and disconnecting as frontend transport service is Inactive.", null);
				base.SmtpResponse = SmtpResponse.ServiceInactive;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				smtpInSession.Disconnect(DisconnectReason.Local);
				return false;
			}
			SecurityIdentifier securityIdentifier;
			if (!AuthCommandHelpers.TryGetAuthenticatedSidFromIdentity(this.authContext, smtpInSession.LogSession, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, hashCode, SmtpCommand.EventLog, out securityIdentifier))
			{
				base.SmtpResponse = SmtpResponse.AuthTempFailure;
				base.ParsingStatus = ParsingStatus.Error;
				smtpInSession.Disconnect(DisconnectReason.Local);
				return false;
			}
			string authenticatedUsernameFromIdentity = AuthCommandHelpers.GetAuthenticatedUsernameFromIdentity(this.authContext, securityIdentifier, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, hashCode, SmtpCommand.EventLog);
			string text = Util.RedactUserName(authenticatedUsernameFromIdentity);
			if (AuthCommandHelpers.IsProhibitedAccount(this.authContext))
			{
				AuthCommandHelpers.LogAuthenticatedAsProhibitedAccount(authenticatedUsernameFromIdentity, text, smtpInSession.Connector.Name, smtpInSession.LogSession, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, hashCode, SmtpCommand.EventLog);
				this.HandleAuthFailure();
				return false;
			}
			if (AuthCommandHelpers.IsUserLookupRequiredOnAuth(this.currentAuthMechanism, smtpInSession.RemoteIdentity, this.transportConfiguration.ProcessTransportRole))
			{
				TransportMiniRecipient authUserRecipient;
				SmtpResponse smtpResponse;
				if (!AuthCommandHelpers.TryLookupAuthenticatedUserRecipientObjectFromSid(this.loginUserName, securityIdentifier, smtpInSession.Connector, smtpInSession.LogSession, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, smtpInSession.GetHashCode(), text, out authUserRecipient, out smtpResponse))
				{
					base.ParsingStatus = ParsingStatus.Error;
					base.SmtpResponse = smtpResponse;
					smtpInSession.Disconnect(DisconnectReason.Local);
					return false;
				}
				smtpInSession.AuthUserRecipient = authUserRecipient;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.ClientAuthRequireMailboxDatabase.Enabled && smtpInSession.AuthUserRecipient.Database == null)
				{
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication succeeded, but {0} has no mailbox database.", new object[]
					{
						text
					});
					base.ParsingStatus = ParsingStatus.Error;
					base.SmtpResponse = SmtpResponse.MailboxOffline;
					smtpInSession.Disconnect(DisconnectReason.Local);
					return false;
				}
				if (SmtpInAccessChecker.HasZeroProhibitSendQuota(smtpInSession.AuthUserRecipient, smtpInSession.RemoteIdentityName, smtpInSession.RemoteIdentityName, smtpInSession.Connector.Name, smtpInSession.Tracer, hashCode))
				{
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication succeeded, but {0} has ProhibitSendQuota of zero.", new object[]
					{
						text
					});
					base.ParsingStatus = ParsingStatus.Error;
					base.SmtpResponse = SmtpResponse.SubmissionDisabledBySendQuota;
					smtpInSession.Disconnect(DisconnectReason.Local);
					return false;
				}
			}
			smtpInSession.SetSessionPermissions(this.authContext.Identity.Token);
			if (!SmtpInSessionUtils.HasSMTPSubmitPermission(smtpInSession.Permissions))
			{
				AuthCommandHelpers.LogDoesNotHaveSmtpSubmitPermission(authenticatedUsernameFromIdentity, text, smtpInSession.Connector.Name, smtpInSession.LogSession, Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, hashCode, SmtpCommand.EventLog);
				this.HandleAuthFailure();
				return false;
			}
			base.ParsingStatus = ParsingStatus.Complete;
			smtpInSession.RemoteIdentity = securityIdentifier;
			smtpInSession.RemoteWindowsIdentity = this.authContext.DetachIdentity();
			smtpInSession.RemoteIdentityName = authenticatedUsernameFromIdentity;
			smtpInSession.AuthMethod = this.multilevelAuthMechanism;
			this.authContext.Dispose();
			this.authContext = null;
			if (!this.HandleInboundClientProxyState(smtpInSession))
			{
				base.SmtpResponse = SmtpResponse.AuthUnsuccessful;
				base.ParsingStatus = ParsingStatus.Error;
				return false;
			}
			if (SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(smtpInSession.Permissions) || SmtpInSessionUtils.ShouldAcceptProxyToProtocol(this.transportConfiguration.ProcessTransportRole, smtpInSession.Capabilities))
			{
				smtpInSession.RemoveClientIpConnection();
			}
			if (smtpInSession.InboundClientProxyState == InboundClientProxyStates.XProxyReceived)
			{
				smtpInSession.InboundClientProxyState = InboundClientProxyStates.XProxyReceivedAndAuthenticated;
			}
			return true;
		}

		private void HandleAuthInitFailure(ISmtpInSession session, ExEventLog.EventTuple eventTuple, string traceMsg, string logMsg, string eventMsg)
		{
			Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), traceMsg);
			session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, logMsg);
			SmtpCommand.EventLogger.LogEvent(eventTuple, session.RemoteEndPoint.Address.ToString(), new object[]
			{
				eventMsg,
				session.Connector.Name,
				this.currentAuthMechanism,
				session.RemoteEndPoint.Address
			});
			this.HandleAuthFailure();
		}

		private void HandleAuthFailure()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "Authentication failed");
			base.ParsingStatus = ParsingStatus.Error;
			smtpInSession.LogonFailures++;
			if (this.currentAuthMechanism != SmtpAuthenticationMechanism.None)
			{
				string text = null;
				if (this.authContext != null && this.authContext.UserNameBytes != null)
				{
					text = Encoding.ASCII.GetString(this.authContext.UserNameBytes);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = "NULL";
				}
				else
				{
					text = Util.RedactUserName(text);
				}
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "User Name: {0}", new object[]
				{
					text
				});
			}
			if (!AuthSmtpCommand.IsMaxLogonFailuresExceeded(smtpInSession.LogonFailures, smtpInSession.Connector.MaxLogonFailures))
			{
				if (this.expsCommand)
				{
					base.SmtpResponse = SmtpResponse.AuthTempFailure;
					smtpInSession.Disconnect(DisconnectReason.DroppedSession);
				}
				else
				{
					base.SmtpResponse = SmtpResponse.AuthUnsuccessful;
				}
			}
			else
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailedTooManyErrors, null, new object[]
				{
					smtpInSession.Connector.Name,
					smtpInSession.MaxLogonFailures,
					smtpInSession.RemoteEndPoint
				});
				base.SmtpResponse = SmtpResponse.TooManyAuthenticationErrors;
				smtpInSession.Disconnect(DisconnectReason.TooManyErrors);
			}
			if (this.authContext != null)
			{
				this.authContext.Dispose();
				this.authContext = null;
			}
		}

		private bool GetNextAuthBlob(out int beginOffset, out int length)
		{
			int num;
			if (!base.GetNextArgOffsets(out beginOffset, out num))
			{
				length = 0;
				return true;
			}
			length = num - beginOffset;
			if (length == 0 || (length == 1 && base.ProtocolCommand[beginOffset] == 61))
			{
				length = 0;
				return true;
			}
			if (length == 1 && base.ProtocolCommand[beginOffset] == 42)
			{
				base.SmtpResponse = SmtpResponse.AuthCancelled;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		private void OnAuthCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				AsyncCallback asyncCallback = this.inboundCallback;
				MExAsyncResult mexAsyncResult = (MExAsyncResult)this.RaiseEndOfAuthenticationEvent(asyncCallback, null);
				if (mexAsyncResult.CompletedSynchronously)
				{
					mexAsyncResult.SetAsync();
					asyncCallback(mexAsyncResult);
				}
			}
		}

		private void OnLiveIdAuthCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				ProcessAuthenticationEventArgs processAuthenticationEventArgs = (ProcessAuthenticationEventArgs)ar.AsyncState;
				ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
				if (processAuthenticationEventArgs.Identity == null)
				{
					this.LogAndHandleLiveIdAuthFailure(processAuthenticationEventArgs.AuthResult, processAuthenticationEventArgs.AuthErrorDetails);
					this.inboundCallback(ar);
					return;
				}
				if (this.transportConfiguration.AppConfig.SmtpProxyConfiguration.SimulateUserNotInAdAuthError && smtpInSession.Connector.ProxyEnabled)
				{
					this.HandleAuthFailure();
					this.inboundCallback(ar);
					return;
				}
				this.authContext.Identity = processAuthenticationEventArgs.Identity;
				this.InboundAuthComplete();
				MExAsyncResult mexAsyncResult = (MExAsyncResult)this.RaiseEndOfAuthenticationEvent(this.inboundCallback, null);
				if (mexAsyncResult.CompletedSynchronously)
				{
					mexAsyncResult.SetAsync();
					this.inboundCallback(mexAsyncResult);
				}
			}
		}

		private IAsyncResult RaiseEndOfAuthenticationEvent(AsyncCallback callback, object state)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			EndOfAuthenticationEventArgs originalEventArgsWrapper = new EndOfAuthenticationEventArgs(smtpInSession.SessionSource, this.authCommandEventArgs.AuthenticationMechanism, smtpInSession.RemoteIdentityName);
			this.originalEventArgsWrapper = originalEventArgsWrapper;
			return smtpInSession.AgentSession.BeginRaiseEvent("OnEndOfAuthentication", EndOfAuthenticationEventSourceImpl.Create(smtpInSession.SessionSource), this.originalEventArgsWrapper, callback, state);
		}

		protected virtual void SetCommandStringForAuth()
		{
			switch (this.currentAuthMechanism)
			{
			case SmtpAuthenticationMechanism.Login:
				base.ProtocolCommandString = "AUTH LOGIN";
				return;
			case SmtpAuthenticationMechanism.Gssapi:
				if (this.expsCommand)
				{
					base.ProtocolCommandString = "X-EXPS GSSAPI";
					return;
				}
				base.ProtocolCommandString = "AUTH GSSAPI";
				return;
			case SmtpAuthenticationMechanism.Ntlm:
				base.ProtocolCommandString = "AUTH NTLM";
				return;
			default:
				return;
			}
		}

		protected void OutboundFormatNegotiateCommand(string connectorName, IProtocolLogSession logSession, string advertisedFqdn)
		{
			if (!this.authVerbSent)
			{
				this.SetCommandStringForAuth();
				return;
			}
			byte[] array;
			SecurityStatus securityStatus = this.authContext.NegotiateSecurityContext(this.authBlob, out array);
			if (securityStatus == SecurityStatus.OK || securityStatus == SecurityStatus.ContinueNeeded)
			{
				int num = (array == null) ? 0 : array.Length;
				base.ProtocolCommand = new byte[num + 2];
				if (num != 0)
				{
					Buffer.BlockCopy(array, 0, base.ProtocolCommand, 0, num);
				}
				base.ProtocolCommand[num] = 13;
				base.ProtocolCommand[num + 1] = 10;
				return;
			}
			SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendOutboundAuthenticationFailed, null, new object[]
			{
				securityStatus,
				connectorName,
				this.currentAuthMechanism,
				"SMTPSVC/" + advertisedFqdn
			});
			string text = string.Format("Outbound Authentication failed with " + securityStatus.ToString(), new object[0]);
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendOutboundAuthenticationFailed", null, text, ResultSeverityLevel.Warning, false);
			Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer.TraceError<SecurityStatus>((long)this.GetHashCode(), "An error occurred during negotiation. Canceling AUTH. Status: {0}", securityStatus);
			logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text);
			this.cancelInProgress = true;
			base.ProtocolCommandString = "*";
		}

		private void OutboundFormatExchangeAuthCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			byte[] array;
			SecurityStatus securityStatus = this.authContext.NegotiateSecurityContext(null, out array);
			if (securityStatus != SecurityStatus.OK)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendOutboundAuthenticationFailed, null, new object[]
				{
					securityStatus,
					smtpOutSession.Connector.Name,
					this.currentAuthMechanism,
					"SMTPSVC/" + smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN
				});
				string text = string.Format("Outbound Authentication failed with " + securityStatus.ToString(), new object[0]);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendOutboundAuthenticationFailed", null, text, ResultSeverityLevel.Warning, false);
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text);
				smtpOutSession.FailoverConnection(SmtpResponse.AuthTempFailure);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			BufferBuilder bufferBuilder = new BufferBuilder("X-EXPS EXCHANGEAUTH SHA256 ".Length + array.Length + "\r\n".Length);
			bufferBuilder.Append("X-EXPS EXCHANGEAUTH SHA256 ");
			bufferBuilder.Append(array);
			bufferBuilder.Append("\r\n");
			bufferBuilder.RemoveUnusedBufferSpace();
			base.ProtocolCommand = bufferBuilder.GetBuffer();
			smtpOutSession.LogSession.LogSend(Util.AsciiStringToBytesAndAppendCRLF("X-EXPS EXCHANGEAUTH SHA256 "));
		}

		private void OutboundProcessExchangeAuthResponse(string response)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			byte[] array = new byte[response.Length];
			for (int i = 0; i < response.Length; i++)
			{
				array[i] = (byte)response[i];
			}
			byte[] array2;
			SecurityStatus securityStatus = this.authContext.NegotiateSecurityContext(array, out array2);
			if (securityStatus != SecurityStatus.OK)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAuthenticationFailed, null, new object[]
				{
					smtpOutSession.Connector.Name,
					smtpOutSession.RemoteEndPoint,
					base.SmtpResponse
				});
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Outbound Exchange Authentication failed with " + securityStatus.ToString());
				smtpOutSession.FailoverConnection(base.SmtpResponse);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			using (WindowsIdentity identity = this.authContext.Identity)
			{
				smtpOutSession.IsAuthenticated = identity.IsAuthenticated;
				try
				{
					smtpOutSession.RemoteIdentity = identity.User;
				}
				catch (SystemException ex)
				{
					Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "Could not obtain SID due to {0}. Failing authentication.", ex.Message);
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCouldNotDetermineUserNameOrSid, null, new object[]
					{
						ex.Message
					});
					smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Outbound Exchange Authentication failed to obtain remote's identity due to " + ex.Message);
					smtpOutSession.FailoverConnection(base.SmtpResponse);
					smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				try
				{
					smtpOutSession.RemoteIdentityName = identity.Name;
				}
				catch (SystemException ex2)
				{
					Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "Could not obtain name due to {0}. Using SID.", ex2.Message);
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCouldNotDetermineUserNameOrSid, null, new object[]
					{
						ex2.Message
					});
					smtpOutSession.RemoteIdentityName = smtpOutSession.RemoteIdentity.ToString();
				}
				smtpOutSession.SetSessionPermissions(identity.Token);
			}
			if (smtpOutSession is ShadowSmtpOutSession)
			{
				smtpOutSession.PrepareToSendXshadowOrMessage();
				return;
			}
			if (smtpOutSession is InboundProxySmtpOutSession && smtpOutSession.AdvertisedEhloOptions.XProxyFrom)
			{
				smtpOutSession.NextState = SmtpOutSession.SessionState.XProxyFrom;
				return;
			}
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub && smtpOutSession.NextHopDeliveryType == DeliveryType.SmtpDeliveryToMailbox && smtpOutSession.AdvertisedEhloOptions.XSessionMdbGuid)
			{
				smtpOutSession.NextState = SmtpOutSession.SessionState.XSessionParams;
				return;
			}
			smtpOutSession.PrepareNextStateForEstablishedSession();
		}

		private static bool IsMaxLogonFailuresExceeded(int numLogonFailures, int maxLogonFailures)
		{
			return maxLogonFailures > 0 && numLogonFailures > maxLogonFailures;
		}

		private void ExtractClearTextCredentialForLogin(byte[] domainName, byte[] userName, byte[] password)
		{
			this.loginUserName = AuthCommandHelpers.UsernameFromDomainAndUsername(domainName, userName);
			this.securePassword = AuthCommandHelpers.SecureStringFromBytes(password);
		}

		private SecurityStatus ExtractClearTextCredentialForLiveId(byte[] userName, byte[] password, out WindowsIdentity windowsIdentity, out IAccountValidationContext accountValidationContext)
		{
			windowsIdentity = null;
			accountValidationContext = null;
			this.loginUserName = userName;
			this.securePassword = AuthCommandHelpers.SecureStringFromBytes(password);
			return SecurityStatus.CompleteNeeded;
		}

		private bool HandleInboundClientProxyState(ISmtpInSession session)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			if ((this.currentAuthMechanism == SmtpAuthenticationMechanism.Login || this.HandlingIntegratedAuth) && session.ShouldProxyClientSession)
			{
				if (session.InboundClientProxyState != InboundClientProxyStates.None)
				{
					Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "The local server is configured to proxy client sessions but the incoming session is itself a proxied session");
					session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "The local server is configured to proxy client sessions but the incoming session is itself a proxied session.");
					return false;
				}
				session.StartClientProxySession = true;
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.Login)
				{
					session.ProxyUserName = new ASCIIEncoding().GetString(this.loginUserName, 0, this.loginUserName.Length - 1);
					session.ProxyPassword = this.securePassword.Copy();
				}
			}
			return true;
		}

		protected const string SmtpSpnPrefix = "SMTPSVC/";

		protected const string ExchangeAuthCommand = "X-EXPS EXCHANGEAUTH SHA256 ";

		protected bool authVerbSent;

		protected AuthenticationContext authContext;

		protected byte[] authBlob;

		protected SmtpAuthenticationMechanism currentAuthMechanism;

		protected bool cancelInProgress;

		protected bool expsCommand;

		private readonly AuthCommandEventArgs authCommandEventArgs;

		private AsyncCallback inboundCallback;

		private bool authEventRaised;

		private MultilevelAuthMechanism multilevelAuthMechanism;

		private bool completeNeeded;

		private byte[] loginUserName;

		private SecureString securePassword;

		private readonly ITransportConfiguration transportConfiguration;
	}
}
