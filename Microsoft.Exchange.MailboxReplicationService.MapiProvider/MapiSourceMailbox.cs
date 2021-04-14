using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MapiSourceMailbox : MapiMailbox, ISourceMailbox, IMailbox, IDisposable
	{
		public MapiSourceMailbox(LocalMailboxFlags flags) : base(flags)
		{
		}

		internal MapiSourceMailbox(MapiStore mapiStore) : base(mapiStore)
		{
		}

		internal override bool SupportsSavingSyncState
		{
			get
			{
				return true;
			}
		}

		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			MrsTracer.Provider.Function("MapiSourceMailbox.GetMailboxBasicInfo", new object[0]);
			base.CheckDisposed();
			if (base.IsPublicFolderMigrationSource)
			{
				return MailboxSignatureConverter.CreatePublicFolderMailboxBasicInformation();
			}
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			uint mailboxSignatureServerVersion;
			byte[] array;
			using (ExRpcAdmin rpcAdmin = base.GetRpcAdmin())
			{
				using (base.RHTracker.Start())
				{
					MrsTracer.Provider.Debug("Reading mailbox basic info \"{0}\" in MDB {1}, flags {2}", new object[]
					{
						base.TraceMailboxId,
						base.MdbGuid,
						flags
					});
					mailboxSignatureServerVersion = rpcAdmin.GetMailboxSignatureServerVersion();
					array = rpcAdmin.GetMailboxBasicInfo(base.MdbGuid, base.MailboxGuid, flags);
				}
			}
			if (flags == MailboxSignatureFlags.GetLegacy && mailboxSignatureServerVersion >= 102U)
			{
				array = MailboxSignatureConverter.ExtractMailboxBasicInfo(array);
			}
			return array;
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("MapiSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<MapiSourceFolder>(entryId);
		}

		void ISourceMailbox.CopyTo(IFxProxy fxProxy, PropTag[] excludeTags)
		{
			MrsTracer.Provider.Function("MapiSourceMailbox.CopyTo", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			bool flag = false;
			try
			{
				CopyPropertiesFlags copyPropertiesFlags = CopyPropertiesFlags.None;
				if ((base.Flags & LocalMailboxFlags.StripLargeRulesForDownlevelTargets) != LocalMailboxFlags.None)
				{
					copyPropertiesFlags |= CopyPropertiesFlags.StripLargeRulesForDownlevelTargets;
				}
				using (base.RHTracker.Start())
				{
					using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(fxProxy, false, new Func<IDisposable>(base.RHTracker.StartExclusive), new Action<uint>(base.RHTracker.Charge)))
					{
						base.MapiStore.ExportObject(fxProxyBudgetWrapper, copyPropertiesFlags, excludeTags);
					}
				}
				flag = true;
				fxProxy.Flush();
			}
			finally
			{
				if (!flag)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(fxProxy.Flush), null);
				}
			}
		}

		void ISourceMailbox.SetMailboxSyncState(string syncStateStr)
		{
			base.SetMailboxSyncState(syncStateStr);
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			return base.GetMailboxSyncState();
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			bool catchup = flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			return this.EnumerateHierarchyChanges(catchup, delegate(SyncHierarchyManifestState hierarchyData)
			{
				bool flag = false;
				MailboxChangesManifest result;
				try
				{
					IL_02:
					result = this.DoManifestSync(flags, maxChanges, hierarchyData, this.MapiStore);
				}
				catch (MapiExceptionCallFailed mapiExceptionCallFailed)
				{
					if (mapiExceptionCallFailed.LowLevelError == 1228 && !flag)
					{
						this.ReadRulesFromAllFolders();
						flag = true;
						goto IL_02;
					}
					throw;
				}
				return result;
			});
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.Provider.Function("MapiSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			if ((flags & ExportMessagesFlags.OneByOne) != ExportMessagesFlags.None)
			{
				this.CopyMessagesOneByOne(messages, proxyPool, propsToCopyExplicitly, excludeProps, null);
				return;
			}
			this.CopyMessageBatch(messages, proxyPool);
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			throw new NotImplementedException();
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			throw new NotImplementedException();
		}

		public void ConfigPublicFolders(ADObjectId databaseId)
		{
			if (base.IsPureMAPI)
			{
				return;
			}
			base.ConfiguredMdbGuid = databaseId.ObjectGuid;
		}

		protected override Exception GetMailboxInTransitException(Exception innerException)
		{
			MrsTracer.Provider.Error("Source mailbox is being moved.", new object[0]);
			return new SourceMailboxAlreadyBeingMovedTransientException(innerException);
		}

		protected override OpenEntryFlags GetFolderOpenEntryFlags()
		{
			if (base.IsPublicFolderMove)
			{
				return OpenEntryFlags.Modify | OpenEntryFlags.DontThrowIfEntryIsMissing;
			}
			return OpenEntryFlags.DontThrowIfEntryIsMissing;
		}

		protected override void CopySingleMessage(MessageRec curMsg, IFolderProxy folderProxy, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("MapiSourceMailbox.CopySingleMessage", OperationType.None),
				new EntryIDsDataContext(curMsg.EntryId)
			}).Execute(delegate
			{
				using (this.RHTracker.Start())
				{
					using (MapiMessage mapiMessage = (MapiMessage)this.OpenMapiEntry(curMsg.FolderId, curMsg.EntryId, OpenEntryFlags.DontThrowIfEntryIsMissing))
					{
						if (mapiMessage == null)
						{
							MrsTracer.Provider.Debug("Message {0} is missing in source, ignoring", new object[]
							{
								TraceUtils.DumpEntryId(curMsg.EntryId)
							});
						}
						else
						{
							using (IMessageProxy messageProxy = folderProxy.OpenMessage(curMsg.EntryId))
							{
								using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(messageProxy, false, new Func<IDisposable>(this.RHTracker.StartExclusive), new Action<uint>(this.RHTracker.Charge)))
								{
									mapiMessage.ExportObject(fxProxyBudgetWrapper, CopyPropertiesFlags.None, excludeProps);
								}
								if (propsToCopyExplicitly != null && propsToCopyExplicitly.Length > 0)
								{
									PropValue[] props = mapiMessage.GetProps(propsToCopyExplicitly);
									using (this.RHTracker.StartExclusive())
									{
										List<PropValueData> list = new List<PropValueData>();
										foreach (PropValue propValue in props)
										{
											if (!propValue.IsNull() && !propValue.IsError())
											{
												list.Add(new PropValueData(propValue.PropTag, propValue.Value));
											}
										}
										if (list.Count > 0)
										{
											messageProxy.SetProps(list.ToArray());
										}
									}
								}
								using (this.RHTracker.StartExclusive())
								{
									messageProxy.SaveChanges();
								}
							}
						}
					}
				}
			});
		}

		private void CopyMessageBatch(List<MessageRec> messages, IFxProxyPool proxyPool)
		{
			MrsTracer.Provider.Function("MapiSourceMailbox.CopyMessageBatch({0} messages)", new object[]
			{
				messages.Count
			});
			byte[] curFolderId = null;
			List<MessageRec> curBatch = new List<MessageRec>();
			bool exportCompleted = false;
			List<byte[]> list = new List<byte[]>(messages.Count);
			foreach (MessageRec messageRec in messages)
			{
				list.Add(messageRec.EntryId);
			}
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("LocalSourceMailbox.CopyMessageBatch", OperationType.None),
				new EntryIDsDataContext(list)
			}).Execute(delegate
			{
				try
				{
					foreach (MessageRec messageRec2 in messages)
					{
						if (curFolderId != null && !CommonUtils.IsSameEntryId(curFolderId, messageRec2.FolderId))
						{
							this.FlushBatchToFolder(curBatch, proxyPool);
							curFolderId = null;
							curBatch.Clear();
						}
						curFolderId = messageRec2.FolderId;
						curBatch.Add(messageRec2);
					}
					this.FlushBatchToFolder(curBatch, proxyPool);
					exportCompleted = true;
					proxyPool.Flush();
				}
				catch (LocalizedException)
				{
					if (!exportCompleted)
					{
						MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
						CommonUtils.CatchKnownExceptions(new Action(proxyPool.Flush), null);
					}
					throw;
				}
			});
		}

		private void FlushBatchToFolder(List<MessageRec> batch, IFxProxyPool proxyPool)
		{
			if (batch.Count == 0)
			{
				return;
			}
			MrsTracer.Provider.Function("MapiSourceMailbox.FlushBatchToFolder({0} messages)", new object[]
			{
				batch.Count
			});
			byte[] folderId = batch[0].FolderId;
			using (MapiSourceFolder folder = base.GetFolder<MapiSourceFolder>(folderId))
			{
				if (folder == null)
				{
					MrsTracer.Provider.Debug("Folder {0} is missing in source. Will sync its deletion later.", new object[]
					{
						TraceUtils.DumpEntryId(folderId)
					});
				}
				else
				{
					folder.CopyBatch(proxyPool, batch);
				}
			}
		}

		private void ReadRulesFromAllFolders()
		{
			List<FolderRec> list = ((IMailbox)this).EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags.None, null);
			foreach (FolderRec folderRec in list)
			{
				if (folderRec.FolderType != FolderType.Search)
				{
					using (MapiSourceFolder folder = base.GetFolder<MapiSourceFolder>(folderRec.EntryId))
					{
						if (folder == null)
						{
							MrsTracer.Provider.Debug("Folder {0} is missing in source while reading rules. Will sync its deletion later.", new object[]
							{
								TraceUtils.DumpEntryId(folderRec.EntryId)
							});
							break;
						}
						using (base.RHTracker.Start())
						{
							folder.Folder.GetRules(null);
						}
					}
				}
			}
		}
	}
}
