using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class COWSessionBase : DisposableObject, IDumpsterItemOperations
	{
		protected COWSessionBase()
		{
			this.inCallback = false;
			this.folderIdState = FolderIdState.FolderIdDefered;
		}

		protected virtual bool InCallback
		{
			get
			{
				return this.inCallback;
			}
			set
			{
				this.inCallback = value;
			}
		}

		public StoreObjectId RecoverableItemsDeletionsFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsDeletionsFolderId");
				return this.recoverableItemsDeletionsFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsDeletionsFolderId");
				this.recoverableItemsDeletionsFolderId = value;
			}
		}

		public StoreObjectId RecoverableItemsVersionsFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsVersionsFolderId");
				return this.recoverableItemsVersionsFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsVersionsFolderId");
				this.recoverableItemsVersionsFolderId = value;
			}
		}

		public StoreObjectId RecoverableItemsPurgesFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsPurgesFolderId");
				return this.recoverableItemsPurgesFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsPurgesFolderId");
				this.recoverableItemsPurgesFolderId = value;
			}
		}

		public StoreObjectId RecoverableItemsDiscoveryHoldsFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsDiscoveryHoldsFolderId");
				return this.recoverableItemsDiscoveryHoldsFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsDiscoveryHoldsFolderId");
				this.recoverableItemsDiscoveryHoldsFolderId = value;
			}
		}

		public StoreObjectId RecoverableItemsMigratedMessagesFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsMigratedMessagesFolderId");
				return this.recoverableItemsMigratedMessagesFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsMigratedMessagesFolderId");
				this.recoverableItemsMigratedMessagesFolderId = value;
			}
		}

		public StoreObjectId RecoverableItemsRootFolderId
		{
			get
			{
				this.CheckDisposed("get_RecoverableItemsRootFolderId");
				return this.recoverableItemsRootFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_RecoverableItemsRootFolderId");
				this.recoverableItemsRootFolderId = value;
			}
		}

		public StoreObjectId CalendarLoggingFolderId
		{
			get
			{
				this.CheckDisposed("get_CalendarLoggingFolderId");
				return this.calendarLoggingFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_CalendarLoggingFolderId");
				this.calendarLoggingFolderId = value;
			}
		}

		public StoreObjectId AuditsFolderId
		{
			get
			{
				this.CheckDisposed("get_AuditsFolderId");
				return this.auditsFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_AuditsFolderId");
				this.auditsFolderId = value;
			}
		}

		public StoreObjectId AdminAuditLogsFolderId
		{
			get
			{
				this.CheckDisposed("get_AdminAuditLogsFolderId");
				return this.adminAuditLogsFolderId;
			}
			protected set
			{
				this.CheckDisposed("set_AdminAuditLogsFolderId");
				this.adminAuditLogsFolderId = value;
			}
		}

		public COWResults Results
		{
			get
			{
				this.CheckDisposed("get_Results");
				return this.results;
			}
			protected set
			{
				this.CheckDisposed("set_Results");
				this.results = value;
			}
		}

		public abstract StoreSession StoreSession { get; }

		protected FolderIdState FolderIdState
		{
			get
			{
				this.CheckDisposed("get_FolderIdState");
				return this.folderIdState;
			}
			set
			{
				this.CheckDisposed("set_FolderIdState");
				this.folderIdState = value;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<COWSessionBase>(this);
		}

		public GroupOperationResult GetCallbackResults()
		{
			this.CheckDisposed("GetCallbackResults");
			if (this.results == null)
			{
				return null;
			}
			return this.results.GetResults();
		}

		public bool IsDumpsterFolder(MailboxSession sessionWithBestAccess, StoreObjectId folderId)
		{
			this.CheckDisposed("IsDumpsterFolder");
			if (folderId == null)
			{
				return false;
			}
			this.CheckFolderInitState(sessionWithBestAccess);
			return folderId.Equals(this.recoverableItemsRootFolderId) || folderId.Equals(this.recoverableItemsDeletionsFolderId) || folderId.Equals(this.recoverableItemsVersionsFolderId) || folderId.Equals(this.recoverableItemsPurgesFolderId) || folderId.Equals(this.recoverableItemsDiscoveryHoldsFolderId) || folderId.Equals(this.calendarLoggingFolderId) || folderId.Equals(this.recoverableItemsMigratedMessagesFolderId) || folderId.Equals(this.auditsFolderId) || folderId.Equals(this.adminAuditLogsFolderId);
		}

		public virtual bool IsAuditFolder(StoreObjectId folderId)
		{
			return false;
		}

		protected void CheckFolderInitState(MailboxSession sessionWithBestAccess)
		{
			if (this.folderIdState == FolderIdState.FolderIdDefered)
			{
				this.folderIdState = this.InternalInitFolders(sessionWithBestAccess);
			}
			if (this.folderIdState != FolderIdState.FolderIdSuccess)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sessionWithBestAccess.GetHashCode(), "Recoverable Items folders/folderIds missing");
				throw new RecoverableItemsAccessDeniedException("Recoverable Items");
			}
		}

		protected abstract FolderIdState InternalInitFolders(MailboxSession sessionWithBestAccess);

		public virtual StoreObjectId CopyItemToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, ICoreItem item)
		{
			throw new NotImplementedException();
		}

		public void CopyItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds)
		{
			this.CheckDisposed("CopyItemsToDumpster");
			this.InternalCopyOrMoveItemsToDumpster(sessionWithBestAccess, destinationFolderId, itemIds, false, false, false);
		}

		public void CopyItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, bool forceNonIPM)
		{
			this.CheckDisposed("CopyItemsToDumpster");
			this.InternalCopyOrMoveItemsToDumpster(sessionWithBestAccess, destinationFolderId, itemIds, false, forceNonIPM, false);
		}

		public void MoveItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds)
		{
			this.CheckDisposed("MoveItemsToDumpster");
			this.InternalCopyOrMoveItemsToDumpster(sessionWithBestAccess, destinationFolderId, itemIds, true, false, false);
		}

		protected virtual IList<StoreObjectId> InternalCopyOrMoveItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, bool moveRequest, bool forceNonIPM, bool returnNewItemIds)
		{
			throw new NotImplementedException();
		}

		public virtual void RollbackItemVersion(MailboxSession sessionWithBestAccess, CoreItem itemUpdated, StoreObjectId itemIdToRollback)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDumpsterOverWarningQuota(COWSettings settings)
		{
			throw new NotImplementedException();
		}

		public virtual void DisableCalendarLogging()
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDumpsterOverCalendarLoggingQuota(MailboxSession sessionWithBestAccess, COWSettings settings)
		{
			throw new NotImplementedException();
		}

		internal static COWTriggerAction GetTriggerAction(FolderChangeOperation operation)
		{
			EnumValidator.ThrowIfInvalid<FolderChangeOperation>(operation, "operation");
			switch (operation)
			{
			case FolderChangeOperation.Copy:
				return COWTriggerAction.Copy;
			case FolderChangeOperation.Move:
				return COWTriggerAction.Move;
			case FolderChangeOperation.MoveToDeletedItems:
				return COWTriggerAction.MoveToDeletedItems;
			case FolderChangeOperation.SoftDelete:
				return COWTriggerAction.SoftDelete;
			case FolderChangeOperation.HardDelete:
				return COWTriggerAction.HardDelete;
			case FolderChangeOperation.DoneWithMessageDelete:
				return COWTriggerAction.DoneWithMessageDelete;
			default:
				throw new NotSupportedException("Invalid folder change operation");
			}
		}

		internal static bool IsDeleteOperation(COWTriggerAction operation)
		{
			return operation == COWTriggerAction.SoftDelete || operation == COWTriggerAction.HardDelete;
		}

		internal static List<StoreObjectId> InternalFilterFolders(ICollection<StoreObjectId> itemIds)
		{
			List<StoreObjectId> list = new List<StoreObjectId>(1);
			foreach (StoreObjectId storeObjectId in itemIds)
			{
				if (Folder.IsFolderId(storeObjectId) && !list.Contains(storeObjectId))
				{
					list.Add(storeObjectId);
				}
			}
			return list;
		}

		internal static List<StoreObjectId> InternalFilterItems(ICollection<StoreObjectId> itemIds)
		{
			HashSet<StoreObjectId> hashSet = new HashSet<StoreObjectId>(itemIds);
			List<StoreObjectId> list = new List<StoreObjectId>(hashSet.Count);
			foreach (StoreObjectId storeObjectId in hashSet)
			{
				if (!Folder.IsFolderId(storeObjectId))
				{
					list.Add(storeObjectId);
				}
			}
			return list;
		}

		private bool inCallback;

		private StoreObjectId recoverableItemsDeletionsFolderId;

		private StoreObjectId recoverableItemsVersionsFolderId;

		private StoreObjectId recoverableItemsPurgesFolderId;

		private StoreObjectId recoverableItemsDiscoveryHoldsFolderId;

		private StoreObjectId recoverableItemsMigratedMessagesFolderId;

		private StoreObjectId recoverableItemsRootFolderId;

		private StoreObjectId calendarLoggingFolderId;

		private StoreObjectId auditsFolderId;

		private StoreObjectId adminAuditLogsFolderId;

		private COWResults results;

		protected FolderIdState folderIdState;
	}
}
