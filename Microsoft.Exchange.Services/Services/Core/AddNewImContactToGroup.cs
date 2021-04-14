using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddNewImContactToGroup
	{
		internal AddNewImContactToGroup(IMailboxSession mailboxSession, string imAddress, string displayName, StoreId groupId, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (imAddress == null)
			{
				throw new ArgumentNullException("imAddress");
			}
			if (imAddress.Length == 0)
			{
				throw new ArgumentException("imAddress was empty");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(mailboxSession, xsoFactory, configuration);
			this.imAddress = imAddress;
			this.displayName = (string.IsNullOrEmpty(displayName) ? imAddress : displayName);
			this.groupId = groupId;
		}

		internal PersonId Execute()
		{
			StoreObjectId contactId;
			PersonId result;
			this.unifiedContactStoreUtilities.RetrieveOrCreateContact(this.imAddress, this.displayName, out contactId, out result);
			this.unifiedContactStoreUtilities.AddContactToGroup(contactId, this.displayName, this.groupId);
			return result;
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly string imAddress;

		private readonly string displayName;

		private readonly StoreId groupId;
	}
}
