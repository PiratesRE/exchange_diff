using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RsetInboundProxySmtpCommand : SmtpCommand
	{
		public RsetInboundProxySmtpCommand(ISmtpSession session) : base(session, "RSET", "OnRsetCommand", LatencyComponent.None)
		{
			base.IsResponseBuffered = true;
			this.CommandEventArgs = new RsetCommandEventArgs();
		}

		internal override void InboundParseCommand()
		{
			throw new InvalidOperationException("This Rset command handler should never be called for an inbound session");
		}

		internal override void InboundProcessCommand()
		{
			throw new InvalidOperationException("This Rset command handler should never be called for an inbound session");
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "RSET";
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			if (inboundProxySmtpOutSession.AdvertisedEhloOptions.XProxyFrom)
			{
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.XProxyFrom;
				ExTraceGlobals.ServiceTracer.TraceDebug((long)this.GetHashCode(), "Setting Next State: XProxyFrom");
				return;
			}
			if (!inboundProxySmtpOutSession.BetweenMessagesRset)
			{
				throw new InvalidOperationException("Error, unexpected call to RSET");
			}
			inboundProxySmtpOutSession.BetweenMessagesRset = false;
			if (inboundProxySmtpOutSession.RoutedMailItem == null)
			{
				throw new InvalidOperationException("Must call PrepareForNextMessage when issuing RSET between messages");
			}
			inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.MessageStart;
			ExTraceGlobals.ServiceTracer.TraceDebug((long)this.GetHashCode(), "Setting Next State: MessageStart");
		}
	}
}
