using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class StorageSourceMailbox : StorageMailbox, ISourceMailbox, IMailbox, IDisposable
	{
		public StorageSourceMailbox(LocalMailboxFlags flags) : base(flags)
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
			MrsTracer.Provider.Function("StorageSourceMailbox.GetMailboxBasicInfo", new object[0]);
			base.CheckDisposed();
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
			MrsTracer.Provider.Function("StorageSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<StorageSourceFolder>(entryId);
		}

		void ISourceMailbox.CopyTo(IFxProxy fxProxy, PropTag[] excludeTags)
		{
			MrsTracer.Provider.Function("StorageSourceMailbox.CopyTo", new object[0]);
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
						base.StoreSession.Mailbox.MapiStore.ExportObject(fxProxyBudgetWrapper, copyPropertiesFlags, excludeTags);
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
			return this.EnumerateHierarchyChanges(catchup, (SyncHierarchyManifestState hierarchyData) => this.DoManifestSync(flags, maxChanges, hierarchyData, this.StoreSession.Mailbox.MapiStore));
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.Provider.Function("StorageSourceMailbox.ExportMessages({0} messages)", new object[]
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
			MrsTracer.ProxyClient.Function("StorageSourceMailbox.ExportFolders", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			bool exportCompleted = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				foreach (byte[] folderEntryId in folderIds)
				{
					this.ExportSingleFolder(proxyPool, folderEntryId, exportFoldersDataToCopyFlags, folderRecFlags, additionalFolderRecProps, copyPropertiesFlags, excludeProps, extendedAclFlags);
				}
				exportCompleted = true;
				proxyPool.Flush();
			}, delegate(Exception ex)
			{
				if (!exportCompleted)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(proxyPool.Flush), null);
				}
				return false;
			});
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			throw new NotImplementedException();
		}

		protected override StoreSession CreateStoreConnection(MailboxConnectFlags mailboxConnectFlags)
		{
			StoreSession storeSession = base.CreateStoreConnection(mailboxConnectFlags);
			if ((base.Flags.HasFlag(LocalMailboxFlags.Move) || base.Flags.HasFlag(LocalMailboxFlags.PublicFolderMove)) && !mailboxConnectFlags.HasFlag(MailboxConnectFlags.DoNotOpenMapiSession) && !mailboxConnectFlags.HasFlag(MailboxConnectFlags.NonMrsLogon) && !mailboxConnectFlags.HasFlag(MailboxConnectFlags.PublicFolderHierarchyReplication) && !mailboxConnectFlags.HasFlag(MailboxConnectFlags.ValidateOnly))
			{
				storeSession.IsMoveSource = true;
			}
			return storeSession;
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
				new OperationDataContext("StorageSourceMailbox.CopySingleMessage", OperationType.None),
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

		private static IFolderProxy GetFolderProxyForExportFolder(IFxProxyPool proxyPool, IFolder sourceFolder, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps)
		{
			IFolderProxy result;
			if ((exportFoldersDataToCopyFlags & ExportFoldersDataToCopyFlags.OutputCreateMessages) == ExportFoldersDataToCopyFlags.OutputCreateMessages)
			{
				FolderRec folderRec = sourceFolder.GetFolderRec(additionalFolderRecProps, folderRecFlags);
				result = proxyPool.CreateFolder(folderRec);
			}
			else
			{
				result = proxyPool.GetFolderProxy(sourceFolder.GetFolderId());
			}
			return result;
		}

		private void ExportExtendedAcls(AclFlags extendedAclFlags, ISourceFolder srcFolder, IFolderProxy targetFolder)
		{
			base.VerifyCapability(MRSProxyCapabilities.SetItemProperties, CapabilityCheck.BothMRSAndOtherProvider);
			if (extendedAclFlags.HasFlag(AclFlags.FolderAcl))
			{
				PropValueData[][] extendedAcl = srcFolder.GetExtendedAcl(AclFlags.FolderAcl);
				if (extendedAcl != null)
				{
					targetFolder.SetItemProperties(new FolderAcl(AclFlags.FolderAcl, extendedAcl));
				}
			}
			if (extendedAclFlags.HasFlag(AclFlags.FreeBusyAcl))
			{
				PropValueData[][] extendedAcl2 = srcFolder.GetExtendedAcl(AclFlags.FreeBusyAcl);
				if (extendedAcl2 != null)
				{
					targetFolder.SetItemProperties(new FolderAcl(AclFlags.FreeBusyAcl, extendedAcl2));
				}
			}
		}

		private void CopyMessageBatch(List<MessageRec> messages, IFxProxyPool proxyPool)
		{
			MrsTracer.Provider.Function("StorageSourceMailbox.CopyMessageBatch({0} messages)", new object[]
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
				CommonUtils.ProcessKnownExceptions(delegate
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
				}, delegate(Exception ex)
				{
					if (!exportCompleted)
					{
						MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
						CommonUtils.CatchKnownExceptions(new Action(proxyPool.Flush), null);
					}
					return false;
				});
			});
		}

		private void ExportSingleFolder(IFxProxyPool proxyPool, byte[] folderEntryId, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludePropertiesFromCopy, AclFlags extendedAclFlags)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("StorageSourceMailbox.ExportSingleFolder", OperationType.None),
				new EntryIDsDataContext(folderEntryId),
				new SimpleValueDataContext("exportFoldersDataToCopyFlags", exportFoldersDataToCopyFlags),
				new SimpleValueDataContext("folderRecFlags", folderRecFlags),
				new PropTagsDataContext(additionalFolderRecProps),
				new SimpleValueDataContext("copyPropertiesFlags", copyPropertiesFlags),
				new PropTagsDataContext(excludePropertiesFromCopy),
				new SimpleValueDataContext("extendedAclFlags", extendedAclFlags)
			}).Execute(delegate
			{
				using (this.RHTracker.Start())
				{
					using (ISourceFolder folder = this.GetFolder<StorageSourceFolder>(folderEntryId))
					{
						if (folder == null)
						{
							MrsTracer.Provider.Debug("Folder {0} is missing in source. Skipping.", new object[]
							{
								TraceUtils.DumpEntryId(folderEntryId)
							});
						}
						else
						{
							using (IFolderProxy folderProxyForExportFolder = StorageSourceMailbox.GetFolderProxyForExportFolder(proxyPool, folder, exportFoldersDataToCopyFlags, folderRecFlags, additionalFolderRecProps))
							{
								if (extendedAclFlags != AclFlags.None)
								{
									this.ExportExtendedAcls(extendedAclFlags, folder, folderProxyForExportFolder);
								}
								using (FxProxyBudgetWrapper fxProxyBudgetWrapper = new FxProxyBudgetWrapper(folderProxyForExportFolder, false, new Func<IDisposable>(this.RHTracker.StartExclusive), new Action<uint>(this.RHTracker.Charge)))
								{
									if (exportFoldersDataToCopyFlags.HasFlag(ExportFoldersDataToCopyFlags.IncludeCopyToStream))
									{
										folder.CopyTo(fxProxyBudgetWrapper, copyPropertiesFlags, excludePropertiesFromCopy);
									}
								}
							}
						}
					}
				}
			});
		}

		private void FlushBatchToFolder(List<MessageRec> batch, IFxProxyPool proxyPool)
		{
			if (batch.Count == 0)
			{
				return;
			}
			MrsTracer.Provider.Function("StorageSourceMailbox.FlushBatchToFolder({0} messages)", new object[]
			{
				batch.Count
			});
			byte[] folderId = batch[0].FolderId;
			using (StorageSourceFolder folder = base.GetFolder<StorageSourceFolder>(folderId))
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
	}
}
