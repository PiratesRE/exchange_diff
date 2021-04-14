using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxOwnerFactory
	{
		public MailboxOwnerFactory(IMailboxSession session)
		{
			this.session = session;
		}

		public IMailboxOwner Create()
		{
			if (this.session.IsGroupMailbox())
			{
				return new GroupMailboxOwnerAdapter();
			}
			if (this.session.IsMoveUser)
			{
				return new SystemServiceMailboxOwnerAdapter();
			}
			return new ExchangePrincipalMailboxOwnerAdapter(this.session.MailboxOwner, this.session.MailboxOwner.GetContext(null), this.session.MailboxOwner.RecipientTypeDetails, this.session.LogonType);
		}

		public IMailboxOwner Create(MiniRecipient recipient)
		{
			if (recipient == null)
			{
				return new NullMailboxOwnerAdapter();
			}
			if (this.session.IsGroupMailbox())
			{
				return new GroupMailboxOwnerAdapter();
			}
			RecipientTypeDetails recipientTypeDetails = recipient.RecipientTypeDetails;
			return new MiniRecipientMailboxOwnerAdapter(recipient, recipient.GetContext(null), recipientTypeDetails, this.session.LogonType);
		}

		private readonly IMailboxSession session;
	}
}
