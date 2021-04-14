using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class ManifestContentChangesFetcher : DisposeTrackableBase, IContentChangesFetcher, IDisposable
	{
		public ManifestContentChangesFetcher(ISourceFolder folder, MapiFolder mapiFolder, MailboxProviderBase mailbox, bool isPagedEnumeration)
		{
			this.folder = folder;
			this.mapiFolder = mapiFolder;
			this.mailbox = mailbox;
			this.isPagedEnumeration = isPagedEnumeration;
		}

		FolderChangesManifest IContentChangesFetcher.EnumerateContentChanges(SyncContentsManifestState syncState, EnumerateContentChangesFlags flags, int maxChanges)
		{
			FolderChangesManifest folderChangesManifest = this.EnumerateChanges(syncState, flags, maxChanges);
			if (MrsTracer.Provider.IsEnabled(TraceType.DebugTrace))
			{
				int num;
				int num2;
				int num3;
				folderChangesManifest.GetMessageCounts(out num, out num2, out num3);
				MrsTracer.Provider.Debug("Discovered {0} new, {1} changed, {2} deleted, {3} read, {4} unread.", new object[]
				{
					num,
					num2,
					num3,
					folderChangesManifest.ReadMessages.Count,
					folderChangesManifest.UnreadMessages.Count
				});
			}
			return folderChangesManifest;
		}

		private FolderChangesManifest EnumerateChanges(SyncContentsManifestState syncState, EnumerateContentChangesFlags flags, int maxChanges)
		{
			FolderChangesManifest folderChangesManifest = new FolderChangesManifest(this.folder.GetFolderId());
			if (this.folder.GetFolderRec(null, GetFolderRecFlags.None).FolderType == FolderType.Search)
			{
				return folderChangesManifest;
			}
			this.ConfigureMapiManifest(syncState, folderChangesManifest, flags, maxChanges);
			ManifestStatus manifestStatus;
			do
			{
				manifestStatus = this.mapiManifest.Synchronize();
			}
			while (manifestStatus != ManifestStatus.Done && manifestStatus != ManifestStatus.Yielded);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.mapiManifest.GetState(memoryStream);
				syncState.Data = memoryStream.ToArray();
			}
			return folderChangesManifest;
		}

		private void ConfigureMapiManifest(SyncContentsManifestState syncState, FolderChangesManifest changes, EnumerateContentChangesFlags flags, int maxChanges)
		{
			if (this.isPagedEnumeration && !flags.HasFlag(EnumerateContentChangesFlags.FirstPage))
			{
				this.callback.InitializeNextPage(changes, maxChanges);
				return;
			}
			this.callback = new ManifestContentsCallback(this.folder.GetFolderId(), this.isPagedEnumeration);
			this.callback.InitializeNextPage(changes, maxChanges);
			ManifestConfigFlags manifestConfigFlags = ManifestConfigFlags.Associated | ManifestConfigFlags.Normal | ManifestConfigFlags.OrderByDeliveryTime;
			if (flags.HasFlag(EnumerateContentChangesFlags.Catchup))
			{
				manifestConfigFlags |= ManifestConfigFlags.Catchup;
			}
			Restriction restriction = ((this.mailbox.Options & MailboxOptions.IgnoreExtendedRuleFAIs) != MailboxOptions.None) ? ContentChangesFetcherUtils.ExcludeAllRulesRestriction : ContentChangesFetcherUtils.ExcludeV40RulesRestriction;
			using (this.mailbox.RHTracker.Start())
			{
				this.mapiManifest = this.mapiFolder.CreateExportManifest();
			}
			using (MemoryStream memoryStream = (syncState.Data != null) ? new MemoryStream(syncState.Data) : null)
			{
				using (this.mailbox.RHTracker.Start())
				{
					this.mapiManifest.Configure(manifestConfigFlags, restriction, memoryStream, this.callback, new PropTag[0]);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ManifestContentChangesFetcher>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.mapiManifest != null)
			{
				this.mapiManifest.Dispose();
				this.mapiManifest = null;
			}
		}

		private readonly ISourceFolder folder;

		private readonly MapiFolder mapiFolder;

		private readonly MailboxProviderBase mailbox;

		private readonly bool isPagedEnumeration;

		private MapiManifest mapiManifest;

		private ManifestContentsCallback callback;
	}
}
