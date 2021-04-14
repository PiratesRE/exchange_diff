using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IStateStorage : INativeStateStorage, ISimpleStateStorage, IDisposeTrackable, IDisposable
	{
		bool IsDirty { get; }

		SyncProgress SyncProgress { get; }

		bool ForceRecoverySyncNext { get; }

		bool InitialSyncDone { get; }

		Exception Commit(bool commitState, MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete);

		void ReloadForRetry(EventHandler<RoundtripCompleteEventArgs> roundtripComplete);

		void SetSyncProgress(SyncProgress progress);

		bool ShouldPromoteItemTransientException(string cloudId, SyncTransientException exception);

		bool ShouldPromoteItemTransientException(StoreObjectId nativeId, SyncTransientException exception);

		bool ShouldPromoteFolderTransientException(string cloudId, SyncTransientException exception);

		bool ShouldPromoteFolderTransientException(StoreObjectId nativeId, SyncTransientException exception);

		bool TryAddFailedItem(string cloudId, string cloudFolderId);

		bool TryRemoveFailedItem(string cloudId);

		bool TryAddFailedFolder(string cloudId, string cloudFolderId);

		bool TryRemoveFailedFolder(string cloudId);

		void MarkInitialSyncDone();
	}
}
