using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal static class RewriteHelper
	{
		internal static void RewriteHeader(Header header, Configuration currentConfig, MapTable.MapEntryType entryType)
		{
			if (header == null)
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug(0L, "Empty header, skipping");
				return;
			}
			for (MimeNode mimeNode = header.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
			{
				MimeRecipient mimeRecipient = mimeNode as MimeRecipient;
				if (mimeRecipient == null)
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<string>(0L, "The header '{0}', is either (a) empty (b) contains a recipient-group which we don't handle in rewriting since we handle only flat address-list headers (c) malformed. This header will be skipped", header.Name);
					return;
				}
				string text;
				if (entryType == MapTable.MapEntryType.External)
				{
					text = currentConfig.RewriteInbound((RoutingAddress)mimeRecipient.Email);
				}
				else
				{
					text = currentConfig.RewriteOutbound((RoutingAddress)mimeRecipient.Email);
				}
				if (!string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.AddressRewritingTracer.TraceDebug<string, string>(0L, "Rewritten recipient: {0} -> {1}", mimeRecipient.Email, text);
					mimeRecipient.Email = (string)RoutingAddress.Parse(text);
				}
			}
		}

		internal static void OutboundRewriteList(ICollection<EmailRecipient> recipList, Configuration currentConfig)
		{
			foreach (EmailRecipient recipient in recipList)
			{
				RewriteHelper.OutboundRewriteRecipient(recipient, currentConfig);
			}
		}

		internal static void OutboundRewriteRecipient(EmailRecipient recipient, Configuration currentConfig)
		{
			string text = currentConfig.RewriteOutbound((RoutingAddress)recipient.SmtpAddress);
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.AddressRewritingTracer.TraceDebug<string, string>(0L, "Rewritten recipient, {0} -> {1}", recipient.SmtpAddress, text);
				recipient.SmtpAddress = (string)RoutingAddress.Parse(text);
			}
		}

		internal static bool IsSenderInternal(MailItem mailItem, SmtpServer server)
		{
			object obj = null;
			ExTraceGlobals.AddressRewritingTracer.TraceDebug<RoutingAddress>(0L, "Checking if sender is internal: {0}", mailItem.FromAddress);
			bool flag;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.AddressRewrite.InternalSenderProperty", out obj))
			{
				flag = (bool)obj;
			}
			else
			{
				RoutingAddress fromAddress;
				if (mailItem.FromAddress != RoutingAddress.NullReversePath)
				{
					fromAddress = mailItem.FromAddress;
				}
				else
				{
					if (mailItem.Message == null || mailItem.Message.Sender == null || mailItem.Message.Sender.SmtpAddress == null)
					{
						return false;
					}
					fromAddress = new RoutingAddress(mailItem.Message.Sender.SmtpAddress);
				}
				flag = (server.AcceptedDomains.Find(fromAddress) != null);
				mailItem.Properties["Microsoft.Exchange.AddressRewrite.InternalSenderProperty"] = flag;
			}
			ExTraceGlobals.AddressRewritingTracer.TraceDebug<string>(0L, "Message was submitted by {0} sender", flag ? "internal" : "external");
			return flag;
		}

		private const string InternalSenderProperty = "Microsoft.Exchange.AddressRewrite.InternalSenderProperty";
	}
}
