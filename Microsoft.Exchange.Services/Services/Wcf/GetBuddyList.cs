using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetBuddyList
	{
		internal GetBuddyList(MailboxSession session)
		{
			this.session = session;
		}

		internal GetBuddyListResponse Execute()
		{
			GetBuddyListResponse result = new GetBuddyListResponse();
			using (Folder folder = Folder.Bind(this.session, DefaultFolderType.Contacts))
			{
				StoreId subFolderIdByClass = BuddyListUtilities.GetSubFolderIdByClass(folder, "IPF.Contact.QuickContacts");
				if (subFolderIdByClass != null)
				{
					StoreId subFolderIdByClass2 = BuddyListUtilities.GetSubFolderIdByClass(folder, "IPF.Contact.BuddyList");
					using (Folder folder2 = Folder.Bind(this.session, subFolderIdByClass))
					{
						result = this.GetBuddies(folder2, subFolderIdByClass2);
					}
				}
			}
			return result;
		}

		private GetBuddyListResponse GetBuddies(Folder quickContactsFolder, StoreId buddyListFolderId)
		{
			Dictionary<StoreObjectId, Microsoft.Exchange.Services.Core.Types.ItemId> dictionary = new Dictionary<StoreObjectId, Microsoft.Exchange.Services.Core.Types.ItemId>();
			List<Persona> list = new List<Persona>();
			PropertyDefinition[] defaultPersonaProperties = Persona.DefaultPersonaProperties;
			PropertyDefinition[] array = new PropertyDefinition[defaultPersonaProperties.Length + 1];
			array[0] = PersonSchema.ContactItemIds;
			defaultPersonaProperties.CopyTo(array, 1);
			using (IQueryResult queryResult = quickContactsFolder.PersonItemQuery(new SortBy[]
			{
				new SortBy(PersonSchema.DisplayName, SortOrder.Ascending)
			}, array))
			{
				List<IStorePropertyBag> list2 = queryResult.FetchAllPropertyBags();
				foreach (IStorePropertyBag propertyBag in list2)
				{
					Persona persona = Persona.LoadFromPropertyBag(this.session, Persona.GetProperties(propertyBag, defaultPersonaProperties), Persona.DefaultPersonaPropertyList);
					list.Add(persona);
					StoreObjectId[] array2 = propertyBag.TryGetValueOrDefault(PersonSchema.ContactItemIds, new StoreObjectId[0]);
					foreach (StoreObjectId key in array2)
					{
						dictionary[key] = persona.PersonaId;
					}
				}
			}
			List<BuddyGroup> list3 = new List<BuddyGroup>();
			using (Folder folder = Folder.Bind(this.session, buddyListFolderId))
			{
				using (QueryResult queryResult2 = folder.ItemQuery(ItemQueryType.None, new TextFilter(StoreObjectSchema.ItemClass, "IPM.DistList", MatchOptions.Prefix, MatchFlags.Default), null, new PropertyDefinition[]
				{
					ItemSchema.Id,
					StoreObjectSchema.DisplayName
				}))
				{
					List<IStorePropertyBag> list4 = queryResult2.FetchAllPropertyBags();
					foreach (IStorePropertyBag dlResult in list4)
					{
						BuddyGroup item = this.BuildGroupFromDL(dlResult, dictionary);
						list3.Add(item);
					}
				}
			}
			return new GetBuddyListResponse
			{
				Buddies = list.ToArray(),
				Groups = list3.ToArray()
			};
		}

		private BuddyGroup BuildGroupFromDL(IStorePropertyBag dlResult, Dictionary<StoreObjectId, Microsoft.Exchange.Services.Core.Types.ItemId> contactToPersonaMap)
		{
			VersionedId versionedId = dlResult.TryGetValueOrDefault(ItemSchema.Id, null);
			string displayName = dlResult.TryGetValueOrDefault(StoreObjectSchema.DisplayName, null);
			BuddyGroup buddyGroup = new BuddyGroup(versionedId, displayName);
			using (DistributionList distributionList = DistributionList.Bind(this.session, versionedId))
			{
				HashSet<Microsoft.Exchange.Services.Core.Types.ItemId> hashSet = new HashSet<Microsoft.Exchange.Services.Core.Types.ItemId>();
				foreach (DistributionListMember distributionListMember in distributionList)
				{
					if (!(distributionListMember.Participant == null))
					{
						StoreParticipantOrigin storeParticipantOrigin = distributionListMember.Participant.Origin as StoreParticipantOrigin;
						if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId.ObjectType == StoreObjectType.Contact)
						{
							StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
							Microsoft.Exchange.Services.Core.Types.ItemId item;
							if (contactToPersonaMap.TryGetValue(originItemId, out item))
							{
								hashSet.Add(item);
							}
						}
					}
				}
				buddyGroup.PersonaIds = hashSet.ToArray<Microsoft.Exchange.Services.Core.Types.ItemId>();
			}
			return buddyGroup;
		}

		private readonly MailboxSession session;
	}
}
