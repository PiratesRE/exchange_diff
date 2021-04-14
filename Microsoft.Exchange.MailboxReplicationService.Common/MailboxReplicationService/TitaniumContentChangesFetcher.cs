using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class TitaniumContentChangesFetcher : DisposeTrackableBase, IContentChangesFetcher, IDisposable
	{
		public TitaniumContentChangesFetcher(ISourceFolder folder, MapiFolder mapiFolder, MailboxProviderBase mailbox)
		{
			this.folder = folder;
			this.mapiFolder = mapiFolder;
			this.mailbox = mailbox;
		}

		FolderChangesManifest IContentChangesFetcher.EnumerateContentChanges(SyncContentsManifestState syncState, EnumerateContentChangesFlags enumerateFlags, int maxChanges)
		{
			FolderChangesManifest folderChangesManifest = new FolderChangesManifest(this.folder.GetFolderId());
			bool flag = enumerateFlags.HasFlag(EnumerateContentChangesFlags.Catchup);
			ManifestContentsCallback manifestContentsCallback = new ManifestContentsCallback(this.folder.GetFolderId(), false);
			manifestContentsCallback.InitializeNextPage(folderChangesManifest, maxChanges);
			SyncConfigFlags syncConfigFlags = SyncConfigFlags.ReadState | SyncConfigFlags.Associated | SyncConfigFlags.Normal | SyncConfigFlags.OnlySpecifiedProps;
			if (flag)
			{
				syncConfigFlags |= SyncConfigFlags.Catchup;
			}
			PropTag[] propsInclude = new PropTag[]
			{
				PropTag.EntryId,
				PropTag.MessageSize
			};
			MapiSynchronizer mapiSynchronizer;
			using (this.mailbox.RHTracker.Start())
			{
				mapiSynchronizer = this.mapiFolder.CreateContentsSynchronizer();
			}
			using (MapiManifestCollector mapiManifestCollector = new MapiManifestCollector(this.mapiFolder, manifestContentsCallback))
			{
				using (mapiSynchronizer)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						if (syncState.Data != null)
						{
							memoryStream.Write(syncState.Data, 0, syncState.Data.Length);
							memoryStream.Position = 0L;
						}
						Restriction restriction = ((this.mailbox.Options & MailboxOptions.IgnoreExtendedRuleFAIs) != MailboxOptions.None) ? ContentChangesFetcherUtils.ExcludeAllRulesRestriction : ContentChangesFetcherUtils.ExcludeV40RulesRestriction;
						using (this.mailbox.RHTracker.Start())
						{
							mapiSynchronizer.Config(memoryStream, syncConfigFlags, mapiManifestCollector, restriction, propsInclude, null);
							while (mapiSynchronizer.Synchronize() != 0)
							{
							}
						}
						syncState.Data = memoryStream.ToArray();
					}
				}
			}
			if (!flag && MrsTracer.Provider.IsEnabled(TraceType.DebugTrace))
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

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TitaniumContentChangesFetcher>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		private readonly ISourceFolder folder;

		private readonly MapiFolder mapiFolder;

		private readonly MailboxProviderBase mailbox;
	}
}
