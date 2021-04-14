using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RsetShadowSmtpCommand : RsetSmtpCommand
	{
		public RsetShadowSmtpCommand(ISmtpSession session) : base(session)
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
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (!smtpOutSession.AdvertisedEhloOptions.XShadowRequest)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "EHLO response did not advertise XSHADOWREQUEST, failing over");
				smtpOutSession.FailoverConnection(base.SmtpResponse);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			smtpOutSession.NextState = SmtpOutSession.SessionState.XShadowRequest;
			ExTraceGlobals.ServiceTracer.TraceDebug((long)this.GetHashCode(), "Setting Next State: XShadowRequest");
		}
	}
}
