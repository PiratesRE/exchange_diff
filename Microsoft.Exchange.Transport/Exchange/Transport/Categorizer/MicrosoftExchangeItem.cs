using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MicrosoftExchangeItem : ForwardableItem
	{
		public MicrosoftExchangeItem(MailRecipient recipient) : base(recipient)
		{
		}

		public override void Allow(Expansion expansion)
		{
			if (base.ForwardingAddress == null || base.DeliverToMailboxAndForward)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Suppressing 'Microsoft Exchange' recipient '{0}'.", base.Recipient.Email);
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.MicrosoftExchangeRecipientSuppressed);
				MsgTrackExpandInfo msgTrackInfo = new MsgTrackExpandInfo(base.Recipient.Email, null, base.Recipient.SmtpResponse.ToString());
				MessageTrackingLog.TrackExpand<MailRecipient>(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo, new List<MailRecipient>());
				return;
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Forwarding to alt recipient for 'Microsoft Exchange' recipient '{0}'.", base.Recipient.Email);
			base.Allow(expansion);
		}
	}
}
