using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class ReadOnlyMailRecipientCollectionWrapper : ReadOnlyEnvelopeRecipientCollection
	{
		public ReadOnlyMailRecipientCollectionWrapper(IList<MailRecipient> recipients, IReadOnlyMailItem mailItem)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			this.recipients = recipients;
			this.mailItem = mailItem;
		}

		public override int Count
		{
			get
			{
				return this.recipients.Count;
			}
		}

		public override EnvelopeRecipient this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException(Strings.IndexOutOfBounds(index, this.Count));
				}
				MailRecipient recipientObject = this.recipients[index];
				return this.CreateRecipientWrapper(recipientObject);
			}
		}

		public override bool Contains(RoutingAddress address)
		{
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				if (mailRecipient.Email.Equals(address))
				{
					return true;
				}
			}
			return false;
		}

		public override EnvelopeRecipientCollection.Enumerator GetEnumerator()
		{
			return new EnvelopeRecipientCollection.Enumerator(this.recipients, new Converter<object, EnvelopeRecipient>(this.CreateRecipientWrapper));
		}

		private MailRecipientWrapper CreateRecipientWrapper(object recipientObject)
		{
			return new MailRecipientWrapper((MailRecipient)recipientObject, this.mailItem);
		}

		private IList<MailRecipient> recipients;

		private IReadOnlyMailItem mailItem;
	}
}
