using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class EHLOSmtpProxyCommand : EHLOSmtpCommand
	{
		public EHLOSmtpProxyCommand(ISmtpSession session, ITransportConfiguration transportConfiguration) : base(session, transportConfiguration)
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

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "Processing EHLO response. Response {1}", base.SmtpResponse);
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO command failed with response {0}", base.SmtpResponse);
				if (!smtpOutProxySession.TlsConfiguration.RequireTls && !smtpOutProxySession.IsClientProxy)
				{
					smtpOutProxySession.NextState = SmtpOutSession.SessionState.Helo;
					return;
				}
				SmtpResponse requireEhloToSendMail = SmtpResponse.RequireEhloToSendMail;
				smtpOutProxySession.FailoverConnection(requireEhloToSendMail, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			else
			{
				if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.TransientError)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO command failed with response {0}", base.SmtpResponse);
					smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
					smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO command failed, the response was {0}", base.SmtpResponse);
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "The session will be terminated");
					smtpOutProxySession.AckConnection(AckStatus.Retry, base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
					smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				smtpOutProxySession.AdvertisedEhloOptions.ParseResponse(base.SmtpResponse, smtpOutProxySession.RemoteEndPoint.Address);
				this.DetermineNextStateForProxySession();
				return;
			}
		}

		public static void DetermineNextStateForClientProxySession(SmtpOutProxySession session, ITransportConfiguration transportConfiguration, int hashCode)
		{
			if (transportConfiguration.AppConfig.SmtpProxyConfiguration.RequireXProxyExtension && !session.AdvertisedEhloOptions.XProxy)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, "Unable to proxy because XPROXY is not advertised");
				session.FailoverConnection(SmtpResponse.RequireXProxy, SessionSetupFailureReason.ProtocolError);
				session.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (session.SecureState == SecureState.AnonymousTls && !session.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, "We require X-EXPS ExchangeAuth but that was not offered.");
				session.FailoverConnection(SmtpResponse.CannotExchangeAuthenticate, SessionSetupFailureReason.ProtocolError);
				session.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (!session.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("AUTH LOGIN"))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, "Unable to proxy because remote doesn't advertise AUTH LOGIN. Drop session");
				session.FailoverConnection(SmtpResponse.RequireBasicAuthentication, SessionSetupFailureReason.ProtocolError);
				session.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			EhloOptions other = (EhloOptions)session.AdvertisedEhloOptions;
			string text;
			string text2;
			if (!session.InSession.AdvertisedEhloOptions.MatchForClientProxySession(other, out text, out text2))
			{
				string text3 = string.Format("The Ehlo options for the client proxy target {0} did not match while setting up proxy for user {1} on inbound session {2}. The critical non-matching options were <{3}>. The non-critical non-matching options were <{4}>. The session will be dropped.", new object[]
				{
					session.Connection.RemoteEndPoint.Address.ToString(),
					session.InSession.ProxyUserName,
					session.InSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					string.IsNullOrEmpty(text2) ? "NONE" : text2,
					string.IsNullOrEmpty(text) ? "NONE" : text
				});
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text3);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, text3);
				session.FailoverConnection(SmtpResponse.RequireMatchingEhloOptions, SessionSetupFailureReason.ProtocolError);
				session.NextState = SmtpOutSession.SessionState.Quit;
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendProxyEhloOptionsDoNotMatch, "ClientProxy", new object[]
				{
					session.Connection.RemoteEndPoint.Address.ToString(),
					session.InSession.ProxyUserName,
					session.InSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					text2,
					text
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpProxyEhloOptionsDoNotMatchStopProxying", null, text3, ResultSeverityLevel.Warning, false);
				return;
			}
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				string text4 = string.Format("The Ehlo options for the client proxy target {0} did not match while setting up proxy for user {1} on inbound session {2}. The critical non-matching options were <{3}>. The non-critical non-matching options were <{4}>. Client proxying will continue.", new object[]
				{
					session.Connection.RemoteEndPoint.Address.ToString(),
					session.InSession.ProxyUserName,
					session.InSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					string.IsNullOrEmpty(text2) ? "NONE" : text2,
					string.IsNullOrEmpty(text) ? "NONE" : text
				});
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text4);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, text4);
				if (!string.IsNullOrEmpty(text2))
				{
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendProxyEhloOptionsDoNotMatchButStillContinueProxying, "ClientProxy", new object[]
					{
						session.Connection.RemoteEndPoint.Address.ToString(),
						session.InSession.ProxyUserName,
						session.InSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
						text2,
						text
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpProxyEhloOptionsDoNotMatchContinueProxying", null, text4, ResultSeverityLevel.Warning, false);
				}
			}
			if (session.SecureState == SecureState.AnonymousTls)
			{
				session.NextState = SmtpOutSession.SessionState.Exps;
				return;
			}
			if (transportConfiguration.AppConfig.SmtpProxyConfiguration.RequireXProxyExtension)
			{
				session.NextState = SmtpOutSession.SessionState.XProxy;
				return;
			}
			session.SetNextStateToAuthLogin();
		}

		private void DetermineNextStateForProxySession()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			if (smtpOutProxySession.SecureState != SecureState.StartTls && smtpOutProxySession.SecureState != SecureState.AnonymousTls)
			{
				if (smtpOutProxySession.Connector.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer)
				{
					if (smtpOutProxySession.AdvertisedEhloOptions.AnonymousTLS)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, we require X-ANONYMOUSTLS and will try to establish it");
						smtpOutProxySession.NextState = SmtpOutSession.SessionState.AnonymousTLS;
						return;
					}
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "We require X-ANONYMOUSTLS, but remote doesn't advertise it");
					smtpOutProxySession.FailoverConnection(SmtpResponse.RequireAnonymousTlsToSendMail, SessionSetupFailureReason.ProtocolError);
					smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				else if (smtpOutProxySession.TlsConfiguration.RequireTls)
				{
					if (smtpOutProxySession.AdvertisedEhloOptions.StartTLS)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, we require TLS, will try to starttls");
						smtpOutProxySession.NextState = SmtpOutSession.SessionState.StartTLS;
						return;
					}
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "We require TLS, but remote doesn't advertise STARTTLS, will try to fail over session");
					smtpOutProxySession.FailoverConnection(SmtpResponse.RequireTLSToSendMail, SessionSetupFailureReason.ProtocolError);
					smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				else
				{
					if (!smtpOutProxySession.IsClientProxy)
					{
						SmtpResponse blindProxySuccessfulInboundResponse;
						if (XProxyToSmtpCommand.TryGetInboundXProxyToResponse(this.GetHashCode(), smtpOutProxySession, 2000, base.SmtpResponse, out blindProxySuccessfulInboundResponse))
						{
							smtpOutProxySession.BlindProxySuccessfulInboundResponse = blindProxySuccessfulInboundResponse;
						}
						else
						{
							smtpOutProxySession.BlindProxySuccessfulInboundResponse = base.SmtpResponse;
						}
					}
					if (smtpOutProxySession.AdvertisedEhloOptions.StartTLS && !smtpOutProxySession.TlsConfiguration.ShouldSkipTls)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, we will try to starttls opportunistically");
						smtpOutProxySession.NextState = SmtpOutSession.SessionState.StartTLS;
						return;
					}
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Proceeding without tls");
				}
			}
			if (!smtpOutProxySession.CheckRequireOorg())
			{
				return;
			}
			if (smtpOutProxySession.IsClientProxy)
			{
				EHLOSmtpProxyCommand.DetermineNextStateForClientProxySession(smtpOutProxySession, this.transportConfiguration, this.GetHashCode());
				return;
			}
			SmtpResponse blindProxySuccessfulInboundResponse2;
			if (XProxyToSmtpCommand.TryGetInboundXProxyToResponse(this.GetHashCode(), smtpOutProxySession, 2000, base.SmtpResponse, out blindProxySuccessfulInboundResponse2))
			{
				smtpOutProxySession.BlindProxySuccessfulInboundResponse = blindProxySuccessfulInboundResponse2;
			}
			else
			{
				smtpOutProxySession.BlindProxySuccessfulInboundResponse = base.SmtpResponse;
			}
			smtpOutProxySession.IsProxying = true;
		}
	}
}
