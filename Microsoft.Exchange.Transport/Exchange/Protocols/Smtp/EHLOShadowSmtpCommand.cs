using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class EHLOShadowSmtpCommand : EHLOSmtpCommand
	{
		public EHLOShadowSmtpCommand(ISmtpSession session, ITransportConfiguration transportConfiguration) : base(session, transportConfiguration)
		{
		}

		internal override void InboundParseCommand()
		{
			throw new NotSupportedException();
		}

		internal override void InboundProcessCommand()
		{
			throw new NotSupportedException();
		}

		internal override void OutboundProcessResponse()
		{
			ShadowSmtpOutSession shadowSmtpOutSession = (ShadowSmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string, SmtpResponse>((long)this.GetHashCode(), "EHLOShadowSmtpCommand.OutboundProcessResponse. Status Code: {0} Response {1}", statusCode, base.SmtpResponse);
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO failed with response {0}", base.SmtpResponse);
				shadowSmtpOutSession.FailoverConnection(base.SmtpResponse);
				shadowSmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			shadowSmtpOutSession.AdvertisedEhloOptions.ParseResponse(base.SmtpResponse, shadowSmtpOutSession.RemoteEndPoint.Address);
			if (!shadowSmtpOutSession.AdvertisedEhloOptions.XShadowRequest)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "EHLO response did not advertise XSHADOWREQUEST, failing over");
				shadowSmtpOutSession.FailoverConnection(base.SmtpResponse);
				shadowSmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			base.OutboundProcessResponse();
		}
	}
}
