using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContactStoreForContactLinking
	{
		StoreId[] FolderScope { get; }

		IEnumerable<ContactInfoForLinking> GetPersonContacts(PersonId personId);

		IEnumerable<ContactInfoForLinking> GetAllContacts();

		IEnumerable<ContactInfoForLinking> GetAllContactsPerCriteria(IEnumerable<string> emailAddresses, string imAddress);

		void ContactRemovedFromPerson(PersonId personId, ContactInfoForLinking contact);

		void ContactAddedToPerson(PersonId personId, ContactInfoForLinking contact);
	}
}
