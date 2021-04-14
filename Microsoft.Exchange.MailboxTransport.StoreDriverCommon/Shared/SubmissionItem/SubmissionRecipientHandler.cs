using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem
{
	internal delegate void SubmissionRecipientHandler(int? recipientType, Recipient recipient, TransportMailItem mailItem, MailRecipient mailRecipient);
}
