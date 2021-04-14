using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactStoreForCowContactLinking : ContactStoreForContactLinking
	{
		public ContactStoreForCowContactLinking(MailboxSession mailboxSession, IContactLinkingPerformanceTracker performanceTracker) : base(mailboxSession, performanceTracker)
		{
		}

		public override void ContactRemovedFromPerson(PersonId personId, ContactInfoForLinking contact)
		{
		}

		public override void ContactAddedToPerson(PersonId personId, ContactInfoForLinking contact)
		{
		}

		public override IEnumerable<ContactInfoForLinking> GetPersonContacts(PersonId personId)
		{
			Util.ThrowOnNullArgument(personId, "personId");
			List<ContactInfoForLinking> list = new List<ContactInfoForLinking>(10);
			IEnumerable<IStorePropertyBag> enumerable = AllPersonContactsEnumerator.Create(this.MailboxSession, personId, ContactInfoForLinking.Properties);
			foreach (IStorePropertyBag propertyBag in enumerable)
			{
				list.Add(base.CreateContactInfoForLinking(propertyBag));
			}
			return list;
		}

		public override IEnumerable<ContactInfoForLinking> GetAllContacts()
		{
			return ContactsEnumerator<ContactInfoForLinking>.CreateContactsOnlyEnumerator(this.MailboxSession, DefaultFolderType.MyContactsExtended, ContactInfoForLinking.Properties, new Func<IStorePropertyBag, ContactInfoForLinking>(base.CreateContactInfoForLinking), new XSOFactory());
		}

		public override IEnumerable<ContactInfoForLinking> GetAllContactsPerCriteria(IEnumerable<string> emailAddresses, string imAddress)
		{
			List<ContactInfoForLinking> list = new List<ContactInfoForLinking>(10);
			IEnumerable<ContactInfoForLinking> result;
			using (IFolder folder = new XSOFactory().BindToFolder(this.MailboxSession, DefaultFolderType.MyContacts))
			{
				IEnumerable<IStorePropertyBag> enumerable = new ContactsByEmailAddressEnumerator(folder, ContactInfoForLinking.Properties, emailAddresses);
				foreach (IStorePropertyBag propertyBag in enumerable)
				{
					list.Add(base.CreateContactInfoForLinking(propertyBag));
				}
				enumerable = new ContactsByPropertyValueEnumerator(folder, InternalSchema.IMAddress, this.GetIMAddressWithVariation(imAddress), ContactInfoForLinking.Properties);
				foreach (IStorePropertyBag propertyBag2 in enumerable)
				{
					list.Add(base.CreateContactInfoForLinking(propertyBag2));
				}
				result = list;
			}
			return result;
		}

		private IEnumerable<string> GetIMAddressWithVariation(string imAddress)
		{
			if (imAddress.StartsWith("sip:", StringComparison.OrdinalIgnoreCase))
			{
				return new string[]
				{
					imAddress,
					imAddress.Substring("sip:".Length)
				};
			}
			return new string[]
			{
				imAddress,
				"sip:" + imAddress
			};
		}
	}
}
