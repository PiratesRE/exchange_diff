using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SiteMailboxSynchronizerReference : DisposeTrackableBase
	{
		public SiteMailboxSynchronizerReference(SiteMailboxSynchronizer siteMailboxSynchronizer, Action<SiteMailboxSynchronizer> onDispose)
		{
			this.siteMailboxSynchronizer = siteMailboxSynchronizer;
			this.onDispose = onDispose;
		}

		public bool TryToSyncNow()
		{
			return this.siteMailboxSynchronizer.TryToSyncNow();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SiteMailboxSynchronizerReference>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.onDispose(this.siteMailboxSynchronizer);
				this.siteMailboxSynchronizer = null;
			}
		}

		private readonly Action<SiteMailboxSynchronizer> onDispose;

		private SiteMailboxSynchronizer siteMailboxSynchronizer;
	}
}
