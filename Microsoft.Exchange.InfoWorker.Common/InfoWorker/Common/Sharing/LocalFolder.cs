using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class LocalFolder : IDisposable
	{
		private LocalFolder(MailboxSession mailboxSession, Folder folder, string remoteFolderId, Item binding)
		{
			this.mailboxSession = mailboxSession;
			this.folder = folder;
			this.remoteFolderId = remoteFolderId;
			this.binding = binding;
		}

		public StoreId Id
		{
			get
			{
				return this.folder.Id;
			}
		}

		public string RemoteFolderId
		{
			get
			{
				return this.remoteFolderId;
			}
		}

		public StoreObjectType Type
		{
			get
			{
				return this.folder.Id.ObjectId.ObjectType;
			}
		}

		public static LocalFolder Bind(MailboxSession mailboxSession, StoreId folderId)
		{
			return LocalFolder.Bind(mailboxSession, folderId, delegate(LocalFolder folder)
			{
				folder.Initialize();
			});
		}

		public static LocalFolder BindOnly(MailboxSession mailboxSession, StoreId folderId)
		{
			return LocalFolder.Bind(mailboxSession, folderId, delegate(LocalFolder folder)
			{
			});
		}

		private static LocalFolder Bind(MailboxSession mailboxSession, StoreId folderId, LocalFolder.ProcessFolderDelegate processFolder)
		{
			SharingBindingManager sharingBindingManager = new SharingBindingManager(mailboxSession);
			SharingBindingData sharingBindingDataInFolder = sharingBindingManager.GetSharingBindingDataInFolder(folderId);
			if (sharingBindingDataInFolder == null)
			{
				LocalFolder.Tracer.TraceError<IExchangePrincipal, StoreId>(0L, "{0}: Unable to find the binding for folder {1}, fail sync", mailboxSession.MailboxOwner, folderId);
				throw new SubscriptionNotFoundException();
			}
			bool flag = false;
			Item item = null;
			Folder folder = null;
			LocalFolder localFolder = null;
			try
			{
				item = LocalFolder.BindToBindingMessage(mailboxSession, sharingBindingDataInFolder.Id);
				folder = Folder.Bind(mailboxSession, folderId, LocalFolder.extraProperties);
				localFolder = new LocalFolder(mailboxSession, folder, sharingBindingDataInFolder.RemoteFolderId, item);
				processFolder(localFolder);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (localFolder != null)
					{
						localFolder.Dispose();
					}
					else
					{
						if (item != null)
						{
							item.Dispose();
						}
						if (folder != null)
						{
							folder.Dispose();
						}
					}
					localFolder = null;
				}
			}
			return localFolder;
		}

		private static Item BindToBindingMessage(MailboxSession mailboxSession, StoreId itemId)
		{
			return Item.Bind(mailboxSession, itemId, LocalFolder.extraProperties);
		}

		public void Dispose()
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
			}
			if (this.binding != null)
			{
				this.binding.Dispose();
			}
		}

		public StoreId GetLocalIdFromRemoteId(string remoteId)
		{
			LocalFolder.LocalItem localItem;
			if (this.localItems.TryGetValue(remoteId, out localItem))
			{
				return localItem.Id;
			}
			return null;
		}

		public void DeleteAllItems()
		{
			LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: deleting all items from folder.", this);
			this.folder.DeleteAllObjects(DeleteItemFlags.HardDelete);
			this.itemsToDelete.Clear();
		}

		public void SelectItemToDelete(StoreId itemId)
		{
			this.itemsToDelete.Add(itemId);
		}

		public void DeleteSelectedItems()
		{
			List<StoreId> list = new List<StoreId>(100);
			foreach (StoreId item in this.itemsToDelete)
			{
				if (list.Count == 100)
				{
					this.DeleteItems(list.ToArray());
					list.Clear();
				}
				list.Add(item);
			}
			if (list.Count > 0)
			{
				this.DeleteItems(list.ToArray());
			}
			this.itemsToDelete.Clear();
		}

		public void SaveSyncState(string syncState)
		{
			LocalFolder.Tracer.TraceDebug<LocalFolder, string>((long)this.GetHashCode(), "{0}: Saving sync state: {1}", this, syncState);
			this.SafeUpdateBindingItem(delegate(Item binding)
			{
				using (Stream stream = binding.OpenPropertyStream(SharingSchema.ExternalSharingSyncState, PropertyOpenMode.Create))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						streamWriter.Write(syncState);
					}
				}
			});
		}

		public void UpdateLastAttemptedSyncTime()
		{
			LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: Updating the last attempted sync time.", this);
			ExDateTime now = ExDateTime.Now;
			this.folder[FolderSchema.SubscriptionLastAttemptedSyncTime] = now;
			this.folder.Save();
			this.folder.Load();
		}

		public void UpdateLastSyncTimes()
		{
			LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: Updating the last attempted and successful sync times.", this);
			ExDateTime now = ExDateTime.Now;
			this.folder[FolderSchema.SubscriptionLastAttemptedSyncTime] = now;
			this.folder[FolderSchema.SubscriptionLastSuccessfulSyncTime] = now;
			this.folder.Save();
			this.folder.Load();
			this.SafeUpdateBindingItem(delegate(Item binding)
			{
				binding[BindingItemSchema.SharingLastSync] = now;
			});
		}

		public string LoadSyncState()
		{
			string text;
			try
			{
				using (Stream stream = this.binding.OpenPropertyStream(SharingSchema.ExternalSharingSyncState, PropertyOpenMode.ReadOnly))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						text = streamReader.ReadToEnd();
					}
				}
			}
			catch (PropertyErrorException)
			{
				LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: Got a PropertyError exception trying to fetch the syncstate, starting over.", this);
				text = null;
			}
			catch (ObjectNotFoundException)
			{
				LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: No sync state found on the folder", this);
				text = null;
			}
			LocalFolder.Tracer.TraceDebug<LocalFolder, string>((long)this.GetHashCode(), "{0}: Current sync state: {1}", this, text);
			return text;
		}

		public void DeleteSyncState()
		{
			this.SafeUpdateBindingItem(delegate(Item binding)
			{
				binding.Delete(SharingSchema.ExternalSharingSyncState);
			});
		}

		public void SaveLevelOfDetails(LevelOfDetails levelOfDetails)
		{
			this.folder[SharingSchema.ExternalSharingLevelOfDetails] = levelOfDetails;
			this.folder.Save();
			this.folder.Load();
		}

		public LevelOfDetails LoadLevelOfDetails()
		{
			object obj = this.folder.TryGetProperty(SharingSchema.ExternalSharingLevelOfDetails);
			if (obj == null || obj is PropertyError || !Enum.IsDefined(typeof(LevelOfDetails), obj))
			{
				return LevelOfDetails.Unknown;
			}
			return (LevelOfDetails)obj;
		}

		public void ProcessAllItems(LocalFolder.ProcessItem processItem)
		{
			using (QueryResult queryResult = this.folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
			{
				ItemSchema.Id
			}))
			{
				LocalFolder.Tracer.TraceDebug<LocalFolder, int>((long)this.GetHashCode(), "{0}: Inspecting {1} items for excessive data.", this, queryResult.EstimatedRowCount);
				for (;;)
				{
					object[][] rows = queryResult.GetRows(200);
					if (rows.Length == 0)
					{
						break;
					}
					foreach (object[] array2 in rows)
					{
						VersionedId versionedId = array2[0] as VersionedId;
						if (versionedId != null)
						{
							LocalFolder.Tracer.TraceDebug<LocalFolder, VersionedId>((long)this.GetHashCode(), "{0}: Processing item {1} due to excessive data.", this, versionedId);
							processItem(versionedId);
						}
					}
				}
			}
		}

		private void SafeUpdateBindingItem(Action<Item> stampBindingItem)
		{
			stampBindingItem(this.binding);
			ConflictResolutionResult conflictResolutionResult = this.binding.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				LocalFolder.Tracer.TraceDebug<LocalFolder>((long)this.GetHashCode(), "{0}: Conflict occurs when saving the binding message. Reload and try saving again.", this);
				StoreObjectId objectId = this.binding.Id.ObjectId;
				this.binding.Dispose();
				this.binding = LocalFolder.BindToBindingMessage(this.mailboxSession, objectId);
				this.binding.OpenAsReadWrite();
				stampBindingItem(this.binding);
				this.binding.Save(SaveMode.NoConflictResolution);
			}
			this.binding.Load();
		}

		private void DeleteItems(StoreId[] itemIds)
		{
			LocalFolder.Tracer.TraceDebug<LocalFolder, ArrayTracer<StoreId>>((long)this.GetHashCode(), "{0}: deleting items from folder: {1}", this, new ArrayTracer<StoreId>(itemIds));
			this.folder.DeleteObjects(DeleteItemFlags.HardDelete, itemIds);
		}

		private void Initialize()
		{
			this.localItems = this.LoadLocalItemsAndDeleteDuplicates();
		}

		private Dictionary<string, LocalFolder.LocalItem> LoadLocalItemsAndDeleteDuplicates()
		{
			Dictionary<string, LocalFolder.LocalItem> result;
			using (QueryResult queryResult = this.folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
			{
				ItemSchema.Id,
				SharingSchema.ExternalSharingMasterId,
				StoreObjectSchema.CreationTime
			}))
			{
				Dictionary<string, LocalFolder.LocalItem> dictionary = new Dictionary<string, LocalFolder.LocalItem>();
				bool flag = false;
				for (;;)
				{
					object[][] rows = queryResult.GetRows(200);
					if (rows.Length == 0)
					{
						break;
					}
					foreach (object[] array2 in rows)
					{
						VersionedId versionedId = array2[0] as VersionedId;
						string text = array2[1] as string;
						ExDateTime creationTime = (ExDateTime)array2[2];
						if (versionedId != null)
						{
							if (string.IsNullOrEmpty(text))
							{
								LocalFolder.Tracer.TraceError<LocalFolder, StoreObjectId>((long)this.GetHashCode(), "{0}: Found item {1} without remote item id.", this, versionedId.ObjectId);
							}
							else
							{
								LocalFolder.Tracer.TraceDebug<LocalFolder, StoreObjectId, string>((long)this.GetHashCode(), "{0}: Found item {1} with remote item id {2}.", this, versionedId.ObjectId, text);
								LocalFolder.LocalItem localItem = new LocalFolder.LocalItem(versionedId, creationTime);
								try
								{
									dictionary.Add(text, localItem);
								}
								catch (ArgumentException)
								{
									LocalFolder.Tracer.TraceError<LocalFolder, string, StoreObjectId>((long)this.GetHashCode(), "{0}: there is already a local item with the same remote id. remoteId={1}, localId={2}", this, text, versionedId.ObjectId);
									flag = true;
									LocalFolder.LocalItem localItem2;
									if (dictionary.TryGetValue(text, out localItem2))
									{
										int num = localItem2.CreationTime.CompareTo(localItem.CreationTime);
										if (num < 0)
										{
											dictionary.Remove(text);
											this.SelectItemToDelete(localItem2.Id);
											dictionary.Add(text, localItem);
										}
										else
										{
											this.SelectItemToDelete(localItem.Id);
										}
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					LocalFolder.Tracer.TraceError<LocalFolder>((long)this.GetHashCode(), "{0}: Duplicates have been found. Deleting items now.", this);
					this.DeleteSelectedItems();
				}
				result = dictionary;
			}
			return result;
		}

		private const int ItemsQueryBatch = 200;

		private const int DeleteBatchNumber = 100;

		private static readonly Trace Tracer = ExTraceGlobals.LocalFolderTracer;

		private static PropertyDefinition[] extraProperties = new PropertyDefinition[]
		{
			FolderSchema.DisplayName,
			SharingSchema.ExternalSharingLevelOfDetails,
			SharingSchema.ExternalSharingSyncState
		};

		private readonly MailboxSession mailboxSession;

		private Dictionary<string, LocalFolder.LocalItem> localItems;

		private List<StoreId> itemsToDelete = new List<StoreId>();

		private Folder folder;

		private string remoteFolderId;

		private Item binding;

		private delegate void ProcessFolderDelegate(LocalFolder folder);

		public delegate void ProcessItem(StoreId localItemId);

		private sealed class LocalItem
		{
			public LocalItem(StoreId id, ExDateTime creationTime)
			{
				this.Id = id;
				this.CreationTime = creationTime;
			}

			public StoreId Id { get; private set; }

			public ExDateTime CreationTime { get; private set; }
		}
	}
}
