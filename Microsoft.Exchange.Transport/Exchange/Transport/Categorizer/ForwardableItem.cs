using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ForwardableItem : MailboxItem
	{
		public ForwardableItem(MailRecipient recipient) : base(recipient)
		{
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ForwardingAddress");
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return base.GetProperty<bool>("Microsoft.Exchange.Transport.DirectoryData.DeliverToMailboxAndForward", false);
			}
		}

		protected override bool CheckDeliveryRestrictions(Expansion expansion)
		{
			if (expansion.Resolver.Message.AltRecipientProhibited && this.ForwardingAddress != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "The sender prohibited alt-recipient redirection, recipient {0} will NDR", base.Recipient.Email);
				base.FailRecipient(AckReason.AlternateRecipientBlockedBySender);
				return false;
			}
			return base.CheckDeliveryRestrictions(expansion);
		}

		public override void Allow(Expansion expansion)
		{
			if (this.ForwardingAddress == null)
			{
				return;
			}
			if (this.AlreadyForwarded(expansion))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "redirect handled header is present");
				return;
			}
			string orcpt = null;
			Expansion expansion2;
			if (this.DeliverToMailboxAndForward)
			{
				MessageTemplate template = MessageTemplate.Default;
				if (expansion.MailItem.From != RoutingAddress.NullReversePath)
				{
					template = MessageTemplate.Default.Merge(null, (AutoResponseSuppress)0, null, true, false, expansion.Message.Type == ResolverMessageType.Recall, false);
				}
				expansion2 = base.Expand(expansion, template, HistoryType.DeliveredAndForwarded);
				if (expansion2 != null)
				{
					MessageTemplate template2 = new MessageTemplate(null, (AutoResponseSuppress)0, null, false, true, false, false);
					base.ApplyTemplate(template2);
				}
			}
			else
			{
				orcpt = (base.Recipient.ORcpt ?? ("rfc822;" + base.Recipient.Email));
				expansion2 = base.Expand(expansion, HistoryType.Forwarded);
			}
			if (expansion2 == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "loop detected in alternate recipient case");
				return;
			}
			Result<TransportMiniRecipient> result = expansion.MailItem.ADRecipientCache.FindAndCacheRecipient(this.ForwardingAddress);
			if (result.Error is ObjectValidationError)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "alternate recipient is invalid");
				if (!this.DeliverToMailboxAndForward)
				{
					base.FailRecipient(AckReason.AlternateRecipientInvalid);
				}
				return;
			}
			TransportMiniRecipient data = result.Data;
			if (data == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "alternate recipient not found");
				if (!this.DeliverToMailboxAndForward)
				{
					base.FailRecipient(AckReason.AlternateRecipientNotFound);
				}
				return;
			}
			MailRecipient mailRecipient = null;
			if (expansion.BypassChildModeration || expansion.Message.BypassChildModeration || !expansion.Resolver.ResolverCache.RecipientExists(data.Id.ObjectGuid))
			{
				mailRecipient = expansion2.Add(data, base.Recipient.DsnRequested, orcpt);
				if (mailRecipient == null)
				{
					ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "alternate recipient primary SMTP address missing or invalid");
					if (!this.DeliverToMailboxAndForward)
					{
						base.FailRecipient(AckReason.AlternateRecipientNotFound);
					}
					return;
				}
				OrarGenerator.CopyOrar(base.Recipient, mailRecipient);
				RedirectionHistory.SetRedirectionHistoryOnRecipient(mailRecipient, base.Recipient.Email.ToString());
			}
			if (this.DeliverToMailboxAndForward)
			{
				MsgTrackRedirectInfo msgTrackInfo = new MsgTrackRedirectInfo(base.Recipient.Email, base.Recipient.Email, null);
				MessageTrackingLog.TrackRedirect(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo);
			}
			else
			{
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.ForwardedToAlternateRecipient);
			}
			if (mailRecipient != null)
			{
				MsgTrackRedirectInfo msgTrackInfo2 = new MsgTrackRedirectInfo(base.Recipient.Email, mailRecipient.Email, null);
				MessageTrackingLog.TrackRedirect(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo2);
				return;
			}
			string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(data);
			if (!string.IsNullOrEmpty(primarySmtpAddress))
			{
				MsgTrackRedirectInfo msgTrackInfo3 = new MsgTrackRedirectInfo(base.Recipient.Email, new RoutingAddress(primarySmtpAddress), null);
				MessageTrackingLog.TrackRedirectEvent(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo3, MessageTrackingEvent.DUPLICATEREDIRECT);
			}
		}

		public bool AlreadyForwarded(Expansion expansion)
		{
			if (expansion.Message.RedirectHandled)
			{
				return true;
			}
			if (base.GetProperty<bool>("Microsoft.Exchange.Transport.Legacy.AlreadyForwarded", false))
			{
				MessageTemplate template = new MessageTemplate(null, (AutoResponseSuppress)0, null, false, true, false, false);
				base.ApplyTemplate(template);
				return true;
			}
			return false;
		}

		public const string LegacyAlreadyForwarded = "Microsoft.Exchange.Transport.Legacy.AlreadyForwarded";
	}
}
