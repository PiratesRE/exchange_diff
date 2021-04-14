using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PublicFolderMailboxSynchronizerReference : DisposeTrackableBase
	{
		public PublicFolderMailboxSynchronizerReference(PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer, Action<PublicFolderMailboxSynchronizer> onDispose)
		{
			this.publicFolderMailboxSynchronizer = publicFolderMailboxSynchronizer;
			this.onDispose = onDispose;
		}

		public PublicFolderSyncJobState SyncJobState
		{
			get
			{
				return this.publicFolderMailboxSynchronizer.SyncJobState;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMailboxSynchronizerReference>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.onDispose(this.publicFolderMailboxSynchronizer);
				this.publicFolderMailboxSynchronizer = null;
			}
		}

		private PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer;

		private Action<PublicFolderMailboxSynchronizer> onDispose;
	}
}
