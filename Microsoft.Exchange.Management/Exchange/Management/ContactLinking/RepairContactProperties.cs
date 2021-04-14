using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ContactLinking
{
	[Cmdlet("Repair", "ContactProperties")]
	public sealed class RepairContactProperties : ContactLinkingBaseCmdLet
	{
		[Parameter(Mandatory = true, Position = 1, ParameterSetName = "DisplayNameParameterSet")]
		public SwitchParameter FixDisplayName { get; set; }

		[Parameter(Mandatory = false, Position = 2, ParameterSetName = "DisplayNameParameterSet")]
		[Parameter(Mandatory = true, Position = 1, ParameterSetName = "ConversationIndexParameterSet")]
		public SwitchParameter FixConversationIndexTracking { get; set; }

		protected override string UserAgent
		{
			get
			{
				return "Client=Management;Action=Repair-ContactProperties";
			}
		}

		internal override void ContactLinkingOperation(MailboxSession mailboxSession)
		{
			ContactsEnumerator<IStorePropertyBag> contactsEnumerator = ContactsEnumerator<IStorePropertyBag>.CreateContactsAndPdlsEnumerator(mailboxSession, DefaultFolderType.AllContacts, RepairContactProperties.ContactPropertiesToEnumerate, (IStorePropertyBag propertyBag) => propertyBag, new XSOFactory());
			foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				base.PerformanceTracker.IncrementContactsRead();
				try
				{
					using (Item item = Item.Bind(mailboxSession, valueOrDefault.ObjectId, RepairContactProperties.ContactPropertiesToBind))
					{
						item.OpenAsReadWrite();
						if (this.FixDisplayName.IsPresent)
						{
							RepairContactProperties.TriggerContactDisplayNamePropertyRule(item);
						}
						if (this.FixConversationIndexTracking.IsPresent)
						{
							RepairContactProperties.TurnOnConversationIndexTracking(item);
						}
						if (item.IsDirty)
						{
							item.Save(SaveMode.NoConflictResolution);
							base.PerformanceTracker.IncrementContactsUpdated();
							if (base.IsVerboseOn)
							{
								base.WriteVerbose(RepairContactProperties.FormatContactForVerboseOutput(this.Identity, item));
							}
						}
					}
				}
				catch (ObjectNotFoundException)
				{
					this.WriteWarning(base.GetErrorMessageObjectNotFound(valueOrDefault.ObjectId.ToBase64String(), typeof(Item).ToString(), mailboxSession.ToString()));
				}
			}
		}

		private static void TriggerContactDisplayNamePropertyRule(Item contactItem)
		{
			string valueOrDefault = contactItem.GetValueOrDefault<string>(StoreObjectSchema.DisplayName, null);
			string propertyValue = Guid.NewGuid().ToString();
			contactItem.SetOrDeleteProperty(StoreObjectSchema.DisplayName, propertyValue);
			contactItem.SetOrDeleteProperty(StoreObjectSchema.DisplayName, valueOrDefault);
		}

		private static void TurnOnConversationIndexTracking(Item contactItem)
		{
			contactItem.SetOrDeleteProperty(ItemSchema.ConversationIndexTracking, true);
		}

		private static LocalizedString FormatContactForVerboseOutput(MailboxIdParameter mailbox, Item contactItem)
		{
			contactItem.Load(new PropertyDefinition[]
			{
				ItemSchema.Id,
				ContactSchema.PersonId,
				StoreObjectSchema.DisplayName
			});
			return new LocalizedString(string.Format(CultureInfo.InvariantCulture, "Updated contact in Mailbox: {0}. EntryID: {1}, PersonId: {2}, DisplayNameFirstLast: '{3}'.", new object[]
			{
				mailbox,
				contactItem.Id.ObjectId,
				contactItem.GetValueOrDefault<PersonId>(ContactSchema.PersonId),
				contactItem.GetValueOrDefault<string>(ContactBaseSchema.DisplayNameFirstLast)
			}));
		}

		private const string DisplayNameParameterSet = "DisplayNameParameterSet";

		private const string ConversationIndexParameterSet = "ConversationIndexParameterSet";

		private const string RepairContactPropertiesUserAgent = "Client=Management;Action=Repair-ContactProperties";

		private static readonly PropertyDefinition[] ContactPropertiesToEnumerate = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] ContactPropertiesToBind = new PropertyDefinition[]
		{
			StoreObjectSchema.DisplayName,
			ItemSchema.ConversationIndexTracking
		};
	}
}
