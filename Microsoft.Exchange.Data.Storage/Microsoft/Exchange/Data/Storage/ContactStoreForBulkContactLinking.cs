using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactStoreForBulkContactLinking : ContactStoreForContactLinking
	{
		public ContactStoreForBulkContactLinking(MailboxSession mailboxSession, IContactLinkingPerformanceTracker performanceTracker) : base(mailboxSession, performanceTracker)
		{
		}

		public override IEnumerable<ContactInfoForLinking> GetAllContactsPerCriteria(IEnumerable<string> emailAddresses, string imAddress)
		{
			throw new InvalidOperationException("BulkContactLinking does not implement GetAllContactsPerCriteria.");
		}

		public override IEnumerable<ContactInfoForLinking> GetPersonContacts(PersonId personId)
		{
			Util.ThrowOnNullArgument(personId, "personId");
			if (!this.initializedWorkingSet)
			{
				this.InitializeWorkingSet();
			}
			IList<ContactInfoForLinking> result;
			if (!this.contactsByPersonId.TryGetValue(personId, out result))
			{
				ContactStoreForBulkContactLinking.Tracer.TraceDebug<PersonId>((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.GetPersonContacts: Couldn't find contact list for PersonId: {0}", personId);
				result = new List<ContactInfoForLinking>(0);
			}
			return result;
		}

		public override IEnumerable<ContactInfoForLinking> GetAllContacts()
		{
			if (!this.initializedWorkingSet)
			{
				this.InitializeWorkingSet();
			}
			return this.workingSet;
		}

		public override void ContactRemovedFromPerson(PersonId personId, ContactInfoForLinking contact)
		{
			Util.ThrowOnNullArgument(personId, "personId");
			Util.ThrowOnNullArgument(contact, "contact");
			if (contact is ContactInfoForLinkingFromCoreObject)
			{
				ContactStoreForBulkContactLinking.Tracer.TraceDebug<VersionedId, PersonId>((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.ContactRemovedFromPerson: contact not removed from contactsByPersonId as it is object being saved. ItemId {0}, PersonId {1}", contact.ItemId, contact.PersonId);
				return;
			}
			if (!this.initializedWorkingSet)
			{
				this.InitializeWorkingSet();
			}
			IList<ContactInfoForLinking> list;
			if (this.contactsByPersonId.TryGetValue(personId, out list))
			{
				list.Remove(contact);
				return;
			}
			ContactStoreForBulkContactLinking.Tracer.TraceDebug<PersonId>((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.ReportPersonIdUpdate: Couldn't find contact list for PersonId: {0}", personId);
		}

		public override void ContactAddedToPerson(PersonId personId, ContactInfoForLinking contact)
		{
			Util.ThrowOnNullArgument(personId, "personId");
			Util.ThrowOnNullArgument(contact, "contact");
			if (!personId.Equals(contact.PersonId))
			{
				ContactStoreForBulkContactLinking.Tracer.TraceDebug<PersonId, PersonId>((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.ContactAddedToPerson: PersonId of the contact is not properly set. Found {0}. Expected {1}", contact.PersonId, personId);
				throw new InvalidOperationException("ContactStoreForBulkContactLinking.ContactAddedToPerson: PersonId of the contact is not properly set.");
			}
			if (contact is ContactInfoForLinkingFromCoreObject)
			{
				ContactStoreForBulkContactLinking.Tracer.TraceDebug<VersionedId, PersonId>((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.ContactAddedToPerson: contact not added to contactsByPersonId as it is object being saved. ItemId {0}, PersonId {1}", contact.ItemId, contact.PersonId);
				return;
			}
			if (!this.initializedWorkingSet)
			{
				this.InitializeWorkingSet();
			}
			this.AddToContactsByPersonId(contact);
		}

		public void PushContactOntoWorkingSet(IStorePropertyBag contact)
		{
			Util.ThrowOnNullArgument(contact, "contact");
			if (!this.initializedWorkingSet)
			{
				this.InitializeWorkingSet();
			}
			ContactInfoForLinking contact2 = base.CreateContactInfoForLinking(contact);
			this.AddContactToWorkingSet(contact2);
		}

		private void InitializeWorkingSet()
		{
			this.contactsByPersonId = new Dictionary<PersonId, IList<ContactInfoForLinking>>(100);
			this.workingSet = new List<ContactInfoForLinking>(100);
			ContactsEnumerator<ContactInfoForLinking> contactsEnumerator = ContactsEnumerator<ContactInfoForLinking>.CreateContactsOnlyEnumerator(this.MailboxSession, DefaultFolderType.MyContactsExtended, ContactInfoForLinking.Properties, new Func<IStorePropertyBag, ContactInfoForLinking>(base.CreateContactInfoForLinking), new XSOFactory());
			foreach (ContactInfoForLinking contact in contactsEnumerator)
			{
				this.AddContactToWorkingSet(contact);
				if (this.workingSet.Count >= 5000)
				{
					ContactStoreForBulkContactLinking.Tracer.TraceDebug((long)this.GetHashCode(), "ContactStoreForBulkContactLinking.InitializeWorkingSet: reached contacts cap. Skipping remaining contacts in mailbox.");
					break;
				}
			}
			this.initializedWorkingSet = true;
		}

		private void AddContactToWorkingSet(ContactInfoForLinking contact)
		{
			this.workingSet.Add(contact);
			this.AddToContactsByPersonId(contact);
		}

		private void AddToContactsByPersonId(ContactInfoForLinking contact)
		{
			IList<ContactInfoForLinking> list;
			if (!this.contactsByPersonId.TryGetValue(contact.PersonId, out list))
			{
				list = new List<ContactInfoForLinking>(2);
				this.contactsByPersonId[contact.PersonId] = list;
			}
			list.Add(contact);
		}

		private const int MaxContactsLoadedFromMailbox = 5000;

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private bool initializedWorkingSet;

		private IList<ContactInfoForLinking> workingSet;

		private Dictionary<PersonId, IList<ContactInfoForLinking>> contactsByPersonId;
	}
}
