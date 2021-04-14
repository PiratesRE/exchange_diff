using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorFolder : AnchorStoreObject
	{
		internal AnchorFolder(AnchorContext context, Folder folder)
		{
			AnchorUtil.ThrowOnNullArgument(context, "context");
			AnchorUtil.ThrowOnNullArgument(folder, "folder");
			base.AnchorContext = context;
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

		public static bool RemoveFolder(AnchorContext context, MailboxSession mailboxSession, string folderName)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			StoreObjectId folderId = AnchorFolder.GetFolderId(context, mailboxSession, defaultFolderId, folderName);
			if (folderId == null)
			{
				context.Logger.Log(MigrationEventType.Verbose, "couldn't find folder with name {0} treating as success", new object[]
				{
					folderName
				});
				return true;
			}
			using (Folder folder = Folder.Bind(mailboxSession, folderId, AnchorFolder.FolderIdPropertyDefinition))
			{
				context.Logger.Log(MigrationEventType.Information, "About to remove all messages & subfolders from {0} with id {1}", new object[]
				{
					folderName,
					folderId
				});
				GroupOperationResult groupOperationResult = folder.DeleteAllObjects(DeleteItemFlags.HardDelete, true);
				if (groupOperationResult.OperationResult != OperationResult.Succeeded)
				{
					context.Logger.Log(MigrationEventType.Warning, "unsuccessfully removed messages & subfolders from {0} with id {1} with result {2}", new object[]
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
				context.Logger.Log(MigrationEventType.Information, "About to remove folder {0} with id {1}", new object[]
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
					context.Logger.Log(MigrationEventType.Warning, "unsuccessfully removed folder {0} with id {1} with result {2}", new object[]
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
			if (AnchorUtil.IsTransientException(folderSaveResult.Exception))
			{
				throw new MigrationTransientException(folderSaveResult.Exception.LocalizedString, folderSaveResult.Exception);
			}
			throw new MigrationPermanentException(folderSaveResult.Exception.LocalizedString, folderSaveResult.Exception);
		}

		internal static AnchorFolder GetFolder(AnchorContext context, MailboxSession mailboxSession, string folderName)
		{
			AnchorFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = AnchorFolder.GetFolder(context, mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Root), folderName);
				disposeGuard.Add<Folder>(folder);
				AnchorFolder anchorFolder = new AnchorFolder(context, folder);
				disposeGuard.Success();
				result = anchorFolder;
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
			return DisposeTracker.Get<AnchorFolder>(this);
		}

		private static StoreObjectId GetFolderId(AnchorContext context, MailboxSession mailboxSession, StoreObjectId rootFolderId, string folderName)
		{
			AnchorUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			AnchorUtil.ThrowOnNullArgument(rootFolderId, "rootFolderId");
			AnchorUtil.ThrowOnNullArgument(folderName, "folderName");
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
				context.Logger.Log(MigrationEventType.Warning, "Couldn't find subfolder {0}", new object[]
				{
					folderName
				});
			}
			catch (ObjectNotFoundException exception)
			{
				context.Logger.Log(MigrationEventType.Warning, exception, "Folder {0} missing, will try to create it", new object[]
				{
					folderName
				});
			}
			catch (StorageTransientException exception2)
			{
				context.Logger.Log(MigrationEventType.Warning, exception2, "Transient exception when trying to get Folder {0} will try to create it", new object[]
				{
					folderName
				});
			}
			return null;
		}

		private static Folder GetFolder(AnchorContext context, MailboxSession mailboxSession, StoreObjectId rootFolderId, string folderName)
		{
			AnchorUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			AnchorUtil.ThrowOnNullArgument(rootFolderId, "rootFolderId");
			AnchorUtil.ThrowOnNullArgument(folderName, "folderName");
			Folder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = null;
				StoreObjectId storeObjectId = AnchorFolder.GetFolderId(context, mailboxSession, rootFolderId, folderName);
				if (storeObjectId == null)
				{
					folder = Folder.Create(mailboxSession, rootFolderId, StoreObjectType.Folder, folderName, CreateMode.OpenIfExists);
					disposeGuard.Add<Folder>(folder);
					folder.Save();
					folder.Load(AnchorFolder.FolderIdPropertyDefinition);
					storeObjectId = folder.Id.ObjectId;
				}
				if (folder == null)
				{
					folder = Folder.Bind(mailboxSession, storeObjectId, AnchorFolder.FolderIdPropertyDefinition);
					disposeGuard.Add<Folder>(folder);
				}
				disposeGuard.Success();
				result = folder;
			}
			return result;
		}

		internal static readonly PropertyDefinition[] FolderIdPropertyDefinition = AnchorHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.DisplayName
			},
			AnchorStoreObject.IdPropertyDefinition
		});
	}
}
