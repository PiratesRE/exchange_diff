using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetPeopleFilters : ServiceCommand<PeopleFilter[]>
	{
		public GetPeopleFilters(CallContext callContext) : base(callContext)
		{
		}

		protected override PeopleFilter[] InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			ExchangeVersion.Current = ExchangeVersion.Latest;
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			MailboxId mailboxId = new MailboxId(mailboxIdentityMailboxSession);
			List<PeopleFilter> list = new List<PeopleFilter>();
			list.Add(this.MakePeopleFilter(mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.MyContacts), mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration), mailboxId, ClientStrings.MyContactsFolderName.ToString(), 1, true));
			PeopleFilterGroupPriorityManager peopleFilterGroupPriorityManager = new PeopleFilterGroupPriorityManager(mailboxIdentityMailboxSession, new XSOFactory());
			foreach (IStorePropertyBag storePropertyBag in new ContactFoldersEnumerator(mailboxIdentityMailboxSession, new XSOFactory(), ContactFoldersEnumeratorOptions.SkipHiddenFolders | ContactFoldersEnumeratorOptions.SkipDeletedFolders, PeopleFilterGroupPriorityManager.RequiredFolderProperties))
			{
				StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(FolderSchema.Id)).ObjectId;
				StoreObjectId parentId = storePropertyBag.TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId;
				string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(FolderSchema.DisplayName, string.Empty);
				int sortGroupPriority = peopleFilterGroupPriorityManager.DetermineSortGroupPriority(storePropertyBag);
				ExtendedFolderFlags valueOrDefault2 = storePropertyBag.GetValueOrDefault<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags, (ExtendedFolderFlags)0);
				bool isReadOnly = (valueOrDefault2 & ExtendedFolderFlags.ReadOnly) == ExtendedFolderFlags.ReadOnly;
				list.Add(this.MakePeopleFilter(objectId, parentId, mailboxId, valueOrDefault, sortGroupPriority, isReadOnly));
			}
			if (userContext.FeaturesManager.ClientServerSettings.OwaPublicFolders.Enabled)
			{
				IFavoritePublicFoldersManager favoritePublicFoldersManager = new FavoritePublicFoldersManager(mailboxIdentityMailboxSession);
				List<IFavoritePublicFolder> list2 = new List<IFavoritePublicFolder>();
				using (IEnumerator<IFavoritePublicFolder> enumerator2 = favoritePublicFoldersManager.EnumerateContactsFolders().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IFavoritePublicFolder folder = enumerator2.Current;
						if (!list2.Exists((IFavoritePublicFolder storedFavorite) => storedFavorite.FolderId.Equals(folder.FolderId)))
						{
							list2.Add(folder);
						}
					}
				}
				if (list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						list.Add(this.MakePublicFolderPeopleFilter(list2[i], i));
					}
				}
			}
			ClientSecurityContext clientSecurityContext = base.CallContext.EffectiveCaller.ClientSecurityContext;
			IExchangePrincipal mailboxOwner = base.MailboxIdentityMailboxSession.MailboxOwner;
			AddressLists addressLists = new AddressLists(clientSecurityContext, mailboxOwner, userContext);
			if (addressLists.GlobalAddressList != null)
			{
				list.Add(new PeopleFilter
				{
					DisplayName = addressLists.GlobalAddressList.DisplayName,
					FolderId = new AddressListId
					{
						Id = addressLists.GlobalAddressList.Id.ObjectGuid.ToString()
					},
					SortGroupPriority = 1000,
					IsReadOnly = true
				});
			}
			if (addressLists.AllRoomsAddressList != null)
			{
				list.Add(new PeopleFilter
				{
					DisplayName = addressLists.AllRoomsAddressList.DisplayName,
					FolderId = new AddressListId
					{
						Id = addressLists.AllRoomsAddressList.Id.ObjectGuid.ToString()
					},
					SortGroupPriority = 1001,
					IsReadOnly = true
				});
			}
			foreach (AddressBookBase addressBookBase in addressLists.AllAddressLists)
			{
				if ((addressLists.GlobalAddressList == null || !addressBookBase.Id.Equals(addressLists.GlobalAddressList.Id)) && (addressLists.AllRoomsAddressList == null || !addressBookBase.Id.Equals(addressLists.AllRoomsAddressList.Id)) && !string.IsNullOrEmpty(addressBookBase.RecipientFilter) && (!addressBookBase.IsModernGroupsAddressList || userContext.FeaturesManager.ClientServerSettings.ModernGroups.Enabled))
				{
					list.Add(new PeopleFilter
					{
						DisplayName = addressBookBase.DisplayName,
						FolderId = new AddressListId
						{
							Id = addressBookBase.Id.ObjectGuid.ToString()
						},
						SortGroupPriority = (addressBookBase.IsModernGroupsAddressList ? 1009 : this.GetSortGroupPriority(addressBookBase.RecipientFilter)),
						IsReadOnly = true
					});
				}
			}
			list.Sort(new PeopleFilterComparer(mailboxIdentityMailboxSession.PreferedCulture));
			return list.ToArray();
		}

		private PeopleFilter MakePeopleFilter(StoreId folderId, StoreId parentId, MailboxId mailboxId, string displayName, int sortGroupPriority, bool isReadOnly)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(folderId, mailboxId, null);
			FolderId parentFolderId = null;
			if (parentId != null)
			{
				parentFolderId = IdConverter.GetFolderIdFromStoreId(parentId, mailboxId);
			}
			return new PeopleFilter
			{
				DisplayName = displayName,
				FolderId = new FolderId
				{
					Id = concatenatedId.Id
				},
				ParentFolderId = parentFolderId,
				SortGroupPriority = sortGroupPriority,
				IsReadOnly = isReadOnly
			};
		}

		private PeopleFilter MakePublicFolderPeopleFilter(IFavoritePublicFolder folder, int folderIndex)
		{
			ConcatenatedIdAndChangeKey concatenatedIdForPublicFolder = IdConverter.GetConcatenatedIdForPublicFolder(folder.FolderId);
			return new PeopleFilter
			{
				DisplayName = folder.DisplayName,
				SortGroupPriority = 2000 + folderIndex,
				IsReadOnly = false,
				FolderId = new FolderId
				{
					Id = concatenatedIdForPublicFolder.Id
				}
			};
		}

		private int GetSortGroupPriority(string recipientFilter)
		{
			if (string.Equals(recipientFilter, CannedAddressListsFilterHelper.DefaultAllUsersFilter.GenerateInfixString(FilterLanguage.Monad), StringComparison.OrdinalIgnoreCase))
			{
				return 1002;
			}
			if (string.Equals(recipientFilter, CannedAddressListsFilterHelper.DefaultAllDistributionListsFilter.GenerateInfixString(FilterLanguage.Monad), StringComparison.OrdinalIgnoreCase))
			{
				return 1003;
			}
			if (string.Equals(recipientFilter, CannedAddressListsFilterHelper.DefaultAllContactsFilter.GenerateInfixString(FilterLanguage.Monad), StringComparison.OrdinalIgnoreCase))
			{
				return 1004;
			}
			return 1010;
		}

		private int GetPersonCount(Folder peopleFolder)
		{
			return peopleFolder.PersonItemQuery(new StorePropertyDefinition[]
			{
				PersonSchema.Id
			}).EstimatedRowCount;
		}

		private int GetPersonCount(MailboxSession session, StoreObjectId peopleFolderId)
		{
			int personCount;
			using (Folder folder = Folder.Bind(session, peopleFolderId))
			{
				personCount = this.GetPersonCount(folder);
			}
			return personCount;
		}
	}
}
