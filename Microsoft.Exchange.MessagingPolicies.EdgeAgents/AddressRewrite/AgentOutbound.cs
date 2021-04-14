using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal sealed class AgentOutbound : RoutingAgent
	{
		internal AgentOutbound(SmtpServer server)
		{
			this.server = server;
			this.currentConfig = Configuration.Current;
			base.OnRoutedMessage += this.RewriteMessage;
			base.OnSubmittedMessage += this.FixupRewrittenNdrs;
		}

		private static bool IsAnonymous(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-AuthAs");
			return header == null || string.IsNullOrEmpty(header.Value) || header.Value.Equals("Anonymous", StringComparison.OrdinalIgnoreCase);
		}

		private void FixupRewrittenNdrs(SubmittedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (this.currentConfig == null)
			{
				ExTraceGlobals.AddressRewritingTracer.TraceError((long)this.GetHashCode(), "Address-rewrite configuration not available in OnSubmitted. The AD may be down or unreachable. The message will be put into retry");
				source.Defer(AgentOutbound.deferTimeout);
				return;
			}
			MailItem mailItem = args.MailItem;
			if (mailItem.FromAddress == RoutingAddress.NullReversePath && mailItem.Recipients.Count == 1)
			{
				RoutingAddress address = mailItem.Recipients[0].Address;
				string text = this.currentConfig.RewriteInbound(address);
				if (!string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress, string>((long)this.GetHashCode(), "Fixed up NDR recipient {0} -> {1}", address, text);
					mailItem.Recipients[0].Address = new RoutingAddress(text);
				}
			}
		}

		private void RewriteMessage(RoutedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (this.currentConfig == null)
			{
				return;
			}
			MailItem mailItem = args.MailItem;
			if (ExTraceGlobals.AddressRewritingTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug<string>((long)this.GetHashCode(), "MessageId: {0}, Processing for address-rewrite", mailItem.Message.MessageId);
			}
			try
			{
				if (AgentOutbound.IsAnonymous(args.MailItem.Message.MimeDocument.RootPart.Headers) || !RewriteHelper.IsSenderInternal(args.MailItem, this.server))
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "OutboundAddressRewrite P2 rewrite skipped as sender was external/unauthenticated, and the message is therefore coming Inbound to the Org");
				}
				else
				{
					List<EnvelopeRecipient> list = new List<EnvelopeRecipient>();
					foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
					{
						if (!this.IsInternalAddress(envelopeRecipient.Address))
						{
							ExTraceGlobals.AddressRewritingTracer.TraceDebug<EnvelopeRecipient>((long)this.GetHashCode(), "Recipient {0} is going to the Internet", envelopeRecipient);
							list.Add(envelopeRecipient);
						}
					}
					if (list.Count == 0)
					{
						ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "No recipients are being sent to the Internet, address-rewriting is not required");
					}
					else
					{
						source.Fork(list);
						RoutingAddress fromAddress = mailItem.FromAddress;
						string text = this.currentConfig.RewriteOutbound(fromAddress);
						if (!string.IsNullOrEmpty(text))
						{
							ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress, string>((long)this.GetHashCode(), "Rewrote MAIL FROM address, {0} -> {1}", fromAddress, text);
							mailItem.FromAddress = new RoutingAddress(text);
						}
						ExTraceGlobals.AddressRewritingTracer.TraceDebug((long)this.GetHashCode(), "Rewriting addresses for Internet going recipients");
						HeaderId[] headersToRewrite = new HeaderId[]
						{
							HeaderId.From,
							HeaderId.To,
							HeaderId.Cc,
							HeaderId.Sender,
							HeaderId.ReplyTo,
							HeaderId.ReturnReceiptTo,
							HeaderId.DispositionNotificationTo,
							HeaderId.ResentFrom,
							HeaderId.ResentTo
						};
						this.RewriteHeaders(headersToRewrite, mailItem.Message);
						this.RewriteReturnPath(mailItem.Message.MimeDocument.RootPart.Headers);
					}
				}
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.AddressRewritingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to rewrite headers during Outbound address-rewriting, a MIME-dom exception was hit. The message may be malformed. It will be NDR'ed. Exception: {0}", ex.ToString());
				EnvelopeRecipientCollection recipients = mailItem.Recipients;
				int count = recipients.Count;
				for (int i = count - 1; i >= 0; i--)
				{
					recipients.Remove(recipients[i], DsnType.Failure, SmtpResponse.InvalidContent);
				}
			}
		}

		private void RewriteHeaders(HeaderId[] headersToRewrite, EmailMessage message)
		{
			foreach (HeaderId headerId in headersToRewrite)
			{
				Header[] array = message.MimeDocument.RootPart.Headers.FindAll(headerId);
				if (array == null)
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<HeaderId>((long)this.GetHashCode(), "No '{0}' header found", headerId);
				}
				else
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<int, HeaderId>((long)this.GetHashCode(), "{0} instances of header '{1}' found", array.Length, headerId);
					foreach (Header header in array)
					{
						ExTraceGlobals.AddressRewritingTracer.TraceDebug<string>((long)this.GetHashCode(), "Rewriting '{0}' header", header.Name);
						RewriteHelper.RewriteHeader(header, this.currentConfig, MapTable.MapEntryType.Internal);
					}
				}
			}
		}

		private void RewriteReturnPath(HeaderList messageHeaders)
		{
			AsciiTextHeader asciiTextHeader = messageHeaders.FindFirst(HeaderId.ReturnPath) as AsciiTextHeader;
			if (asciiTextHeader != null)
			{
				char[] trimChars = new char[]
				{
					'<',
					'>',
					' ',
					'\t'
				};
				string address = asciiTextHeader.Value.Trim(trimChars);
				RoutingAddress routingAddress = new RoutingAddress(address);
				if (routingAddress.IsValid)
				{
					string text = this.currentConfig.RewriteOutbound(routingAddress);
					if (!string.IsNullOrEmpty(text))
					{
						ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress, string>((long)this.GetHashCode(), "Rewrote return-path: {0} -> {1}", routingAddress, text);
						asciiTextHeader.Value = string.Format("<{0}>", text);
						return;
					}
				}
				ExTraceGlobals.AddressRewritingTracer.TraceError<string>((long)this.GetHashCode(), "Return path header was invalid: {0}", asciiTextHeader.Value);
			}
		}

		private bool IsInternalAddress(RoutingAddress address)
		{
			string domainPart = address.DomainPart;
			if (string.IsNullOrEmpty(domainPart))
			{
				return false;
			}
			AcceptedDomain acceptedDomain = this.server.AcceptedDomains.Find(domainPart);
			return acceptedDomain != null && acceptedDomain.IsInCorporation;
		}

		private static TimeSpan deferTimeout = new TimeSpan(0, 20, 0);

		private Configuration currentConfig;

		private SmtpServer server;
	}
}
