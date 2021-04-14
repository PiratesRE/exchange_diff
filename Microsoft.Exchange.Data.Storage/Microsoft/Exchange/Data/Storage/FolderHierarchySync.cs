using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderHierarchySync
	{
		public FolderHierarchySync(MailboxSession storeSession, IFolderHierarchySyncState syncState, ChangeTrackingDelegate changeTrackingDelegate)
		{
			ExTraceGlobals.SyncTracer.Information<int>((long)this.GetHashCode(), "FolderHierarchySync::Constructor. HashCode = {0}.", this.GetHashCode());
			if (storeSession == null)
			{
				throw new ArgumentNullException("storeSession");
			}
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			this.changeTrackingDelegate = changeTrackingDelegate;
			this.storeSession = storeSession;
			this.syncState = syncState;
			if (this.ClientState == null)
			{
				this.ClientState = new Dictionary<StoreObjectId, FolderStateEntry>();
			}
			if (this.ServerManifest == null)
			{
				this.ServerManifest = new Dictionary<StoreObjectId, FolderManifestEntry>();
			}
		}

		internal static bool TryGetPropertyFromBag<T>(IStorePropertyBag propertyBag, PropertyDefinition propDef, ISyncLogger syncLogger, out T value)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			object obj = null;
			try
			{
				obj = propertyBag.TryGetProperty(propDef);
			}
			catch (NotInBagPropertyErrorException)
			{
				syncLogger.TraceError<string>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchySync.TryGetPropertyFromBag] NotInBag exception for property {0}.  Returning default value.", propDef.Name);
				value = default(T);
				return false;
			}
			if (obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				syncLogger.TraceError<Type, string, PropertyErrorCode>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchySync.TryGetPropertyFromBag] Expected property of type {0} in bag for propDef {1}, but encountered error {2}.", typeof(T), propDef.Name, propertyError.PropertyErrorCode);
			}
			else
			{
				try
				{
					value = (T)((object)obj);
					return true;
				}
				catch (InvalidCastException ex)
				{
					syncLogger.TraceError(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchySync.TryGetPropertyFromBag] Tried to cast property '{0}' with value '{1}' to type '{2}', but the cast failed with error '{3}'.", new object[]
					{
						propDef.Name,
						(obj == null) ? "<NULL>" : obj,
						typeof(T).FullName,
						ex
					});
				}
			}
			value = default(T);
			return false;
		}

		private Dictionary<StoreObjectId, FolderStateEntry> ClientState
		{
			get
			{
				if (!this.syncState.Contains(SyncStateProp.ClientState))
				{
					return null;
				}
				return ((GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderStateEntry>)this.syncState[SyncStateProp.ClientState]).Data;
			}
			set
			{
				this.syncState[SyncStateProp.ClientState] = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderStateEntry>(value);
			}
		}

		private Dictionary<StoreObjectId, FolderManifestEntry> ServerManifest
		{
			get
			{
				if (!this.syncState.Contains(SyncStateProp.CurServerManifest))
				{
					return null;
				}
				return ((GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderManifestEntry>)this.syncState[SyncStateProp.CurServerManifest]).Data;
			}
			set
			{
				this.syncState[SyncStateProp.CurServerManifest] = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderManifestEntry>(value);
			}
		}

		public bool IsValidClientOperation(StoreObjectId id, ChangeType changeType, Folder folder)
		{
			EnumValidator.ThrowIfInvalid<ChangeType>(changeType, "changeType");
			if (this.ClientState == null || id == null || folder == null)
			{
				return false;
			}
			if (changeType == ChangeType.Add)
			{
				return !this.ClientState.ContainsKey(id);
			}
			return this.ClientState.ContainsKey(id);
		}

		public void AcknowledgeServerOperations()
		{
			ExTraceGlobals.SyncTracer.Information((long)this.GetHashCode(), "Storage.FolderHierarchySync.AcknowledgeServerOperations");
			foreach (FolderManifestEntry folderManifestEntry in this.ServerManifest.Values)
			{
				switch (folderManifestEntry.ChangeType)
				{
				case ChangeType.Add:
				case ChangeType.Change:
					this.ClientState[folderManifestEntry.ItemId] = (FolderStateEntry)folderManifestEntry;
					break;
				case ChangeType.Delete:
					this.ClientState.Remove(folderManifestEntry.ItemId);
					break;
				}
			}
			this.ServerManifest.Clear();
		}

		public HierarchySyncOperations EnumerateServerOperations()
		{
			return this.EnumerateServerOperations(this.storeSession.GetDefaultFolderId(DefaultFolderType.Root), true);
		}

		public HierarchySyncOperations EnumerateServerOperations(StoreObjectId rootFolderId)
		{
			return this.EnumerateServerOperations(rootFolderId, true);
		}

		public HierarchySyncOperations EnumerateServerOperations(StoreObjectId rootFolderId, bool excludeHiddenFolders)
		{
			return this.EnumerateServerOperations(rootFolderId, excludeHiddenFolders, FolderSyncStateMetadata.IPMFolderNullSyncProperties, null);
		}

		public HierarchySyncOperations EnumerateServerOperations(StoreObjectId rootFolderId, bool excludeHiddenFolders, PropertyDefinition[] propertiesToFetch, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			syncLogger.Information(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "Storage.FolderHierarchySync.EnumerateServerOperations(propDefs).");
			ArgumentValidator.ThrowIfNull("rootfolderId", rootFolderId);
			this.ServerManifest.Clear();
			Dictionary<StoreObjectId, FolderStateEntry> serverFolders = new Dictionary<StoreObjectId, FolderStateEntry>();
			using (Folder folder = Folder.Bind(this.storeSession, rootFolderId))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, propertiesToFetch))
				{
					IStorePropertyBag[] propertyBags;
					do
					{
						propertyBags = queryResult.GetPropertyBags(10000);
						for (int i = 0; i < propertyBags.Length; i++)
						{
							this.AddServerManifestEntry(propertyBags[i], excludeHiddenFolders, serverFolders, syncLogger);
						}
					}
					while (propertyBags.Length != 0);
				}
			}
			return this.PostEnumerateServerOperations(serverFolders, rootFolderId);
		}

		public HierarchySyncOperations EnumerateServerOperations(StoreObjectId rootFolderId, bool excludeHiddenFolders, IEnumerable<IStorePropertyBag> rootFolderHierarchyList, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			syncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Storage.FolderHierarchySync.EnumerateServerOperations(hierarchy).");
			ArgumentValidator.ThrowIfNull("rootFolderId", rootFolderId);
			ArgumentValidator.ThrowIfNull("rootFolderHierarchyList", rootFolderHierarchyList);
			this.ServerManifest.Clear();
			Dictionary<StoreObjectId, FolderStateEntry> serverFolders = new Dictionary<StoreObjectId, FolderStateEntry>();
			foreach (IStorePropertyBag propertyBag in rootFolderHierarchyList)
			{
				this.AddServerManifestEntry(propertyBag, excludeHiddenFolders, serverFolders, syncLogger);
			}
			return this.PostEnumerateServerOperations(serverFolders, rootFolderId);
		}

		private HierarchySyncOperations PostEnumerateServerOperations(Dictionary<StoreObjectId, FolderStateEntry> serverFolders, StoreObjectId rootFolderId)
		{
			Dictionary<StoreObjectId, FolderManifestEntry> dictionary = new Dictionary<StoreObjectId, FolderManifestEntry>();
			foreach (KeyValuePair<StoreObjectId, FolderStateEntry> keyValuePair in this.ClientState)
			{
				StoreObjectId key = keyValuePair.Key;
				FolderStateEntry value = keyValuePair.Value;
				if (!serverFolders.ContainsKey(key))
				{
					dictionary.Add(key, new FolderManifestEntry(key)
					{
						ChangeType = ChangeType.Delete,
						ParentId = value.ParentId
					});
				}
			}
			foreach (FolderManifestEntry folderManifestEntry in dictionary.Values)
			{
				if (dictionary.ContainsKey(folderManifestEntry.ParentId))
				{
					this.ClientState.Remove(folderManifestEntry.ItemId);
				}
				else if (this.ClientState.ContainsKey(folderManifestEntry.ParentId) || folderManifestEntry.ParentId.Equals(rootFolderId))
				{
					this.ServerManifest[folderManifestEntry.ItemId] = folderManifestEntry;
				}
			}
			return new HierarchySyncOperations(this, this.ServerManifest, false);
		}

		public void RecordClientOperation(StoreObjectId folderId, ChangeType change, Folder folder)
		{
			EnumValidator.ThrowIfInvalid<ChangeType>(change, "change");
			ExTraceGlobals.SyncTracer.Information<StoreObjectId, ChangeType>((long)this.GetHashCode(), "Storage.FolderHierarchySync.RecordClientOperation. ItemId = {0}, ChangeType = {1}", folderId, change);
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (folder != null && !folderId.Equals(folder.Id.ObjectId))
			{
				throw new ArgumentException(ServerStrings.ExFolderDoesNotMatchFolderId);
			}
			if (change != ChangeType.Add && change != ChangeType.Change && change != ChangeType.Delete)
			{
				throw new ArgumentOutOfRangeException("change");
			}
			if ((change == ChangeType.Add || change == ChangeType.Change) && folder == null)
			{
				throw new ArgumentNullException("folder", ServerStrings.ExInvalidNullParameterForChangeTypes("folder", "ChangeType.Add, ChangeType.Change"));
			}
			switch (change)
			{
			case ChangeType.Add:
				if (this.ClientState.ContainsKey(folderId))
				{
					throw new ArgumentException(ServerStrings.ExFolderAlreadyExistsInClientState);
				}
				break;
			case ChangeType.Change:
			case ChangeType.Delete:
				if (!this.ClientState.ContainsKey(folderId))
				{
					throw new ArgumentException(ServerStrings.ExFolderNotFoundInClientState);
				}
				break;
			}
			switch (change)
			{
			case ChangeType.Add:
			case ChangeType.Change:
			{
				FolderStateEntry value = new FolderStateEntry(folder.ParentId, folder.GetValueOrDefault<byte[]>(InternalSchema.ChangeKey), this.changeTrackingDelegate(this.storeSession, folder.StoreObjectId, null));
				this.ClientState[folderId] = value;
				return;
			}
			case (ChangeType)3:
				break;
			case ChangeType.Delete:
				this.ClientState.Remove(folderId);
				break;
			default:
				return;
			}
		}

		internal Folder GetFolder(FolderManifestEntry serverManifestEntry, params PropertyDefinition[] prefetchProperties)
		{
			if (ChangeType.Delete != serverManifestEntry.ChangeType)
			{
				PropertyDefinition[] array;
				if (prefetchProperties != null)
				{
					array = new PropertyDefinition[prefetchProperties.Length + 1];
					prefetchProperties.CopyTo(array, 1);
				}
				else
				{
					array = new PropertyDefinition[1];
				}
				array[0] = InternalSchema.ChangeKey;
				Folder folder = Folder.Bind(this.storeSession, serverManifestEntry.ItemId, array);
				serverManifestEntry.ChangeKey = folder.GetValueOrDefault<byte[]>(InternalSchema.ChangeKey);
				return folder;
			}
			throw new InvalidOperationException("Cannot GetFolder() on an folder that has been deleted");
		}

		private void AddServerManifestEntry(IStorePropertyBag propertyBag, bool excludeHiddenFolders, Dictionary<StoreObjectId, FolderStateEntry> serverFolders, ISyncLogger syncLogger)
		{
			SharingSubscriptionData[] array = null;
			bool flag = false;
			if (FolderHierarchySync.TryGetPropertyFromBag<bool>(propertyBag, InternalSchema.IsHidden, syncLogger, out flag) && excludeHiddenFolders && flag)
			{
				return;
			}
			VersionedId versionedId;
			StoreObjectId parentId;
			byte[] array2;
			if (!FolderHierarchySync.TryGetPropertyFromBag<VersionedId>(propertyBag, FolderSchema.Id, syncLogger, out versionedId) || !FolderHierarchySync.TryGetPropertyFromBag<StoreObjectId>(propertyBag, StoreObjectSchema.ParentItemId, syncLogger, out parentId) || !FolderHierarchySync.TryGetPropertyFromBag<byte[]>(propertyBag, StoreObjectSchema.ChangeKey, syncLogger, out array2))
			{
				ExTraceGlobals.SyncTracer.Information((long)this.GetHashCode(), "Storage.FolderHierarchySync.AddServerManifestEntry. \nFolder is missing properties. Id , ParentId or ChangeKey");
				return;
			}
			int num = -1;
			serverFolders[versionedId.ObjectId] = null;
			FolderStateEntry folderStateEntry;
			if (this.ClientState.TryGetValue(versionedId.ObjectId, out folderStateEntry) && ArrayComparer<byte>.Comparer.Equals(folderStateEntry.ChangeKey, array2))
			{
				return;
			}
			if (this.changeTrackingDelegate != null)
			{
				num = this.changeTrackingDelegate(this.storeSession, versionedId.ObjectId, propertyBag);
				if (folderStateEntry != null && num == folderStateEntry.ChangeTrackingHash)
				{
					folderStateEntry.ChangeKey = array2;
					return;
				}
			}
			FolderManifestEntry folderManifestEntry = new FolderManifestEntry(versionedId.ObjectId);
			string className;
			FolderHierarchySync.TryGetPropertyFromBag<string>(propertyBag, StoreObjectSchema.ContainerClass, syncLogger, out className);
			folderManifestEntry.ChangeType = ((folderStateEntry != null) ? ChangeType.Change : ChangeType.Add);
			folderManifestEntry.ChangeKey = array2;
			folderManifestEntry.ParentId = parentId;
			folderManifestEntry.ChangeTrackingHash = num;
			folderManifestEntry.Hidden = flag;
			folderManifestEntry.ClassName = className;
			if (folderManifestEntry.ClassName == null)
			{
				folderManifestEntry.ClassName = string.Empty;
			}
			folderManifestEntry.DisplayName = (propertyBag[FolderSchema.DisplayName] as string);
			if (folderManifestEntry.DisplayName == null)
			{
				folderManifestEntry.DisplayName = string.Empty;
			}
			int num2;
			if (!FolderHierarchySync.TryGetPropertyFromBag<int>(propertyBag, FolderSchema.ExtendedFolderFlags, syncLogger, out num2))
			{
				num2 = 0;
			}
			bool flag2 = (num2 & 1073741824) != 0;
			if (flag2)
			{
				if (array == null)
				{
					using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(this.storeSession))
					{
						array = sharingSubscriptionManager.GetAll();
					}
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].LocalFolderId.Equals(folderManifestEntry.ItemId))
					{
						folderManifestEntry.Permissions = SyncPermissions.Readonly;
						folderManifestEntry.Owner = array[i].SharerIdentity;
						break;
					}
				}
			}
			this.ServerManifest.Add(folderManifestEntry.ItemId, folderManifestEntry);
		}

		private ChangeTrackingDelegate changeTrackingDelegate;

		private MailboxSession storeSession;

		private IFolderHierarchySyncState syncState;
	}
}
