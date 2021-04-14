using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationFolder : MigrationStoreObject
	{
		internal MigrationFolder(Folder folder)
		{
			MigrationUtil.ThrowOnNullArgument(folder, "folder");
			this.Folder = folder;
		}

		public override string Name
		{
			get
			{
				return this.Folder.DisplayName;
			}
		}

		internal Folder Folder { get; private set; }

		protected override StoreObject StoreObject
		{
			get
			{
				return this.Folder;
			}
		}

		public static bool RemoveFolder(MailboxSession mailboxSession, MigrationFolderName folderName)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			StoreObjectId folderId = MigrationFolder.GetFolderId(mailboxSession, defaultFolderId, folderName.ToString());
			if (folderId == null)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "couldn't find folder with name {0} treating as success", new object[]
				{
					folderName
				});
				return true;
			}
			using (Folder folder = Folder.Bind(mailboxSession, folderId, MigrationFolder.FolderIdPropertyDefinition))
			{
				MigrationLogger.Log(MigrationEventType.Information, "About to remove all messages & subfolders from {0} with id {1}", new object[]
				{
					folderName,
					folderId
				});
				GroupOperationResult groupOperationResult = folder.DeleteAllObjects(DeleteItemFlags.HardDelete, true);
				if (groupOperationResult.OperationResult != OperationResult.Succeeded)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "unsuccessfully removed messages & subfolders from {0} with id {1} with result {2}", new object[]
					{
						folderName,
						folderId,
						groupOperationResult
					});
					return false;
				}
			}
			bool result;
			using (Folder folder2 = Folder.Bind(mailboxSession, defaultFolderId))
			{
				MigrationLogger.Log(MigrationEventType.Information, "About to remove folder {0} with id {1}", new object[]
				{
					folderName,
					folderId
				});
				AggregateOperationResult aggregateOperationResult = folder2.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
				{
					folderId
				});
				if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "unsuccessfully removed folder {0} with id {1} with result {2}", new object[]
					{
						folderName,
						folderId,
						aggregateOperationResult
					});
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public override void Save(SaveMode saveMode)
		{
			FolderSaveResult folderSaveResult = this.Folder.Save(saveMode);
			if (folderSaveResult.OperationResult == OperationResult.Succeeded)
			{
				return;
			}
			if (MigrationUtil.IsTransientException(folderSaveResult.Exception))
			{
				throw new FailedToSaveFolderTransientException(folderSaveResult.Exception.LocalizedString, folderSaveResult.Exception);
			}
			throw new FailedToSaveFolderPermanentException(folderSaveResult.Exception.LocalizedString, folderSaveResult.Exception);
		}

		internal static MigrationFolder GetFolder(MailboxSession mailboxSession, MigrationFolderName folderName)
		{
			MigrationFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = MigrationFolder.GetFolder(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Root), folderName.ToString());
				disposeGuard.Add<Folder>(folder);
				MigrationFolder migrationFolder = new MigrationFolder(folder);
				disposeGuard.Success();
				result = migrationFolder;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Folder != null)
			{
				this.Folder.Dispose();
				this.Folder = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationFolder>(this);
		}

		private static StoreObjectId GetFolderId(MailboxSession mailboxSession, StoreObjectId rootFolderId, string folderName)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			MigrationUtil.ThrowOnNullArgument(rootFolderId, "rootFolderId");
			MigrationUtil.ThrowOnNullArgument(folderName, "folderName");
			try
			{
				using (Folder folder = Folder.Bind(mailboxSession, rootFolderId))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
					{
						FolderSchema.Id,
						StoreObjectSchema.DisplayName
					}))
					{
						QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.DisplayName, folderName);
						if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
						{
							object[][] rows = queryResult.GetRows(1);
							if (rows.Length > 0)
							{
								VersionedId versionedId = (VersionedId)rows[0][0];
								return versionedId.ObjectId;
							}
						}
					}
				}
				MigrationLogger.Log(MigrationEventType.Warning, "Couldn't find subfolder {0}", new object[]
				{
					folderName
				});
			}
			catch (ObjectNotFoundException exception)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception, "Folder {0} missing, will try to create it", new object[]
				{
					folderName
				});
			}
			catch (StorageTransientException exception2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception2, "Transient exception when trying to get Folder {0} will try to create it", new object[]
				{
					folderName
				});
			}
			return null;
		}

		private static Folder GetFolder(MailboxSession mailboxSession, StoreObjectId rootFolderId, string folderName)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			MigrationUtil.ThrowOnNullArgument(rootFolderId, "rootFolderId");
			MigrationUtil.ThrowOnNullArgument(folderName, "folderName");
			Folder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = null;
				StoreObjectId storeObjectId = MigrationFolder.GetFolderId(mailboxSession, rootFolderId, folderName);
				if (storeObjectId == null)
				{
					folder = Folder.Create(mailboxSession, rootFolderId, StoreObjectType.Folder, folderName, CreateMode.OpenIfExists);
					disposeGuard.Add<Folder>(folder);
					folder.Save();
					folder.Load(MigrationFolder.FolderIdPropertyDefinition);
					storeObjectId = folder.Id.ObjectId;
				}
				if (folder == null)
				{
					folder = Folder.Bind(mailboxSession, storeObjectId, MigrationFolder.FolderIdPropertyDefinition);
					disposeGuard.Add<Folder>(folder);
				}
				disposeGuard.Success();
				result = folder;
			}
			return result;
		}

		internal static readonly PropertyDefinition[] FolderIdPropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.DisplayName
			},
			MigrationStoreObject.IdPropertyDefinition
		});
	}
}
