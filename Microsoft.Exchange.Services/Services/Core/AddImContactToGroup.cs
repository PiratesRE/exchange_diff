using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddImContactToGroup
	{
		internal AddImContactToGroup(IMailboxSession mailboxSession, StoreId contactId, StoreId groupId, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (contactId == null)
			{
				throw new ArgumentNullException("contactId");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.contactId = contactId;
			this.groupId = groupId;
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(mailboxSession, xsoFactory, configuration);
		}

		internal void Execute()
		{
			string displayName = this.unifiedContactStoreUtilities.RetrieveContactDisplayName(this.contactId);
			this.unifiedContactStoreUtilities.AddContactToGroup(this.contactId, displayName, this.groupId);
		}

		private readonly StoreId contactId;

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private StoreId groupId;
	}
}
