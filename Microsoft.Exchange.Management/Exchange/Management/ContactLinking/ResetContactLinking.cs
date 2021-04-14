using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ContactLinking
{
	[Cmdlet("Reset", "ContactLinking")]
	public sealed class ResetContactLinking : ContactLinkingBaseCmdLet
	{
		[Parameter(Mandatory = false, Position = 1)]
		public SwitchParameter IncludeUserApproved { get; set; }

		protected override string UserAgent
		{
			get
			{
				return "Client=Management;Action=Reset-ContactLinking";
			}
		}

		internal override void ContactLinkingOperation(MailboxSession mailboxSession)
		{
			this.UpdateAllItems(mailboxSession, new Predicate<Item>(this.ShouldProcessContact), new Action<Item>(ResetContactLinking.CleanUpContactLinkingInformation));
		}

		private static void CleanUpContactLinkingInformation(Item contactItem)
		{
			contactItem.OpenAsReadWrite();
			contactItem.SetOrDeleteProperty(ContactSchema.PersonId, PersonId.CreateNew());
			contactItem.SetOrDeleteProperty(ContactSchema.Linked, false);
			contactItem.SetOrDeleteProperty(ContactSchema.LinkRejectHistory, null);
			contactItem.SetOrDeleteProperty(ContactSchema.GALLinkState, GALLinkState.NotLinked);
			contactItem.SetOrDeleteProperty(ContactSchema.GALLinkID, null);
			contactItem.SetOrDeleteProperty(ContactSchema.SmtpAddressCache, null);
			contactItem.SetOrDeleteProperty(ContactSchema.AddressBookEntryId, null);
			contactItem.SetOrDeleteProperty(ContactSchema.UserApprovedLink, false);
			contactItem.Save(SaveMode.NoConflictResolution);
		}

		private bool ShouldProcessContact(Item contactItem)
		{
			return this.IncludeUserApproved.IsPresent || !contactItem.GetValueOrDefault<bool>(ContactSchema.UserApprovedLink, false);
		}

		private void UpdateAllItems(MailboxSession mailboxSession, Predicate<Item> filterCriteria, Action<Item> updateAction)
		{
			ContactsEnumerator<IStorePropertyBag> contactsEnumerator = ContactsEnumerator<IStorePropertyBag>.CreateContactsOnlyEnumerator(mailboxSession, DefaultFolderType.AllContacts, ResetContactLinking.ContactPropertiesToEnumerate, (IStorePropertyBag propertyBag) => propertyBag, new XSOFactory());
			foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				base.PerformanceTracker.IncrementContactsRead();
				if (valueOrDefault != null)
				{
					try
					{
						using (Item item = Item.Bind(mailboxSession, valueOrDefault.ObjectId, ResetContactLinking.ContactPropertiesToBind))
						{
							if (filterCriteria(item))
							{
								updateAction(item);
								base.PerformanceTracker.IncrementContactsUpdated();
							}
						}
					}
					catch (ObjectNotFoundException)
					{
						this.WriteWarning(base.GetErrorMessageObjectNotFound(valueOrDefault.ObjectId.ToBase64String(), typeof(Item).ToString(), mailboxSession.ToString()));
					}
				}
			}
		}

		private static readonly PropertyDefinition[] ContactPropertiesToBind = new PropertyDefinition[]
		{
			ContactSchema.GALLinkState,
			ContactSchema.GALLinkID,
			ContactSchema.SmtpAddressCache,
			ContactSchema.AddressBookEntryId,
			ContactSchema.UserApprovedLink,
			ContactSchema.PersonId,
			ContactSchema.Linked,
			ContactSchema.LinkRejectHistory
		};

		private static readonly PropertyDefinition[] ContactPropertiesToEnumerate = new PropertyDefinition[]
		{
			ItemSchema.Id
		};
	}
}
