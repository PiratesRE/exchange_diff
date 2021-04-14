using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddNewTelUriContactToGroup
	{
		internal AddNewTelUriContactToGroup(IMailboxSession mailboxSession, string telUriAddress, string imContactSipUriAddress, string imTelephoneNumber, StoreId groupId, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (telUriAddress == null)
			{
				throw new ArgumentNullException("telUriAddress");
			}
			if (telUriAddress.Length == 0)
			{
				throw new ArgumentException("telUriAddress was empty");
			}
			if (imContactSipUriAddress == null)
			{
				throw new ArgumentNullException("imContactSipUriAddress");
			}
			if (imContactSipUriAddress.Length == 0)
			{
				throw new ArgumentException("imContactSipUriAddress was empty");
			}
			if (imTelephoneNumber == null)
			{
				throw new ArgumentNullException("imTelephoneNumber");
			}
			if (imTelephoneNumber.Length == 0)
			{
				throw new ArgumentException("imTelephoneNumber was empty");
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
			this.telUriAddress = telUriAddress;
			this.imContactSipUriAddress = imContactSipUriAddress;
			this.imTelephoneNumber = imTelephoneNumber;
			this.groupId = groupId;
		}

		internal PersonId Execute()
		{
			StoreObjectId contactId;
			PersonId result;
			this.unifiedContactStoreUtilities.RetrieveOrCreateTelUriContact(this.telUriAddress, this.imContactSipUriAddress, this.imTelephoneNumber, out contactId, out result);
			this.unifiedContactStoreUtilities.AddContactToGroup(contactId, this.imTelephoneNumber, this.groupId);
			return result;
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly string telUriAddress;

		private readonly string imContactSipUriAddress;

		private readonly string imTelephoneNumber;

		private readonly StoreId groupId;
	}
}
