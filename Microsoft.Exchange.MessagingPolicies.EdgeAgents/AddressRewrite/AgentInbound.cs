using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal sealed class AgentInbound : SmtpReceiveAgent
	{
		internal AgentInbound(SmtpServer server)
		{
			this.server = server;
			this.currentConfig = Configuration.Current;
			base.OnRcptCommand += this.RewriteRecipient;
			base.OnEndOfHeaders += this.RewriteP2Addresses;
		}

		private void RewriteRecipient(ReceiveCommandEventSource source, RcptCommandEventArgs args)
		{
			if (this.currentConfig == null)
			{
				ExTraceGlobals.AddressRewritingTracer.TraceError((long)this.GetHashCode(), "Rejecting recipient as we have no configuration");
				source.RejectCommand(SmtpResponse.ServiceUnavailable);
				return;
			}
			MailItem mailItem = args.MailItem;
			if (args.SmtpSession.AuthenticationSource != AuthenticationSource.Anonymous && RewriteHelper.IsSenderInternal(mailItem, this.server))
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "RCPT TO:InboundAddressRewrite skipped as sender was internal, and the message is therefore going Outbound");
				return;
			}
			ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Processing RCPT TO: {0} for message", args.RecipientAddress);
			string text = this.currentConfig.RewriteInbound(args.RecipientAddress);
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress, string>((long)this.GetHashCode(), "Rewriting RCPT TO: {0} -> {1}", args.RecipientAddress, text);
				args.RecipientAddress = RoutingAddress.Parse(text);
			}
		}

		private void RewriteP2Addresses(ReceiveMessageEventSource source, EndOfHeadersEventArgs args)
		{
			if (ExTraceGlobals.AddressRewritingTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (args.MailItem.Message != null && args.MailItem.Message.MessageId != null)
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<string>((long)this.GetHashCode(), "MessageId: {0}, Address Rewriting this Message", args.MailItem.Message.MessageId);
				}
				else
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "MessageId: <null>, Address Rewriting this Message");
				}
			}
			if (args.SmtpSession.AuthenticationSource != AuthenticationSource.Anonymous && RewriteHelper.IsSenderInternal(args.MailItem, this.server))
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "EOH:InboundAddressRewrite skipped as sender was internal, and the message is therefore going Outbound");
				return;
			}
			ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "Rewriting P2 addresses for message");
			HeaderList headers = args.Headers;
			try
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "Rewriting the To: header");
				RewriteHelper.RewriteHeader(headers.FindFirst(HeaderId.To), this.currentConfig, MapTable.MapEntryType.External);
				ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "Rewriting the Cc: header");
				RewriteHelper.RewriteHeader(headers.FindFirst(HeaderId.Cc), this.currentConfig, MapTable.MapEntryType.External);
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.AddressRewritingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to rewrite message headers during Inbound address-rewriting. The message may be malformed. Exception message: {0}", ex.ToString());
				source.RejectMessage(SmtpResponse.InvalidContent);
			}
		}

		private SmtpServer server;

		private Configuration currentConfig;
	}
}
