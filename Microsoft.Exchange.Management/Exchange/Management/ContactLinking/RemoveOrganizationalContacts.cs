using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ContactLinking
{
	[Cmdlet("Remove", "OrganizationalContacts")]
	public sealed class RemoveOrganizationalContacts : ContactLinkingBaseCmdLet
	{
		protected override string UserAgent
		{
			get
			{
				return "Client=Management;Action=Remove-OrganizationalContacts";
			}
		}

		internal override void ContactLinkingOperation(MailboxSession mailboxSession)
		{
			ContactsEnumerator<IStorePropertyBag> contactsEnumerator = ContactsEnumerator<IStorePropertyBag>.CreateContactsOnlyEnumerator(mailboxSession, DefaultFolderType.AllContacts, RemoveOrganizationalContacts.ContactPropertiesToEnumerate, (IStorePropertyBag propertyBag) => propertyBag, new XSOFactory());
			foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				base.PerformanceTracker.IncrementContactsRead();
				if (valueOrDefault != null && RemoveOrganizationalContacts.ShouldDeleteContact(storePropertyBag))
				{
					try
					{
						mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							valueOrDefault.ObjectId
						});
						base.PerformanceTracker.IncrementContactsUpdated();
					}
					catch (ObjectNotFoundException)
					{
						this.WriteWarning(base.GetErrorMessageObjectNotFound(valueOrDefault.ObjectId.ToBase64String(), typeof(Item).ToString(), mailboxSession.ToString()));
					}
				}
			}
		}

		private static bool ShouldDeleteContact(IStorePropertyBag contactItem)
		{
			string valueOrDefault = contactItem.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			return string.Equals(WellKnownNetworkNames.GAL, valueOrDefault, StringComparison.InvariantCulture);
		}

		private static readonly PropertyDefinition[] ContactPropertiesToEnumerate = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ContactSchema.PartnerNetworkId
		};
	}
}
