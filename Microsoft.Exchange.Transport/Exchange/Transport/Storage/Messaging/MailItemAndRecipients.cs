using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal struct MailItemAndRecipients
	{
		public MailItemAndRecipients(IMailItemStorage mailItem, IEnumerable<IMailRecipientStorage> recipients)
		{
			this.mailItem = mailItem;
			this.recipients = recipients;
		}

		public IMailItemStorage MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public IEnumerable<IMailRecipientStorage> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		private readonly IMailItemStorage mailItem;

		private readonly IEnumerable<IMailRecipientStorage> recipients;
	}
}
