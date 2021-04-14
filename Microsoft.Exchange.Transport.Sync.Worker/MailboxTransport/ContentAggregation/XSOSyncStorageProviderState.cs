using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class XSOSyncStorageProviderState : NativeSyncStorageProviderState, ISyncSourceSession
	{
		internal XSOSyncStorageProviderState(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, INativeStateStorage stateStorage, SyncLogSession syncLogSession, bool underRecovery) : base(syncMailboxSession, subscription, stateStorage, syncLogSession, underRecovery)
		{
			this.bulkAutomaticLink = new BulkAutomaticLink(syncMailboxSession.MailboxSession);
		}

		string ISyncSourceSession.Protocol
		{
			get
			{
				base.CheckDisposed();
				return "LocalExchange";
			}
		}

		string ISyncSourceSession.SessionId
		{
			get
			{
				base.CheckDisposed();
				return string.Empty;
			}
		}

		string ISyncSourceSession.Server
		{
			get
			{
				base.CheckDisposed();
				return string.Empty;
			}
		}

		internal BulkAutomaticLink BulkAutomaticLink
		{
			get
			{
				base.CheckDisposed();
				return this.bulkAutomaticLink;
			}
		}

		internal override StoreObjectId EnsureInboxFolder(SyncChangeEntry change)
		{
			base.CheckDisposed();
			return base.EnsureDefaultFolder(DefaultFolderType.Inbox);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.bulkAutomaticLink != null)
			{
				this.bulkAutomaticLink.Dispose();
				this.bulkAutomaticLink = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOSyncStorageProviderState>(this);
		}

		private const string XSOComponentId = "LocalExchange";

		private BulkAutomaticLink bulkAutomaticLink;
	}
}
