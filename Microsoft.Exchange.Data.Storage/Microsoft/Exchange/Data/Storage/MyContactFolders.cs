using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MyContactFolders
	{
		internal MyContactFolders(StoreObjectId[] folderIds)
		{
			this.folderIds = folderIds;
		}

		public MyContactFolders(IXSOFactory xsoFactory, IMailboxSession mailboxSession)
		{
			this.xsoFactory = xsoFactory;
			this.mailboxSession = mailboxSession;
		}

		public StoreObjectId[] Value
		{
			get
			{
				this.EnsureFolderIds(false);
				return this.folderIds;
			}
		}

		public bool Contains(StoreObjectId folderId)
		{
			this.EnsureFolderIds(false);
			return this.folderIds.Contains(folderId);
		}

		public void Add(StoreObjectId newFolderId)
		{
			this.EnsureFolderIds(true);
			if (!Array.Exists<StoreObjectId>(this.folderIds, (StoreObjectId folderId) => folderId.Equals(newFolderId)))
			{
				MyContactFolders.Tracer.TraceDebug<StoreObjectId, ArrayTracer<StoreObjectId>>((long)this.GetHashCode(), "Adding folder '{0}' to the list of MyContactFolders: {1}", newFolderId, new ArrayTracer<StoreObjectId>(this.folderIds));
				List<StoreObjectId> list = new List<StoreObjectId>(this.folderIds.Length + 1);
				list.AddRange(this.folderIds);
				list.Add(newFolderId);
				this.Set(list.ToArray());
			}
		}

		public void Set(StoreObjectId[] newFolderIds)
		{
			MyContactFolders.Tracer.TraceDebug<ArrayTracer<StoreObjectId>>((long)this.GetHashCode(), "Updating folder ids with list of folders: {0}", new ArrayTracer<StoreObjectId>(newFolderIds));
			using (IFolder folder = this.xsoFactory.BindToFolder(this.mailboxSession, DefaultFolderType.Configuration, new PropertyDefinition[]
			{
				InternalSchema.MyContactsFolders
			}))
			{
				this.SetFolderIds(folder, newFolderIds);
				this.UpdateFolderIdsProperty(folder, newFolderIds);
			}
			this.folderIds = newFolderIds;
		}

		private void UpdateFolderIdsProperty(IFolder rootFolder, StoreObjectId[] folderIds)
		{
			MyContactFolders.Tracer.TraceDebug<string, ArrayTracer<StoreObjectId>>((long)this.GetHashCode(), "Updating folder ids property {0} on root folder with new list of folder ids: {1}", InternalSchema.MyContactsFolders.Name, new ArrayTracer<StoreObjectId>(folderIds));
			rootFolder[InternalSchema.MyContactsFolders] = folderIds;
			rootFolder.Save(SaveMode.NoConflictResolutionForceSave);
		}

		private void EnsureFolderIds(bool reload)
		{
			if (this.folderIds == null || reload)
			{
				MyContactFolders.Tracer.TraceDebug((long)this.GetHashCode(), "Loading folder ids from the mailbox");
				using (IFolder folder = this.xsoFactory.BindToFolder(this.mailboxSession, DefaultFolderType.Configuration, new PropertyDefinition[]
				{
					InternalSchema.MyContactsFolders
				}))
				{
					StoreObjectId[] valueOrDefault = folder.GetValueOrDefault<StoreObjectId[]>(InternalSchema.MyContactsFolders, null);
					if (valueOrDefault == null)
					{
						valueOrDefault = this.GetFolderIds();
						this.UpdateFolderIdsProperty(folder, valueOrDefault);
					}
					else if (!MyContactFolders.AreSame(valueOrDefault, this.folderIds))
					{
						this.UpdateFolderIdsProperty(folder, valueOrDefault);
					}
					this.folderIds = valueOrDefault;
				}
			}
		}

		private static bool AreSame(StoreObjectId[] a, StoreObjectId[] b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			Array.Sort<StoreObjectId>(a);
			Array.Sort<StoreObjectId>(b);
			for (int i = 0; i < a.Length; i++)
			{
				if (!a[i].Equals(b[i]))
				{
					return false;
				}
			}
			return true;
		}

		private StoreObjectId[] GetFolderIds()
		{
			StoreObjectId[] array = this.GetFolderIdsFromSearchFolder(DefaultFolderType.MyContacts);
			if (array == null)
			{
				MyContactFolders.Tracer.TraceDebug((long)this.GetHashCode(), "Unable to get folder ids from the MyContacts search folder, instead loading folder ids from the default folders in the mailbox");
				array = ContactsSearchFolderCriteria.MyContacts.GetDefaultFolderScope(this.mailboxSession, true);
			}
			return array;
		}

		private void SetFolderIds(IFolder rootFolder, StoreObjectId[] newFolderIds)
		{
			newFolderIds = ContactsSearchFolderCriteria.RemoveDeletedFoldersFromCollection(this.xsoFactory, this.mailboxSession, newFolderIds);
			ContactsSearchFolderCriteria.MyContacts.UpdateFolderScope(this.xsoFactory, this.mailboxSession, newFolderIds);
			ContactsSearchFolderCriteria.MyContactsExtended.UpdateFolderScope(this.xsoFactory, this.mailboxSession, ContactsSearchFolderCriteria.GetMyContactExtendedFolders(this.mailboxSession, newFolderIds, true));
		}

		private StoreObjectId[] GetFolderIdsFromSearchFolder(DefaultFolderType searchFolderId)
		{
			SearchFolderCriteria searchFolderCriteria = null;
			try
			{
				using (ISearchFolder searchFolder = this.xsoFactory.BindToSearchFolder(this.mailboxSession, searchFolderId))
				{
					searchFolderCriteria = searchFolder.GetSearchCriteria();
				}
			}
			catch (ObjectNotInitializedException arg)
			{
				MyContactFolders.Tracer.TraceError<DefaultFolderType, ObjectNotInitializedException>((long)this.GetHashCode(), "Unable to load folder ids from the search folder {0} due ObjectNotInitializedException: {1}", searchFolderId, arg);
				return null;
			}
			catch (ObjectNotFoundException arg2)
			{
				MyContactFolders.Tracer.TraceError<DefaultFolderType, ObjectNotFoundException>((long)this.GetHashCode(), "Unable to load folder ids from the search folder {0} due ObjectNotFoundException: {1}", searchFolderId, arg2);
				return null;
			}
			if (searchFolderCriteria == null)
			{
				MyContactFolders.Tracer.TraceError<DefaultFolderType>((long)this.GetHashCode(), "There is no search folder criteria in the search folder {0}", searchFolderId);
				return null;
			}
			if (searchFolderCriteria.FolderScope == null)
			{
				MyContactFolders.Tracer.TraceError<DefaultFolderType>((long)this.GetHashCode(), "There is no folder scope in the search folder {0}", searchFolderId);
				return null;
			}
			if (searchFolderCriteria.FolderScope.Length == 0)
			{
				MyContactFolders.Tracer.TraceError<DefaultFolderType>((long)this.GetHashCode(), "The folder scope in the search folder {0} is empty", searchFolderId);
				return null;
			}
			StoreObjectId[] array = new StoreObjectId[searchFolderCriteria.FolderScope.Length];
			for (int i = 0; i < searchFolderCriteria.FolderScope.Length; i++)
			{
				array[i] = (StoreObjectId)searchFolderCriteria.FolderScope[i];
			}
			MyContactFolders.Tracer.TraceDebug<DefaultFolderType, ArrayTracer<StoreObjectId>>((long)this.GetHashCode(), "Loaded folder ids from the scope of search folder {0}: {1}", searchFolderId, new ArrayTracer<StoreObjectId>(array));
			return array;
		}

		private static readonly Trace Tracer = ExTraceGlobals.MyContactsFolderTracer;

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession mailboxSession;

		private StoreObjectId[] folderIds;
	}
}
