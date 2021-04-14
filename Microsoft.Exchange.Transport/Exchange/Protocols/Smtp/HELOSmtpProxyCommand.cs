using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class HELOSmtpProxyCommand : SmtpCommand
	{
		public HELOSmtpProxyCommand(ISmtpSession session) : base(session, "HELO", "OnHeloCommand", LatencyComponent.None)
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
			if (smtpOutProxySession.IsClientProxy)
			{
				throw new InvalidOperationException("Helo should not be sent if proxying a client session");
			}
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "HELO " + base.SmtpSession.HelloDomain;
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.TransientError)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "HELO failed with response {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Initiating failover");
				smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "HELO command failed with response {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "The session will be terminated");
				smtpOutProxySession.AckConnection(AckStatus.Retry, base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (!smtpOutProxySession.CheckRequireOorg())
			{
				return;
			}
			smtpOutProxySession.AdvertisedEhloOptions.ParseHeloResponse(base.SmtpResponse);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "HELO command succeeded");
			smtpOutProxySession.IsProxying = true;
			if (!smtpOutProxySession.IsClientProxy)
			{
				SmtpResponse blindProxySuccessfulInboundResponse;
				if (XProxyToSmtpCommand.TryGetInboundXProxyToResponse(this.GetHashCode(), smtpOutProxySession, 2000, base.SmtpResponse, out blindProxySuccessfulInboundResponse))
				{
					smtpOutProxySession.BlindProxySuccessfulInboundResponse = blindProxySuccessfulInboundResponse;
					return;
				}
				smtpOutProxySession.BlindProxySuccessfulInboundResponse = base.SmtpResponse;
			}
		}
	}
}
