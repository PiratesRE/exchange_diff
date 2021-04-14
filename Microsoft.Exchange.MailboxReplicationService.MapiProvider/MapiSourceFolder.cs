using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MapiSourceFolder : MapiFolder, ISourceFolder, IFolder, IDisposable
	{
		void ISourceFolder.CopyTo(IFxProxy destFolderProxy, CopyPropertiesFlags flags, PropTag[] excludeTags)
		{
			MrsTracer.Provider.Function("MapiSourceFolder.CopyTo", new object[0]);
			if ((base.Mailbox.Flags & LocalMailboxFlags.StripLargeRulesForDownlevelTargets) != LocalMailboxFlags.None)
			{
				flags |= CopyPropertiesFlags.StripLargeRulesForDownlevelTargets;
			}
			bool flag = false;
			try
			{
				using (base.Mailbox.RHTracker.Start())
				{
					using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(destFolderProxy, false, new Func<IDisposable>(base.Mailbox.RHTracker.StartExclusive), new Action<uint>(base.Mailbox.RHTracker.Charge)))
					{
						base.Folder.ExportObject(fxProxyBudgetWrapper, flags, excludeTags);
					}
				}
				flag = true;
				destFolderProxy.Flush();
			}
			catch (LocalizedException)
			{
				if (!flag)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(destFolderProxy.Flush), null);
				}
				throw;
			}
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			MrsTracer.Provider.Function("MapiSourceFolder.CopyTo", new object[0]);
			bool flag = false;
			try
			{
				using (base.Mailbox.RHTracker.Start())
				{
					using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(destFolderProxy, false, new Func<IDisposable>(base.Mailbox.RHTracker.StartExclusive), new Action<uint>(base.Mailbox.RHTracker.Charge)))
					{
						base.Folder.ExportMessages(fxProxyBudgetWrapper, flags, entryIds);
					}
				}
				flag = true;
				destFolderProxy.Flush();
			}
			catch (LocalizedException)
			{
				if (!flag)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(destFolderProxy.Flush), null);
				}
				throw;
			}
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			if (!base.Folder.IsContentAvailable)
			{
				return new FolderChangesManifest(base.FolderId);
			}
			SyncContentsManifestState syncState = ((MapiSourceMailbox)base.Mailbox).SyncState[base.FolderId];
			bool flag = maxChanges != 0;
			if (!flag || flags.HasFlag(EnumerateContentChangesFlags.FirstPage))
			{
				MapiStore mapiStore = base.Mailbox.MapiStore;
				int[] array = new int[4];
				array[0] = 8;
				if (mapiStore.IsVersionGreaterOrEqualThan(array))
				{
					this.contentChangesFetcher = new ManifestContentChangesFetcher(this, base.Folder, base.Mailbox, flag);
				}
				else
				{
					this.contentChangesFetcher = new TitaniumContentChangesFetcher(this, base.Folder, base.Mailbox);
				}
			}
			return this.contentChangesFetcher.EnumerateContentChanges(syncState, flags, maxChanges);
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			throw new NotImplementedException();
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			throw new NotImplementedException();
		}

		internal override void Config(byte[] folderId, MapiFolder folder, MapiMailbox mailbox)
		{
			base.Config(folderId, folder, mailbox);
			base.Folder.AllowWarnings = true;
		}

		internal void CopyBatch(IFxProxyPool proxyPool, List<MessageRec> batch)
		{
			MapiSourceFolder.<>c__DisplayClass1 CS$<>8__locals1 = new MapiSourceFolder.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			if (batch.Count == 0)
			{
				return;
			}
			byte[][] array = new byte[batch.Count][];
			for (int i = 0; i < batch.Count; i++)
			{
				array[i] = batch[i].EntryId;
			}
			CS$<>8__locals1.flags = CopyMessagesFlags.SendEntryId;
			using (IMapiFxProxy destFolderProxy = proxyPool.GetFolderProxy(base.FolderId))
			{
				if (destFolderProxy == null)
				{
					MrsTracer.Provider.Warning("Destination folder {0} does not exist.", new object[]
					{
						TraceUtils.DumpEntryId(base.FolderId)
					});
				}
				else
				{
					MapiUtils.ProcessMapiCallInBatches<byte[]>(array, delegate(byte[][] smallBatch)
					{
						using (CS$<>8__locals1.<>4__this.Mailbox.RHTracker.Start())
						{
							using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(destFolderProxy, false, new Func<IDisposable>(CS$<>8__locals1.<>4__this.Mailbox.RHTracker.StartExclusive), new Action<uint>(CS$<>8__locals1.<>4__this.Mailbox.RHTracker.Charge)))
							{
								CS$<>8__locals1.<>4__this.Folder.ExportMessages(fxProxyBudgetWrapper, CS$<>8__locals1.flags, smallBatch);
							}
						}
					});
				}
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.contentChangesFetcher != null)
			{
				this.contentChangesFetcher.Dispose();
				this.contentChangesFetcher = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiSourceFolder>(this);
		}

		private IContentChangesFetcher contentChangesFetcher;
	}
}
