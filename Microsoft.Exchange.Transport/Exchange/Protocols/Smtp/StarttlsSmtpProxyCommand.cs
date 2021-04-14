using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class StarttlsSmtpProxyCommand : StarttlsSmtpCommand
	{
		public StarttlsSmtpProxyCommand(ISmtpSession session, ITransportConfiguration transportConfiguration, bool anonymous) : base(session, anonymous)
		{
			if (transportConfiguration == null)
			{
				throw new ArgumentNullException("transportConfiguration");
			}
			this.transportConfiguration = transportConfiguration;
		}

		internal override void InboundParseCommand()
		{
			throw new NotImplementedException();
		}

		internal override void InboundProcessCommand()
		{
			throw new NotImplementedException();
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = base.ProtocolCommandKeyword;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Issued {0} command", base.ProtocolCommandKeyword);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (string.Equals(statusCode, "220", StringComparison.Ordinal))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} command succeeded, will start TLS negotiation", base.ProtocolCommandKeyword);
				smtpOutProxySession.StartTls(this.anonymous ? SecureState.AnonymousTls : SecureState.StartTls);
				return;
			}
			this.HandleStartTlsErrorsProxy();
		}

		private void HandleStartTlsErrorsProxy()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			if (smtpOutProxySession.TlsConfiguration.RequireTls || this.anonymous)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string, SmtpResponse>((long)this.GetHashCode(), "{0} command failed with response {1}", base.ProtocolCommandKeyword, base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "The session will be terminated");
				smtpOutProxySession.AckConnection(AckStatus.Retry, base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (smtpOutProxySession.CheckRequireOorg())
			{
				if (smtpOutProxySession.IsClientProxy)
				{
					EHLOSmtpProxyCommand.DetermineNextStateForClientProxySession(smtpOutProxySession, this.transportConfiguration, this.GetHashCode());
					return;
				}
				smtpOutProxySession.IsProxying = true;
			}
		}

		private ITransportConfiguration transportConfiguration;
	}
}
