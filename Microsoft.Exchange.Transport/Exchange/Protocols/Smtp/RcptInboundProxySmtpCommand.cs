using System;
using System.Globalization;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RcptInboundProxySmtpCommand : RcptSmtpCommand
	{
		public RcptInboundProxySmtpCommand(ISmtpSession session, RecipientCorrelator recipientCorrelator, TransportAppConfig transportAppConfig) : base(session, recipientCorrelator, transportAppConfig)
		{
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			inboundProxySmtpOutSession.NumberOfRecipientsAcked++;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<int, int>((long)this.GetHashCode(), "Number of Recipients acked is {0}, attempted is {1}", inboundProxySmtpOutSession.NumberOfRecipientsAcked, inboundProxySmtpOutSession.NumberOfRecipientsAttempted);
			if (inboundProxySmtpOutSession.NextHopConnection == null || inboundProxySmtpOutSession.RoutedMailItem == null)
			{
				base.SetNextState();
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Connection already marked for Failover or the message has already been acked for a non-success status.  Not processing response for the RCPT TO command: {0}", base.SmtpResponse);
				return;
			}
			if (statusCode.Equals("552", StringComparison.OrdinalIgnoreCase) || statusCode.Equals("452", StringComparison.OrdinalIgnoreCase))
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendInboundProxyRecipientLimitsDoNotMatch, inboundProxySmtpOutSession.RemoteEndPoint.Address.ToString(), new object[]
				{
					inboundProxySmtpOutSession.RemoteEndPoint.Address.ToString(),
					inboundProxySmtpOutSession.ProxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo)
				});
			}
			if (statusCode.Equals("421", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Attempting failover. 421 Status code for RCPT command. NextState: QUIT");
				inboundProxySmtpOutSession.FailoverConnection(base.SmtpResponse, false);
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<RoutingAddress, SmtpResponse>((long)this.GetHashCode(), "Recipient {0} failed with {1}, will be retried", base.RecipientAddress, base.SmtpResponse);
				inboundProxySmtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
				inboundProxySmtpOutSession.PrepareForNextMessage(true);
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Recipient: {0} submitted", base.RecipientAddress);
			inboundProxySmtpOutSession.AckRecipient(AckStatus.Success, base.SmtpResponse);
			base.SetNextState();
		}

		protected override void SetNextStateForSuccessfulRecipients()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			if (inboundProxySmtpOutSession.ProxyLayer.IsBdat)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Setting NextState: BDAT");
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Bdat;
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Setting NextState: DATA");
			inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Data;
		}
	}
}
