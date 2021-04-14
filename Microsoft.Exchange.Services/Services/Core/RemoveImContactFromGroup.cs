using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoveImContactFromGroup
	{
		internal RemoveImContactFromGroup(IMailboxSession mailboxSession, StoreId contactId, StoreId groupId, IXSOFactory xsoFactory)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (contactId == null)
			{
				throw new ArgumentNullException("contactId");
			}
			if (groupId == null)
			{
				throw new ArgumentNullException("groupId");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.contactId = contactId;
			this.groupId = groupId;
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(mailboxSession, xsoFactory);
		}

		internal void Execute()
		{
			this.unifiedContactStoreUtilities.RemoveContactFromGroup(this.contactId, this.groupId);
		}

		private readonly StoreId contactId;

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private StoreId groupId;
	}
}
