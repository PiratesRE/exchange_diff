using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class StorageSourceFolder : StorageFolder, ISourceFolder, IFolder, IDisposable
	{
		private MapiFolder MapiFolder
		{
			get
			{
				PersistablePropertyBag persistablePropertyBag = (PersistablePropertyBag)base.CoreFolder.PropertyBag;
				return (MapiFolder)persistablePropertyBag.MapiProp;
			}
		}

		void ISourceFolder.CopyTo(IFxProxy destFolderProxy, CopyPropertiesFlags flags, PropTag[] excludeTags)
		{
			MrsTracer.Provider.Function("StorageSourceFolder.CopyTo({0})", new object[]
			{
				base.DisplayNameForTracing
			});
			if ((base.Mailbox.Flags & LocalMailboxFlags.StripLargeRulesForDownlevelTargets) != LocalMailboxFlags.None)
			{
				flags |= CopyPropertiesFlags.StripLargeRulesForDownlevelTargets;
			}
			bool exportCompleted = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				using (this.Mailbox.RHTracker.Start())
				{
					using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(destFolderProxy, false, new Func<IDisposable>(this.Mailbox.RHTracker.StartExclusive), new Action<uint>(this.Mailbox.RHTracker.Charge)))
					{
						this.MapiFolder.ExportObject(fxProxyBudgetWrapper, flags, excludeTags);
					}
				}
				exportCompleted = true;
				destFolderProxy.Flush();
			}, delegate(Exception ex)
			{
				if (!exportCompleted)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(destFolderProxy.Flush), null);
				}
				return false;
			});
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			MrsTracer.Provider.Function("StorageSourceFolder.ExportMessages({0})", new object[]
			{
				base.DisplayNameForTracing
			});
			bool exportCompleted = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				using (this.Mailbox.RHTracker.Start())
				{
					using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(destFolderProxy, false, new Func<IDisposable>(this.Mailbox.RHTracker.StartExclusive), new Action<uint>(this.Mailbox.RHTracker.Charge)))
					{
						this.MapiFolder.ExportMessages(fxProxyBudgetWrapper, flags, entryIds);
					}
				}
				exportCompleted = true;
				destFolderProxy.Flush();
			}, delegate(Exception ex)
			{
				if (!exportCompleted)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(destFolderProxy.Flush), null);
				}
				return false;
			});
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			MrsTracer.Provider.Function("StorageSourceFolder.EnumerateChanges({0}, 0x{1:x}, {2})", new object[]
			{
				base.DisplayNameForTracing,
				(int)flags,
				maxChanges
			});
			SyncContentsManifestState syncState = ((StorageSourceMailbox)base.Mailbox).SyncState[base.FolderId];
			bool flag = maxChanges != 0;
			if (!flag || flags.HasFlag(EnumerateContentChangesFlags.FirstPage))
			{
				this.contentChangesFetcher = new ManifestContentChangesFetcher(this, this.MapiFolder, base.Mailbox, flag);
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

		internal override void Config(byte[] folderId, CoreFolder folder, StorageMailbox mailbox)
		{
			base.Config(folderId, folder, mailbox);
			this.MapiFolder.AllowWarnings = true;
		}

		internal void CopyBatch(IFxProxyPool proxyPool, List<MessageRec> batch)
		{
			StorageSourceFolder.<>c__DisplayClass9 CS$<>8__locals1 = new StorageSourceFolder.<>c__DisplayClass9();
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
								CS$<>8__locals1.<>4__this.MapiFolder.ExportMessages(fxProxyBudgetWrapper, CS$<>8__locals1.flags, smallBatch);
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
			return DisposeTracker.Get<StorageSourceFolder>(this);
		}

		private IContentChangesFetcher contentChangesFetcher;
	}
}
