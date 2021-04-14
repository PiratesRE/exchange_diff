using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleFilterGroupPriorityManager
	{
		public PeopleFilterGroupPriorityManager(IMailboxSession session, IXSOFactory xsoFactory)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		private StoreObjectId DefaultContactsFolderId
		{
			get
			{
				if (this.defaultContactsFolderId == null)
				{
					this.defaultContactsFolderId = this.session.GetDefaultFolderId(DefaultFolderType.Contacts);
				}
				return this.defaultContactsFolderId;
			}
		}

		private StoreObjectId QuickContactsFolderId
		{
			get
			{
				if (this.quickContactsFolderId == null)
				{
					this.quickContactsFolderId = this.session.GetDefaultFolderId(DefaultFolderType.QuickContacts);
				}
				return this.quickContactsFolderId;
			}
		}

		public static void SetSortGroupPriorityOnFolder(IStorePropertyBag folder, int sortGroupPriority)
		{
			folder[FolderSchema.PeopleHubSortGroupPriority] = sortGroupPriority;
			folder[FolderSchema.PeopleHubSortGroupPriorityVersion] = 2;
		}

		public int DetermineSortGroupPriority(IStorePropertyBag folder)
		{
			StoreObjectId objectId = ((VersionedId)folder.TryGetProperty(FolderSchema.Id)).ObjectId;
			int valueOrDefault = folder.GetValueOrDefault<int>(FolderSchema.PeopleHubSortGroupPriorityVersion, -1);
			int num = folder.GetValueOrDefault<int>(FolderSchema.PeopleHubSortGroupPriority, -1);
			bool valueOrDefault2 = folder.GetValueOrDefault<bool>(FolderSchema.IsPeopleConnectSyncFolder, false);
			if (valueOrDefault == 2 && num >= 0)
			{
				return num;
			}
			if (object.Equals(objectId, this.DefaultContactsFolderId))
			{
				num = 2;
			}
			else if (object.Equals(objectId, this.QuickContactsFolderId))
			{
				num = 4;
			}
			else if (valueOrDefault2)
			{
				num = 3;
			}
			else
			{
				num = 10;
			}
			using (IFolder folder2 = this.xsoFactory.BindToFolder(this.session, objectId))
			{
				PeopleFilterGroupPriorityManager.SetSortGroupPriorityOnFolder(folder2, num);
				folder2.Save();
			}
			return num;
		}

		public StoreObjectId[] GetMyContactFolderIds()
		{
			ContactFoldersEnumerator contactFoldersEnumerator = new ContactFoldersEnumerator(this.session, this.xsoFactory, ContactFoldersEnumeratorOptions.SkipHiddenFolders | ContactFoldersEnumeratorOptions.SkipDeletedFolders, PeopleFilterGroupPriorityManager.RequiredFolderProperties);
			List<StoreObjectId> list = new List<StoreObjectId>(10);
			foreach (IStorePropertyBag storePropertyBag in contactFoldersEnumerator)
			{
				StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(FolderSchema.Id)).ObjectId;
				int sortGroupPriority = this.DetermineSortGroupPriority(storePropertyBag);
				if (PeopleFilterGroupPriorities.ShouldBeIncludedInMyContactsFolder(sortGroupPriority))
				{
					list.Add(objectId);
				}
			}
			return list.ToArray();
		}

		public static readonly PropertyDefinition[] RequiredFolderProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentItemId,
			FolderSchema.PeopleHubSortGroupPriority,
			FolderSchema.PeopleHubSortGroupPriorityVersion,
			FolderSchema.IsPeopleConnectSyncFolder,
			FolderSchema.ExtendedFolderFlags
		};

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private StoreObjectId defaultContactsFolderId;

		private StoreObjectId quickContactsFolderId;
	}
}
