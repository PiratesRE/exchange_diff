using System;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class AuthSmtpProxyCommand : AuthSmtpCommand
	{
		public AuthSmtpProxyCommand(ISmtpSession session, ITransportConfiguration transportConfiguration, bool expsCommand) : base(session, expsCommand, transportConfiguration)
		{
		}

		internal override void InboundParseCommand()
		{
			throw new NotImplementedException();
		}

		internal override void InboundProcessCommand()
		{
			throw new NotImplementedException();
		}

		internal override void OutboundCreateCommand()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			string text = null;
			SecureString password = null;
			AuthenticationMechanism mechanism = AuthenticationMechanism.None;
			if (this.expsCommand)
			{
				if (!smtpOutProxySession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
				{
					return;
				}
				if (!AuthCommandHelpers.IsExchangeAuthHashSupported(smtpOutProxySession.AdvertisedEhloOptions))
				{
					AuthCommandHelpers.LogExchangeAuthHashNotSupported(smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN, ExTraceGlobals.SmtpReceiveTracer, this.GetHashCode(), smtpOutProxySession.LogSession, SmtpCommand.EventLog);
					return;
				}
				this.currentAuthMechanism = SmtpAuthenticationMechanism.ExchangeAuth;
			}
			else
			{
				text = smtpOutProxySession.InSession.ProxyUserName;
				password = smtpOutProxySession.InSession.ProxyPassword;
				this.currentAuthMechanism = SmtpAuthenticationMechanism.Login;
				mechanism = AuthenticationMechanism.Login;
			}
			SecurityStatus securityStatus;
			if (this.currentAuthMechanism == SmtpAuthenticationMechanism.ExchangeAuth)
			{
				byte[] tlsEapKey = smtpOutProxySession.GetTlsEapKey();
				this.authContext = new AuthenticationContext();
				if (tlsEapKey != null)
				{
					securityStatus = this.authContext.InitializeForOutboundExchangeAuth("SHA256", "SMTPSVC/" + smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN, smtpOutProxySession.GetCertificatePublicKey(), tlsEapKey);
				}
				else
				{
					securityStatus = SecurityStatus.TLS1_0NotSupported;
				}
			}
			else
			{
				this.authContext = new AuthenticationContext();
				securityStatus = this.authContext.InitializeForOutboundNegotiate(mechanism, "SMTPSVC/" + smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN, text, password);
			}
			if (securityStatus != SecurityStatus.OK)
			{
				string text2 = "Outbound Authentication failed to initialize with " + securityStatus.ToString();
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), text2);
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAuthenticationInitializationFailed, null, new object[]
					{
						securityStatus,
						smtpOutProxySession.Connector.Name,
						this.currentAuthMechanism,
						"SMTPSVC/" + smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN,
						(text == null) ? current.Name : text
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendAuthenticationInitializationFailed", null, text2, ResultSeverityLevel.Error, false);
				}
				smtpOutProxySession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text2);
				this.currentAuthMechanism = SmtpAuthenticationMechanism.None;
			}
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			switch (this.currentAuthMechanism)
			{
			case SmtpAuthenticationMechanism.None:
				smtpOutProxySession.AckConnection(AckStatus.Retry, SmtpResponse.AuthTempFailure, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			case SmtpAuthenticationMechanism.Login:
				base.OutboundFormatNegotiateCommand(smtpOutProxySession.Connector.Name, smtpOutProxySession.LogSession, smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN);
				return;
			case SmtpAuthenticationMechanism.ExchangeAuth:
				this.OutboundFormatExchangeAuthCommand();
				return;
			}
			throw new InvalidOperationException(string.Format("Invalid auth mechanism {0}", this.currentAuthMechanism));
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (this.cancelInProgress && statusCode[0] != '5')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "An error occurred during authentication, so we tried to cancel AUTH. However the server did not respond correctly to the cancel request, so the connection will be dropped. The server response was: {0}", base.SmtpResponse);
				throw new FormatException("Tried to cancel AUTH, but server did not exit out of of negotiation");
			}
			if (statusCode[0] == '5' || statusCode[0] == '4')
			{
				smtpOutProxySession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Authentication failed, the response was: {0}.  Quitting.", base.SmtpResponse);
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAuthenticationFailed, null, new object[]
				{
					smtpOutProxySession.Connector.Name,
					smtpOutProxySession.RemoteEndPoint,
					base.SmtpResponse
				});
				smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (statusCode[0] == '2')
			{
				if (this.currentAuthMechanism == SmtpAuthenticationMechanism.ExchangeAuth)
				{
					smtpOutProxySession.LogSession.LogReceive(SmtpResponse.GssapiSmtpResponseToLog235.ToByteArray());
					this.OutboundProcessExchangeAuthResponse(base.SmtpResponse.StatusText[0]);
					return;
				}
				smtpOutProxySession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
				smtpOutProxySession.IsAuthenticated = true;
				smtpOutProxySession.BlindProxySuccessfulInboundResponse = base.SmtpResponse;
				smtpOutProxySession.IsProxying = true;
				return;
			}
			else
			{
				if (base.SmtpResponse.StatusText.Length != 1 || !string.Equals(statusCode, "334", StringComparison.Ordinal))
				{
					smtpOutProxySession.LogSession.LogReceive(base.SmtpResponse.ToByteArray());
					throw new FormatException("AUTH response was illegally formatted : " + base.SmtpResponse.ToString());
				}
				base.ParsingStatus = ParsingStatus.MoreDataRequired;
				smtpOutProxySession.LogSession.LogReceive(SmtpResponse.GssapiSmtpResponseToLog334.ToByteArray());
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

		protected override void SetCommandStringForAuth()
		{
			base.ProtocolCommandString = "AUTH LOGIN";
		}

		private void OutboundFormatExchangeAuthCommand()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			byte[] array;
			SecurityStatus securityStatus = this.authContext.NegotiateSecurityContext(null, out array);
			if (securityStatus != SecurityStatus.OK)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendOutboundAuthenticationFailed, null, new object[]
				{
					securityStatus,
					smtpOutProxySession.Connector.Name,
					this.currentAuthMechanism,
					"SMTPSVC/" + smtpOutProxySession.AdvertisedEhloOptions.AdvertisedFQDN
				});
				string text = string.Format("Outbound Authentication failed with " + securityStatus.ToString(), new object[0]);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendOutboundAuthenticationFailed", null, text, ResultSeverityLevel.Warning, false);
				smtpOutProxySession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text);
				smtpOutProxySession.FailoverConnection(SmtpResponse.AuthTempFailure, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			BufferBuilder bufferBuilder = new BufferBuilder("X-EXPS EXCHANGEAUTH SHA256 ".Length + array.Length + "\r\n".Length);
			bufferBuilder.Append("X-EXPS EXCHANGEAUTH SHA256 ");
			bufferBuilder.Append(array);
			bufferBuilder.Append("\r\n");
			bufferBuilder.RemoveUnusedBufferSpace();
			base.ProtocolCommand = bufferBuilder.GetBuffer();
			smtpOutProxySession.LogSession.LogSend(Util.AsciiStringToBytesAndAppendCRLF("X-EXPS EXCHANGEAUTH SHA256 "));
		}

		private void OutboundProcessExchangeAuthResponse(string response)
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
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
					smtpOutProxySession.Connector.Name,
					smtpOutProxySession.RemoteEndPoint,
					base.SmtpResponse
				});
				smtpOutProxySession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Outbound Exchange Authentication failed with " + securityStatus.ToString());
				smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			smtpOutProxySession.NextState = SmtpOutSession.SessionState.XProxy;
		}
	}
}
