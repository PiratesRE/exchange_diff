using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ContactStoreForContactLinking : IContactStoreForContactLinking
	{
		public ContactStoreForContactLinking(MailboxSession mailboxSession, IContactLinkingPerformanceTracker performanceTracker)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(performanceTracker, "performanceTracker");
			this.MailboxSession = mailboxSession;
			this.performanceTracker = performanceTracker;
		}

		public StoreId[] FolderScope
		{
			get
			{
				if (this.folderScope == null)
				{
					StoreObjectId[] value = this.MailboxSession.ContactFolders.MyContactFolders.Value;
					if (value == null || value.Length == 0)
					{
						this.folderScope = ContactsSearchFolderCriteria.MyContactsExtended.GetDefaultFolderScope(this.MailboxSession, false);
					}
					else
					{
						this.folderScope = ContactsSearchFolderCriteria.GetMyContactExtendedFolders(this.MailboxSession, value, false);
					}
				}
				return this.folderScope;
			}
		}

		public abstract IEnumerable<ContactInfoForLinking> GetPersonContacts(PersonId personId);

		public abstract IEnumerable<ContactInfoForLinking> GetAllContacts();

		public abstract IEnumerable<ContactInfoForLinking> GetAllContactsPerCriteria(IEnumerable<string> emailAddresses, string imAddress);

		public abstract void ContactRemovedFromPerson(PersonId personId, ContactInfoForLinking contact);

		public abstract void ContactAddedToPerson(PersonId personId, ContactInfoForLinking contact);

		protected ContactInfoForLinking CreateContactInfoForLinking(IStorePropertyBag propertyBag)
		{
			this.performanceTracker.IncrementContactsRead();
			return ContactInfoForLinkingFromPropertyBag.Create(this.MailboxSession, propertyBag);
		}

		protected readonly MailboxSession MailboxSession;

		private StoreId[] folderScope;

		private IContactLinkingPerformanceTracker performanceTracker;
	}
}
