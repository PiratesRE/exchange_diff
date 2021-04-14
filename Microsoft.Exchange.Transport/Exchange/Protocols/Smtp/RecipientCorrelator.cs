using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RecipientCorrelator : IEnumerable<MailRecipient>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.recipientList.Count;
			}
		}

		public List<MailRecipient> Recipients
		{
			get
			{
				return this.recipientList;
			}
		}

		public virtual void Add(MailRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("Can't add a null recipient to the recipient correlator");
			}
			this.recipientList.Add(recipient);
		}

		public MailRecipient Find(RoutingAddress recipientAddress)
		{
			foreach (MailRecipient mailRecipient in this.recipientList)
			{
				if (mailRecipient.Email == recipientAddress)
				{
					return mailRecipient;
				}
			}
			return null;
		}

		public IEnumerator<MailRecipient> GetEnumerator()
		{
			return this.recipientList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected List<MailRecipient> recipientList = new List<MailRecipient>();
	}
}
