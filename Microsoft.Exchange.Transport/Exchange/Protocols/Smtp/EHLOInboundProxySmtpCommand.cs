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
	internal class EHLOInboundProxySmtpCommand : SmtpCommand
	{
		public EHLOInboundProxySmtpCommand(ISmtpSession session) : base(session, "EHLO", "OnEhloCommand", LatencyComponent.None)
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
			this.helloDomain = base.SmtpSession.HelloDomain;
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "EHLO " + this.helloDomain;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string, SmtpResponse>((long)this.GetHashCode(), "EhloInboundProxySmtpCommand.OutboundProcessResponse. Status Code: {0} Response {1}", statusCode, base.SmtpResponse);
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO failed with response {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Initiating failover");
				inboundProxySmtpOutSession.FailoverConnection(base.SmtpResponse);
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			inboundProxySmtpOutSession.AdvertisedEhloOptions.ParseResponse(base.SmtpResponse, inboundProxySmtpOutSession.RemoteEndPoint.Address);
			if (!inboundProxySmtpOutSession.TlsConfiguration.RequireTls || inboundProxySmtpOutSession.SecureState == SecureState.AnonymousTls || inboundProxySmtpOutSession.SecureState == SecureState.StartTls)
			{
				EhloOptions other = (EhloOptions)inboundProxySmtpOutSession.AdvertisedEhloOptions;
				string text;
				string text2;
				if (!inboundProxySmtpOutSession.ProxyLayer.SmtpInEhloOptions.MatchForInboundProxySession(other, inboundProxySmtpOutSession.ProxyLayer.IsBdat, out text, out text2))
				{
					inboundProxySmtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "EHLO options between current server and proxy target do not match : {0}. Critical non matching options : {1}. Failing over.", new object[]
					{
						text,
						text2
					});
					ExTraceGlobals.SmtpSendTracer.TraceError<string, string>((long)this.GetHashCode(), "EHLO options between current server and proxy target do not match : {0}. Critical non matching options : {1}. Failing over.", text, text2);
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendInboundProxyEhloOptionsDoNotMatch, "InboundProxy", new object[]
					{
						inboundProxySmtpOutSession.RemoteEndPoint.Address.ToString(),
						inboundProxySmtpOutSession.ProxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
						text2,
						text
					});
					inboundProxySmtpOutSession.FailoverConnection(SmtpResponse.EhloOptionsDoNotMatchForProxy);
					inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				if (!string.IsNullOrEmpty(text))
				{
					inboundProxySmtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "EHLO options between current server and proxy target do not match : {0}. Continuing proxy.", new object[]
					{
						text
					});
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "EHLO options between current server and proxy target do not match : {0}. Continuing proxy.", text);
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendInboundProxyNonCriticalEhloOptionsDoNotMatch, "InboundProxy", new object[]
					{
						inboundProxySmtpOutSession.RemoteEndPoint.Address.ToString(),
						inboundProxySmtpOutSession.ProxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
						text
					});
				}
			}
			if (inboundProxySmtpOutSession.TlsConfiguration.RequireTls && !inboundProxySmtpOutSession.AdvertisedEhloOptions.StartTLS)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Connector is configured to send mail only over TLS connections and remote doesn't support TLS");
				string nextHopDomain = inboundProxySmtpOutSession.NextHopDomain;
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendTLSRequiredFailed, inboundProxySmtpOutSession.Connector.Name, new object[]
				{
					inboundProxySmtpOutSession.Connector.Name,
					nextHopDomain,
					inboundProxySmtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN
				});
				string notificationReason = string.Format("Send connector {0} couldn't connect to remote domain {1}. The send connector requires Transport Layer Security (TLS) authentication, but is unable to establish TLS with the receiving server for the remote domain. Check this connector's authentication setting and the EHLO response from the remote server {2}.", inboundProxySmtpOutSession.Connector.Name, nextHopDomain, inboundProxySmtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendTLSRequiredFailed", null, notificationReason, ResultSeverityLevel.Error, false);
				inboundProxySmtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Connector is configured to send mail only over TLS connections and remote doesn't support TLS");
				inboundProxySmtpOutSession.FailoverConnection(SmtpResponse.RequireTLSToSendMail);
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			EHLOSmtpCommand.OutboundAuthNextState outboundAuthNextState = this.EvaluateOutboundAuthRequirements();
			switch (outboundAuthNextState)
			{
			case EHLOSmtpCommand.OutboundAuthNextState.Auth:
			case EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished:
				break;
			case EHLOSmtpCommand.OutboundAuthNextState.Failure:
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			case EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls:
				if (inboundProxySmtpOutSession.AdvertisedEhloOptions.StartTLS && inboundProxySmtpOutSession.SecureState == SecureState.None && !inboundProxySmtpOutSession.TlsConfiguration.ShouldSkipTls)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Will try to establish a TLS session opportunistically");
					inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.StartTLS;
					return;
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Skipping STARTTLS because TLS is not available");
				outboundAuthNextState = EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished;
				break;
			case EHLOSmtpCommand.OutboundAuthNextState.Tls:
				return;
			default:
				throw new InvalidOperationException("Unexpected OutboundAuthNextState value: " + outboundAuthNextState);
			}
			if (outboundAuthNextState == EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished)
			{
				if (inboundProxySmtpOutSession.AdvertisedEhloOptions.XProxyFrom)
				{
					inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.XProxyFrom;
					return;
				}
				inboundProxySmtpOutSession.PrepareNextStateForEstablishedSession();
			}
		}

		private EHLOSmtpCommand.OutboundAuthNextState EvaluateOutboundAuthRequirements()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.EvaluateOutboundAuthRequirements");
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (smtpOutSession.AuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpSendConnectorConfig.AuthMechanisms>((long)this.GetHashCode(), "EHLO command succeeded. Proceeding with opportunistic TLS if advertised.", smtpOutSession.AuthMechanism);
				return EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls;
			}
			if (smtpOutSession.AdvertisedEhloOptions.AnonymousTLS && smtpOutSession.SecureState == SecureState.None)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, try to perform Exchange server authentication using x-anonymoustls");
				smtpOutSession.NextState = SmtpOutSession.SessionState.AnonymousTLS;
				return EHLOSmtpCommand.OutboundAuthNextState.Tls;
			}
			if (smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Will try to authenticate using x-exps exchangeauth.");
				smtpOutSession.NextState = SmtpOutSession.SessionState.Exps;
				return EHLOSmtpCommand.OutboundAuthNextState.Auth;
			}
			if (smtpOutSession.SecureState == SecureState.None)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Connector requires ExchangeServer authentication but we can't achieve it.");
				smtpOutSession.FailoverConnection(SmtpResponse.CannotExchangeAuthenticate);
				return EHLOSmtpCommand.OutboundAuthNextState.Failure;
			}
			return EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished;
		}

		private string helloDomain;
	}
}
