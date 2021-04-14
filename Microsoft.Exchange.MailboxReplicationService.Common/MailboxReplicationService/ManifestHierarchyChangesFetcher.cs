using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class ManifestHierarchyChangesFetcher : DisposeTrackableBase, IHierarchyChangesFetcher, IDisposable
	{
		public ManifestHierarchyChangesFetcher(MapiStore mapiStore, MailboxProviderBase mailbox, bool isPagedEnumeration)
		{
			this.mapiFolder = mapiStore.GetRootFolder();
			this.mailbox = mailbox;
			this.isPagedEnumeration = isPagedEnumeration;
		}

		MailboxChangesManifest IHierarchyChangesFetcher.EnumerateHierarchyChanges(SyncHierarchyManifestState hierState, EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			MailboxChangesManifest mailboxChangesManifest = this.EnumerateChanges(hierState, flags, maxChanges);
			MrsTracer.Provider.Debug("Discovered {0} changed, {1} deleted", new object[]
			{
				mailboxChangesManifest.ChangedFolders.Count,
				mailboxChangesManifest.DeletedFolders.Count
			});
			return mailboxChangesManifest;
		}

		private MailboxChangesManifest EnumerateChanges(SyncHierarchyManifestState hierState, EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			this.ConfigureMapiManifest(hierState, mailboxChangesManifest, flags, maxChanges);
			ManifestStatus manifestStatus;
			do
			{
				manifestStatus = this.mapiManifest.Synchronize();
			}
			while (manifestStatus != ManifestStatus.Done && manifestStatus != ManifestStatus.Yielded);
			byte[] idsetGiven;
			byte[] cnsetSeen;
			this.mapiManifest.GetState(out idsetGiven, out cnsetSeen);
			hierState.IdsetGiven = idsetGiven;
			hierState.CnsetSeen = cnsetSeen;
			return mailboxChangesManifest;
		}

		private void ConfigureMapiManifest(SyncHierarchyManifestState syncState, MailboxChangesManifest changes, EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			if (this.isPagedEnumeration && !flags.HasFlag(EnumerateHierarchyChangesFlags.FirstPage))
			{
				this.callback.InitializeNextPage(changes, maxChanges);
				return;
			}
			this.callback = new ManifestHierarchyCallback(this.isPagedEnumeration);
			this.callback.InitializeNextPage(changes, maxChanges);
			SyncConfigFlags syncConfigFlags = SyncConfigFlags.ManifestHierReturnDeletedEntryIds;
			if (((this.mailbox.ServerVersion >= Server.E14MinVersion && this.mailbox.ServerVersion < Server.E15MinVersion) || (long)this.mailbox.ServerVersion >= ManifestHierarchyChangesFetcher.E15MinVersionSupportsOnlySpecifiedPropsForHierarchy) && !this.mailbox.IsPureMAPI)
			{
				syncConfigFlags |= SyncConfigFlags.OnlySpecifiedProps;
			}
			using (this.mailbox.RHTracker.Start())
			{
				this.mapiManifest = this.mapiFolder.CreateExportHierarchyManifestEx(syncConfigFlags, syncState.IdsetGiven, syncState.CnsetSeen, this.callback, new PropTag[]
				{
					PropTag.EntryId
				}, null);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ManifestHierarchyChangesFetcher>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.mapiManifest != null)
				{
					this.mapiManifest.Dispose();
					this.mapiManifest = null;
				}
				if (this.mapiFolder != null)
				{
					this.mapiFolder.Dispose();
					this.mapiFolder = null;
				}
			}
		}

		private static readonly long E15MinVersionSupportsOnlySpecifiedPropsForHierarchy = (long)new ServerVersion(15, 0, 922, 0).ToInt();

		private readonly MailboxProviderBase mailbox;

		private readonly bool isPagedEnumeration;

		private MapiFolder mapiFolder;

		private MapiHierarchyManifestEx mapiManifest;

		private ManifestHierarchyCallback callback;
	}
}
