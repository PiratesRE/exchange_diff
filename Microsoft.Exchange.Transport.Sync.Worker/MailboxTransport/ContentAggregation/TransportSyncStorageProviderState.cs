using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TransportSyncStorageProviderState : NativeSyncStorageProviderState
	{
		internal TransportSyncStorageProviderState(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, INativeStateStorage stateStorage, MailSubmitter mailSubmitter, SyncLogSession syncLogSession, bool underRecovery) : base(syncMailboxSession, subscription, stateStorage, syncLogSession, underRecovery)
		{
			this.mailSubmitter = mailSubmitter;
			this.verifiedExistingFolders = new Dictionary<string, string>(3);
		}

		internal MailSubmitter MailSubmitter
		{
			get
			{
				base.CheckDisposed();
				return this.mailSubmitter;
			}
		}

		internal override StoreObjectId EnsureInboxFolder(SyncChangeEntry change)
		{
			base.CheckDisposed();
			if (change.SchemaType == SchemaType.Folder)
			{
				return base.EnsureDefaultFolder(DefaultFolderType.Inbox);
			}
			return null;
		}

		internal void AddExistingFolder(string cloudId, string folderHexId)
		{
			this.verifiedExistingFolders.Add(cloudId, folderHexId);
		}

		internal bool TryGetExistingFolder(string cloudId, out string folderHexId)
		{
			return this.verifiedExistingFolders.TryGetValue(cloudId, out folderHexId);
		}

		internal override bool IsInboxFolderId(StoreObjectId itemId)
		{
			base.CheckDisposed();
			return itemId == null || base.IsInboxFolderId(itemId);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TransportSyncStorageProviderState>(this);
		}

		private const int EstimatedFolderHexIdEntriesInCache = 3;

		private readonly MailSubmitter mailSubmitter;

		private readonly Dictionary<string, string> verifiedExistingFolders;
	}
}
