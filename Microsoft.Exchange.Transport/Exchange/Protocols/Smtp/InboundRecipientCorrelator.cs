using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class InboundRecipientCorrelator : RecipientCorrelator
	{
		public override void Add(MailRecipient recipient)
		{
			base.Add(recipient);
			this.hashSet.Add((string)recipient.Email);
		}

		public bool Contains(string recipientAddress)
		{
			return this.hashSet.Contains(recipientAddress);
		}

		public void AddEmpty()
		{
			this.recipientList.Add(null);
		}

		private HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}
}
