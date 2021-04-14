using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XRsetProxyToSmtpCommand : SmtpCommand
	{
		public XRsetProxyToSmtpCommand(ISmtpSession session) : base(session, "XRSETPROXYTO", null, LatencyComponent.None)
		{
		}

		internal override void InboundParseCommand()
		{
			throw new InvalidOperationException("XRSETPROXYTO should not have been created");
		}

		internal override void InboundProcessCommand()
		{
			throw new InvalidOperationException("XRSETPROXYTO should not have been created");
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			int num = Math.Min(smtpOutSession.MessagesSentOverSession, 999);
			base.ProtocolCommandString = string.Format("XRSETPROXYTO {0} {1}XXX", XProxyToSmtpCommand.FormatSessionIdString(smtpOutSession.SessionId), num.ToString("000"));
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command: {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.Success)
			{
				string[] statusText = base.SmtpResponse.StatusText;
				string value = XProxyToSmtpCommand.FormatSessionIdString(smtpOutSession.SessionId);
				if (statusText != null && statusText.Length > 0 && statusText[0].EndsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "XRSETPROXYTO accepted");
					smtpOutSession.XRsetProxyToAccepted = true;
				}
			}
			else
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "XRSETPROXYTO not accepted: {0}", base.SmtpResponse);
			}
			smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
		}
	}
}
