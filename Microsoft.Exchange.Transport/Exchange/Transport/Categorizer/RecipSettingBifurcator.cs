using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RecipSettingBifurcator : IMailBifurcationHelper<TemplateWithHistory>
	{
		public RecipSettingBifurcator(TransportMailItem mailItem)
		{
			this.mailItem = mailItem;
			this.message = new ResolverMessage(mailItem.Message, mailItem.MimeSize);
		}

		public bool GetBifurcationInfo(MailRecipient recipient, out TemplateWithHistory template)
		{
			template = null;
			if (!recipient.IsActive || recipient.Status == Status.Complete)
			{
				return false;
			}
			TemplateWithHistory templateWithHistory = TemplateWithHistory.ReadFrom(recipient);
			if (recipient.IsProcessed)
			{
				templateWithHistory.Template = templateWithHistory.Template.Derive(MessageTemplate.StripHistory);
			}
			templateWithHistory.Normalize(this.message);
			if (templateWithHistory.Equals(TemplateWithHistory.Default))
			{
				return false;
			}
			template = templateWithHistory;
			return true;
		}

		public TransportMailItem GenerateNewMailItem(IList<MailRecipient> newMailItemRecipients, TemplateWithHistory bifurcationInfo)
		{
			TransportMailItem transportMailItem = this.mailItem.NewCloneWithoutRecipients();
			foreach (MailRecipient mailRecipient in newMailItemRecipients)
			{
				mailRecipient.MoveTo(transportMailItem);
			}
			bifurcationInfo.ApplyTo(transportMailItem);
			transportMailItem.CommitLazy();
			MessageTrackingLog.TrackTransfer(MessageTrackingSource.ROUTING, transportMailItem, this.mailItem.RecordId, "Resolver");
			if (Resolver.PerfCounters != null)
			{
				Resolver.PerfCounters.MessagesCreatedTotal.Increment();
			}
			return transportMailItem;
		}

		public bool NeedsBifurcation()
		{
			return true;
		}

		private TransportMailItem mailItem;

		private ResolverMessage message;
	}
}
