using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal class MailboxResubmitHelper : ResubmitHelper
	{
		public MailboxResubmitHelper(string sourceContext) : base(ResubmitReason.Recovery, MessageTrackingSource.SAFETYNET, sourceContext, NextHopSolutionKey.Empty, ExTraceGlobals.MessageResubmissionTracer)
		{
		}

		protected override bool IsDeleted(MailRecipient recipient)
		{
			return false;
		}

		protected override string GetComponentNameForReceivedHeader()
		{
			return "MailboxResubmission";
		}

		protected override TransportMailItem CloneWithoutRecipients(TransportMailItem mailItem)
		{
			TransportMailItem transportMailItem = base.CloneWithoutRecipients(mailItem);
			if (Components.TransportAppConfig.Dumpster.AllowDuplicateDelivery)
			{
				DateHeader dateHeader = transportMailItem.RootPart.Headers.FindFirst(HeaderId.Date) as DateHeader;
				if (dateHeader != null)
				{
					DateTime dateTime = dateHeader.DateTime.AddMinutes(1.0);
					dateHeader.DateTime = dateTime;
				}
			}
			return transportMailItem;
		}

		protected override ResubmitHelper.RecipientAction DetermineRecipientAction(MailRecipient recipient)
		{
			return ResubmitHelper.RecipientAction.Copy;
		}

		protected override void ProcessRecipient(MailRecipient recipient)
		{
			base.ProcessRecipient(recipient);
			recipient.DsnNeeded &= ~DsnFlags.Delay;
			if (recipient.DsnRequested != DsnRequestedFlags.Default)
			{
				recipient.DsnRequested &= ~DsnRequestedFlags.Delay;
				return;
			}
			recipient.DsnRequested = DsnRequestedFlags.Failure;
		}

		internal const string ComponentNameForReceivedHeader = "MailboxResubmission";
	}
}
