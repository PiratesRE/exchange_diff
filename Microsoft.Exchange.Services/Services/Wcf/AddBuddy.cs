using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class AddBuddy
	{
		internal AddBuddy(MailboxSession session, Buddy buddy)
		{
			if (string.IsNullOrWhiteSpace(buddy.IMAddress))
			{
				throw new ArgumentException("A non-empty IM address needs to be passed in to add a one-off buddy");
			}
			this.session = session;
			this.buddy = buddy;
		}

		internal void Execute()
		{
			using (Folder folder = Folder.Bind(this.session, DefaultFolderType.Contacts))
			{
				StoreId storeId = BuddyListUtilities.GetSubFolderIdByClass(folder, "IPF.Contact.QuickContacts");
				bool flag = storeId != null;
				if (!flag)
				{
					storeId = this.CreateContactsSubFolder(folder, "Quick Contacts", "IPF.Contact.QuickContacts");
				}
				using (Folder folder2 = Folder.Bind(this.session, storeId))
				{
					if (flag)
					{
						StoreId buddyIdByImAddress = this.GetBuddyIdByImAddress(folder2);
						if (buddyIdByImAddress != null)
						{
							return;
						}
					}
					Participant participant = this.CreateOneOffBuddy(storeId);
					this.AddToBuddyList(participant, folder);
				}
			}
		}

		private static StoreId GetOtherContactsDLId(Folder buddyListFolder)
		{
			StoreId result = null;
			using (QueryResult queryResult = buddyListFolder.ItemQuery(ItemQueryType.None, null, new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
			}, new StorePropertyDefinition[]
			{
				ItemSchema.Id
			}))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.DistList.MOC.OtherContacts")))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags.Length > 0)
					{
						result = (StoreId)propertyBags[0].TryGetProperty(ItemSchema.Id);
					}
				}
			}
			return result;
		}

		private StoreId GetBuddyIdByImAddress(Folder quickContactsFolder)
		{
			StoreId result = null;
			using (QueryResult queryResult = quickContactsFolder.ItemQuery(ItemQueryType.None, null, new SortBy[]
			{
				new SortBy(ContactSchema.IMAddress, SortOrder.Ascending)
			}, new PropertyDefinition[]
			{
				ItemSchema.Id,
				ContactSchema.IMAddress
			}))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.IMAddress, this.buddy.IMAddress)))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags.Length > 0)
					{
						result = (StoreId)propertyBags[0].TryGetProperty(ItemSchema.Id);
					}
				}
			}
			return result;
		}

		private Participant CreateOneOffBuddy(StoreId quickContactsFolderId)
		{
			Participant result;
			using (Contact contact = Contact.Create(this.session, quickContactsFolderId))
			{
				contact.EmailAddresses[EmailAddressIndex.Email1] = new Participant(this.buddy.DisplayName, this.buddy.IMAddress, "SMTP");
				contact.DisplayName = this.buddy.DisplayName;
				contact.ImAddress = this.buddy.IMAddress;
				contact[ContactSchema.GivenName] = this.buddy.DisplayName;
				contact.Save(SaveMode.ResolveConflicts);
				contact.Load(new PropertyDefinition[]
				{
					ContactSchema.Email1EmailAddress
				});
				result = contact.EmailAddresses[EmailAddressIndex.Email1];
			}
			return result;
		}

		private void AddToBuddyList(Participant participant, Folder contactsFolder)
		{
			StoreId folderId = BuddyListUtilities.GetSubFolderIdByClass(contactsFolder, "IPF.Contact.BuddyList") ?? this.CreateContactsSubFolder(contactsFolder, "Buddy List", "IPF.Contact.BuddyList");
			using (Folder folder = Folder.Bind(this.session, folderId))
			{
				StoreId storeId = AddBuddy.GetOtherContactsDLId(folder) ?? this.CreateOtherContactsPDL(folder);
				using (DistributionList distributionList = DistributionList.Bind(this.session, storeId))
				{
					distributionList.OpenAsReadWrite();
					distributionList.Add(participant);
					distributionList.Save(SaveMode.ResolveConflicts);
				}
			}
		}

		private StoreId CreateContactsSubFolder(Folder folder, string displayName, string folderClassName)
		{
			StoreId id;
			using (Folder folder2 = Folder.Create(this.session, folder.StoreObjectId, StoreObjectType.ContactsFolder))
			{
				folder2.DisplayName = displayName;
				folder2.ClassName = folderClassName;
				folder2.Save();
				folder2.Load(new PropertyDefinition[]
				{
					FolderSchema.Id
				});
				id = folder2.Id;
			}
			return id;
		}

		private StoreId CreateOtherContactsPDL(Folder buddyListFolder)
		{
			StoreId id;
			using (DistributionList distributionList = DistributionList.Create(this.session, buddyListFolder.StoreObjectId))
			{
				distributionList.DisplayName = "Other Contacts";
				distributionList.ClassName = "IPM.DistList.MOC.OtherContacts";
				distributionList.Save(SaveMode.ResolveConflicts);
				distributionList.Load(new PropertyDefinition[]
				{
					ItemSchema.Id
				});
				id = distributionList.Id;
			}
			return id;
		}

		private readonly MailboxSession session;

		private readonly Buddy buddy;
	}
}
