using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDumpsterItemOperations
	{
		StoreObjectId RecoverableItemsDeletionsFolderId { get; }

		StoreObjectId RecoverableItemsVersionsFolderId { get; }

		StoreObjectId RecoverableItemsPurgesFolderId { get; }

		StoreObjectId RecoverableItemsDiscoveryHoldsFolderId { get; }

		StoreObjectId RecoverableItemsRootFolderId { get; }

		StoreObjectId CalendarLoggingFolderId { get; }

		StoreObjectId AuditsFolderId { get; }

		StoreObjectId AdminAuditLogsFolderId { get; }

		COWResults Results { get; }

		StoreSession StoreSession { get; }

		bool IsDumpsterFolder(MailboxSession sessionWithBestAccess, StoreObjectId itemId);

		StoreObjectId CopyItemToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, ICoreItem item);

		void CopyItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds);

		void CopyItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, bool forceNonIPM);

		void MoveItemsToDumpster(MailboxSession sessionWithBestAccess, StoreObjectId destinationFolderId, StoreObjectId[] itemIds);

		void RollbackItemVersion(MailboxSession sessionWithBestAccess, CoreItem itemUpdated, StoreObjectId itemIdToRollback);

		bool IsDumpsterOverWarningQuota(COWSettings settings);

		void DisableCalendarLogging();

		bool IsDumpsterOverCalendarLoggingQuota(MailboxSession sessionWithBestAccess, COWSettings settings);

		bool IsAuditFolder(StoreObjectId folderId);
	}
}
