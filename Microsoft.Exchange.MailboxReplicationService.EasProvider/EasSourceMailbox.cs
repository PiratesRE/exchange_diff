using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Connections.Eas.Commands.FolderSync;
using Microsoft.Exchange.Connections.Eas.Model.Extensions;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasSourceMailbox : EasMailbox, ISourceMailbox, IMailbox, IDisposable, ISupportMime, IReplayProvider
	{
		public EasSourceMailbox()
		{
		}

		internal EasSourceMailbox(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters) : base(connectionParameters, authenticationParameters, deviceParameters)
		{
		}

		internal override bool SupportsSavingSyncState
		{
			get
			{
				return true;
			}
		}

		Stream ISupportMime.GetMimeStream(MessageRec message, out PropValueData[] extraPropValues)
		{
			extraPropValues = null;
			Properties properties = this.FetchMessageItem(message);
			if (properties == null || properties.Body == null || string.IsNullOrEmpty(properties.Body.Data))
			{
				throw new UnableToFetchMimeStreamException(EasMailbox.GetStringId(message.EntryId));
			}
			if (properties.Flag != null)
			{
				extraPropValues = new PropValueData[]
				{
					new PropValueData((PropTag)277872643U, properties.Flag.Status)
				};
			}
			string data = properties.Body.Data;
			MemoryStream memoryStream = new MemoryStream(data.Length);
			Stream result;
			using (DisposeGuard disposeGuard = memoryStream.Guard())
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
				{
					streamWriter.Write(data);
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				disposeGuard.Success();
				result = memoryStream;
			}
			return result;
		}

		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			throw new NotImplementedException();
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			Add add;
			EasFolderBase easFolderBase;
			if (base.EasFolderCache.TryGetValue(entryId, out add))
			{
				easFolderBase = new EasSourceFolder(add, base.EasConnectionWrapper.UserSmtpAddress);
			}
			else if (EasMailbox.GetStringId(entryId) == EasSyntheticFolder.RootFolder.ServerId)
			{
				easFolderBase = EasSyntheticFolder.RootFolder;
			}
			else
			{
				if (!(EasMailbox.GetStringId(entryId) == EasSyntheticFolder.IpmSubtreeFolder.ServerId))
				{
					MrsTracer.Provider.Debug("Folder with folderId '{0}' does not exist.", new object[]
					{
						entryId
					});
					return null;
				}
				easFolderBase = EasSyntheticFolder.IpmSubtreeFolder;
			}
			return (ISourceFolder)easFolderBase.Configure(this);
		}

		void ISourceMailbox.CopyTo(IFxProxy destMailboxProxy, PropTag[] excludeTags)
		{
			throw new NotImplementedException();
		}

		void ISourceMailbox.SetMailboxSyncState(string syncStateString)
		{
			base.SetMailboxSyncState(syncStateString);
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			return base.GetMailboxSyncState();
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.EnumerateHierarchyChanges(flags:{0}, maxChanges:{1})", new object[]
			{
				flags,
				maxChanges
			});
			bool catchup = flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			if (catchup)
			{
				if (string.IsNullOrEmpty(this.SyncState.HierarchyData.ProviderSyncState) || this.SyncState.HierarchyData.ManualSyncData != null)
				{
					base.RefreshFolderCache();
				}
				return null;
			}
			return this.EnumerateHierarchyChanges(catchup, delegate(SyncHierarchyManifestState hierState)
			{
				MailboxChangesManifest result;
				try
				{
					result = this.DoManifestSync(flags, maxChanges, hierState, null);
				}
				catch (EasRequiresSyncKeyResetException ex)
				{
					MrsTracer.Provider.Error("Encountered RequiresSyncKeyReset error: {0}", new object[]
					{
						ex
					});
					result = this.RunManualHierarchySync(catchup, hierState);
				}
				return result;
			});
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.CopyMessagesOneByOne(messages, proxyPool, propsToCopyExplicitly, excludeProps, null);
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			throw new NotImplementedException();
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.ReplayActions({0} actions)", new object[]
			{
				actions.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			return this.Replay(actions);
		}

		void IReplayProvider.MarkAsRead(IReadOnlyCollection<MarkAsReadAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.MarkAsRead({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (MarkAsReadAction markAsReadAction in actions)
			{
				this.MarkMessageAsReadUnread(markAsReadAction.ItemId, markAsReadAction.FolderId, true);
			}
		}

		void IReplayProvider.MarkAsUnRead(IReadOnlyCollection<MarkAsUnReadAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.MarkAsUnRead({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (MarkAsUnReadAction markAsUnReadAction in actions)
			{
				this.MarkMessageAsReadUnread(markAsUnReadAction.ItemId, markAsUnReadAction.FolderId, false);
			}
		}

		IReadOnlyCollection<MoveActionResult> IReplayProvider.Move(IReadOnlyCollection<MoveAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.Move({0} actions)", new object[]
			{
				actions.Count
			});
			List<MoveActionResult> list = new List<MoveActionResult>(actions.Count);
			foreach (MoveAction moveAction in actions)
			{
				byte[] itemId = moveAction.ItemId;
				bool moveAsDelete;
				byte[] itemId2 = this.MoveItem(itemId, moveAction.PreviousFolderId, moveAction.FolderId, out moveAsDelete);
				list.Add(new MoveActionResult(itemId2, itemId, moveAsDelete));
			}
			return list;
		}

		void IReplayProvider.Send(SendAction action)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.Send({0})", new object[]
			{
				action
			});
			string mimeString = null;
			using (MemoryStream memoryStream = new MemoryStream(action.Data))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
				{
					mimeString = streamReader.ReadToEnd();
				}
			}
			string clientId = EasSourceMailbox.ClientIdFromItemId(action.ItemId);
			base.EasConnectionWrapper.SendMail(clientId, mimeString);
		}

		void IReplayProvider.Delete(IReadOnlyCollection<DeleteAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.Delete({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (MoveAction moveAction in actions)
			{
				this.DeleteItem(moveAction.ItemId, moveAction.PreviousFolderId);
			}
		}

		void IReplayProvider.Flag(IReadOnlyCollection<FlagAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.Flag({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (FlagAction flagAction in actions)
			{
				this.FlagMessage(flagAction.ItemId, flagAction.FolderId, FlagStatus.Flagged);
			}
		}

		void IReplayProvider.FlagClear(IReadOnlyCollection<FlagClearAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.FlagClear({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (FlagClearAction flagClearAction in actions)
			{
				this.FlagMessage(flagClearAction.ItemId, flagClearAction.FolderId, FlagStatus.NotFlagged);
			}
		}

		void IReplayProvider.FlagComplete(IReadOnlyCollection<FlagCompleteAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.FlagComplete({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (FlagCompleteAction flagCompleteAction in actions)
			{
				this.FlagMessage(flagCompleteAction.ItemId, flagCompleteAction.FolderId, FlagStatus.Complete);
			}
		}

		IReadOnlyCollection<CreateCalendarEventActionResult> IReplayProvider.CreateCalendarEvent(IReadOnlyCollection<CreateCalendarEventAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.CreateCalendarEvent({0} actions)", new object[]
			{
				actions.Count
			});
			List<CreateCalendarEventActionResult> list = new List<CreateCalendarEventActionResult>(actions.Count);
			foreach (CreateCalendarEventAction createCalendarEventAction in actions)
			{
				byte[] sourceItemId = this.CreateCalendarEvent(createCalendarEventAction.ItemId, createCalendarEventAction.FolderId, createCalendarEventAction.Event, createCalendarEventAction.ExceptionalEvents, createCalendarEventAction.DeletedOccurrences);
				list.Add(new CreateCalendarEventActionResult(sourceItemId));
			}
			return list;
		}

		void IReplayProvider.UpdateCalendarEvent(IReadOnlyCollection<UpdateCalendarEventAction> actions)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.UpdateCalendarEvent({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (UpdateCalendarEventAction updateCalendarEventAction in actions)
			{
				this.UpdateCalendarEvent(updateCalendarEventAction.ItemId, updateCalendarEventAction.FolderId, updateCalendarEventAction.Event, updateCalendarEventAction.ExceptionalEvents, updateCalendarEventAction.DeletedOccurrences);
			}
		}

		protected override void CopySingleMessage(MessageRec messageRec, IFolderProxy folderProxy, PropTag[] propTagsToExclude, PropTag[] excludeProps)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("EasSourceMailbox.CopySingleMessage", OperationType.None),
				new EntryIDsDataContext(messageRec.EntryId)
			}).Execute(delegate
			{
				Add add;
				if (this.EasFolderCache.TryGetValue(messageRec.FolderId, out add))
				{
					EasFolderType easFolderType = add.GetEasFolderType();
					if (EasFolder.IsCalendarFolder(easFolderType))
					{
						Properties calendarItemProperties = this.ReadCalendarItem(messageRec);
						EasSourceMailbox.CopyCalendarItem(messageRec, calendarItemProperties, folderProxy);
						return;
					}
					if (EasFolder.IsContactFolder(easFolderType))
					{
						EasSourceMailbox.CopyContactItem(messageRec, folderProxy);
						return;
					}
					SyncEmailUtils.CopyMimeStream(this, messageRec, folderProxy);
				}
			});
		}

		protected override MailboxChangesManifest DoManifestSync(EnumerateHierarchyChangesFlags flags, int maxChanges, SyncHierarchyManifestState hierState, MapiStore mapiStore)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.DoManifestSync", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			EasHierarchySyncState easHierarchySyncState = EasHierarchySyncState.Deserialize(hierState.ProviderSyncState);
			string syncKey = easHierarchySyncState.SyncKey;
			string syncKey2;
			IReadOnlyCollection<Add> readOnlyCollection;
			MailboxChangesManifest folderChangesOnServer = this.GetFolderChangesOnServer(syncKey, out syncKey2, out readOnlyCollection);
			bool flag = false;
			easHierarchySyncState.SyncKey = syncKey2;
			if (folderChangesOnServer.DeletedFolders != null)
			{
				List<Add> list = new List<Add>(folderChangesOnServer.DeletedFolders.Count);
				foreach (Add add in easHierarchySyncState.Folders)
				{
					foreach (byte[] entryId in folderChangesOnServer.DeletedFolders)
					{
						if (StringComparer.Ordinal.Equals(add.ServerId, EasMailbox.GetStringId(entryId)))
						{
							list.Add(add);
							break;
						}
					}
				}
				foreach (Add item in list)
				{
					easHierarchySyncState.Folders.Remove(item);
					flag = true;
				}
			}
			if (readOnlyCollection != null)
			{
				foreach (Add item2 in readOnlyCollection)
				{
					easHierarchySyncState.Folders.Add(item2);
					flag = true;
				}
			}
			hierState.ProviderSyncState = easHierarchySyncState.Serialize(false);
			if (flag)
			{
				base.RefreshFolderCache(easHierarchySyncState);
			}
			return folderChangesOnServer;
		}

		protected override MailboxChangesManifest RunManualHierarchySync(bool catchup, SyncHierarchyManifestState hierState)
		{
			MrsTracer.Provider.Function("EasSourceMailbox.RunManualHierarchySync", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			EasHierarchySyncState easHierarchySyncState = EasHierarchySyncState.Deserialize(hierState.ProviderSyncState);
			hierState.ProviderSyncState = null;
			EasHierarchySyncState easHierarchySyncState2 = base.RefreshFolderCache();
			EntryIdMap<EasHierarchySyncState.EasFolderData> entryIdMap = new EntryIdMap<EasHierarchySyncState.EasFolderData>();
			foreach (EasHierarchySyncState.EasFolderData easFolderData in easHierarchySyncState.FolderData)
			{
				entryIdMap[EasMailbox.GetEntryId(easFolderData.ServerId)] = easFolderData;
			}
			EntryIdMap<EasHierarchySyncState.EasFolderData> entryIdMap2 = new EntryIdMap<EasHierarchySyncState.EasFolderData>();
			foreach (EasHierarchySyncState.EasFolderData easFolderData2 in easHierarchySyncState2.FolderData)
			{
				entryIdMap2[EasMailbox.GetEntryId(easFolderData2.ServerId)] = easFolderData2;
			}
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			mailboxChangesManifest.DeletedFolders = new List<byte[]>();
			foreach (byte[] array in entryIdMap.Keys)
			{
				if (!entryIdMap2.ContainsKey(array))
				{
					mailboxChangesManifest.DeletedFolders.Add(array);
				}
			}
			mailboxChangesManifest.ChangedFolders = new List<byte[]>();
			foreach (KeyValuePair<byte[], EasHierarchySyncState.EasFolderData> keyValuePair in entryIdMap2)
			{
				byte[] key = keyValuePair.Key;
				EasHierarchySyncState.EasFolderData value = keyValuePair.Value;
				EasHierarchySyncState.EasFolderData easFolderData3;
				if (entryIdMap.TryGetValue(key, out easFolderData3))
				{
					if (easFolderData3.ParentId != value.ParentId || easFolderData3.DisplayName != value.DisplayName)
					{
						mailboxChangesManifest.ChangedFolders.Add(key);
					}
				}
				else
				{
					mailboxChangesManifest.ChangedFolders.Add(key);
				}
			}
			return mailboxChangesManifest;
		}

		private static string ClientIdFromItemId(byte[] itemId)
		{
			string text = TraceUtils.DumpBytes(itemId);
			if (text.Length > 40)
			{
				text = text.Substring(text.Length - 40);
			}
			return text;
		}

		private static void CopyContactItem(MessageRec messageRec, IFolderProxy folderProxy)
		{
			ArgumentValidator.ThrowIfNull("messageRec", messageRec);
			ArgumentValidator.ThrowIfNull("folderProxy", folderProxy);
			EasFxContactMessage message = new EasFxContactMessage(messageRec);
			FxUtils.CopyItem(messageRec, message, folderProxy, EasSourceMailbox.EmptyPropTagArray);
		}

		private static void CopyCalendarItem(MessageRec messageRec, Properties calendarItemProperties, IFolderProxy folderProxy)
		{
			ArgumentValidator.ThrowIfNull("messageRec", messageRec);
			ArgumentValidator.ThrowIfNull("folderProxy", folderProxy);
			if (calendarItemProperties == null)
			{
				return;
			}
			EasFxCalendarMessage message = new EasFxCalendarMessage(calendarItemProperties);
			FxUtils.CopyItem(messageRec, message, folderProxy, EasSourceMailbox.EmptyPropTagArray);
		}

		private Properties ReadCalendarItem(MessageRec messageRec)
		{
			Add add = base.EasFolderCache[messageRec.FolderId];
			string stringId = EasMailbox.GetStringId(messageRec.EntryId);
			return base.EasConnectionWrapper.FetchCalendarItem(stringId, add.ServerId);
		}

		private MailboxChangesManifest GetFolderChangesOnServer(string syncKey, out string newSyncKey, out IReadOnlyCollection<Add> newFolders)
		{
			newFolders = null;
			FolderSyncResponse folderSyncResponse = base.EasConnectionWrapper.FolderSync(syncKey);
			newSyncKey = folderSyncResponse.SyncKey;
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			mailboxChangesManifest.ChangedFolders = new List<byte[]>(0);
			mailboxChangesManifest.DeletedFolders = new List<byte[]>(0);
			if (folderSyncResponse.Changes != null)
			{
				List<Add> additions = folderSyncResponse.Changes.Additions;
				if (additions != null && additions.Count > 0)
				{
					mailboxChangesManifest.ChangedFolders.Capacity += additions.Count;
					foreach (Add add in additions)
					{
						mailboxChangesManifest.ChangedFolders.Add(EasMailbox.GetEntryId(add.ServerId));
					}
					newFolders = additions;
				}
				List<Update> updates = folderSyncResponse.Changes.Updates;
				if (updates != null && updates.Count > 0)
				{
					mailboxChangesManifest.ChangedFolders.Capacity += updates.Count;
					foreach (Update update in updates)
					{
						mailboxChangesManifest.ChangedFolders.Add(EasMailbox.GetEntryId(update.ServerId));
					}
				}
				List<Delete> deletions = folderSyncResponse.Changes.Deletions;
				if (deletions != null && deletions.Count > 0)
				{
					mailboxChangesManifest.DeletedFolders.Capacity = deletions.Count;
					foreach (Delete delete in deletions)
					{
						mailboxChangesManifest.DeletedFolders.Add(EasMailbox.GetEntryId(delete.ServerId));
					}
				}
			}
			return mailboxChangesManifest;
		}

		private Properties FetchMessageItem(MessageRec messageRec)
		{
			base.CheckDisposed();
			Add add = base.EasFolderCache[messageRec.FolderId];
			string stringId = EasMailbox.GetStringId(messageRec.EntryId);
			return base.EasConnectionWrapper.FetchMessageItem(stringId, add.ServerId);
		}

		private void MarkMessageAsReadUnread(byte[] messageEntryId, byte[] folderEntryId, bool isRead)
		{
			this.UpdateItem(messageEntryId, folderEntryId, delegate(string messageId, string syncKey, string serverId)
			{
				this.EasConnectionWrapper.SyncRead(messageId, syncKey, serverId, isRead);
			});
		}

		private byte[] MoveItem(byte[] messageEntryId, byte[] sourceFolderEntryId, byte[] destFolderEntryId, out bool isPermanentDeletionMove)
		{
			isPermanentDeletionMove = false;
			base.CheckDisposed();
			Add add;
			if (!base.EasFolderCache.TryGetValue(sourceFolderEntryId, out add))
			{
				MrsTracer.Provider.Warning("Source folder {0} doesn't exist", new object[]
				{
					TraceUtils.DumpBytes(sourceFolderEntryId)
				});
				throw new EasObjectNotFoundException(EasMailbox.GetStringId(sourceFolderEntryId));
			}
			Add add2;
			if (!base.EasFolderCache.TryGetValue(destFolderEntryId, out add2))
			{
				MrsTracer.Provider.Warning("Destination folder {0} doesn't exist", new object[]
				{
					TraceUtils.DumpBytes(destFolderEntryId)
				});
				throw new EasObjectNotFoundException(EasMailbox.GetStringId(destFolderEntryId));
			}
			string stringId = EasMailbox.GetStringId(messageEntryId);
			if (add2.Type == 4 && EasFolder.IsCalendarFolder((EasFolderType)add.Type))
			{
				this.DeleteItem(messageEntryId, sourceFolderEntryId);
				isPermanentDeletionMove = true;
				return null;
			}
			string stringId2 = base.EasConnectionWrapper.MoveItem(stringId, add.ServerId, add2.ServerId);
			return EasMailbox.GetEntryId(stringId2);
		}

		private void DeleteItem(byte[] messageEntryId, byte[] folderEntryId)
		{
			base.CheckDisposed();
			Add add;
			if (!base.EasFolderCache.TryGetValue(folderEntryId, out add))
			{
				MrsTracer.Provider.Warning("Source folder {0} doesn't exist", new object[]
				{
					TraceUtils.DumpBytes(folderEntryId)
				});
				throw new EasObjectNotFoundException(EasMailbox.GetStringId(folderEntryId));
			}
			string stringId = EasMailbox.GetStringId(messageEntryId);
			string syncKey = base.GetPersistedSyncState(folderEntryId).SyncKey;
			base.EasConnectionWrapper.DeleteItem(stringId, syncKey, add.ServerId);
		}

		private void FlagMessage(byte[] messageEntryId, byte[] folderEntryId, FlagStatus flagStatus)
		{
			this.UpdateItem(messageEntryId, folderEntryId, delegate(string messageId, string syncKey, string serverId)
			{
				this.EasConnectionWrapper.SyncFlag(messageId, syncKey, serverId, flagStatus);
			});
		}

		private void UpdateItem(byte[] itemEntryId, byte[] folderEntryId, Action<string, string, string> executeSync)
		{
			base.CheckDisposed();
			Add add;
			if (!base.EasFolderCache.TryGetValue(folderEntryId, out add))
			{
				MrsTracer.Provider.Warning("Source folder {0} doesn't exist", new object[]
				{
					TraceUtils.DumpBytes(folderEntryId)
				});
				throw new EasObjectNotFoundException(EasMailbox.GetStringId(folderEntryId));
			}
			string stringId = EasMailbox.GetStringId(itemEntryId);
			string syncKey = base.GetPersistedSyncState(folderEntryId).SyncKey;
			executeSync(stringId, syncKey, add.ServerId);
		}

		private byte[] CreateItem(byte[] itemEntryId, byte[] folderEntryId, Func<string, string, string, byte[]> executeSync)
		{
			base.CheckDisposed();
			Add add;
			if (!base.EasFolderCache.TryGetValue(folderEntryId, out add))
			{
				MrsTracer.Provider.Warning("Source folder {0} doesn't exist", new object[]
				{
					TraceUtils.DumpBytes(folderEntryId)
				});
				throw new EasObjectNotFoundException(EasMailbox.GetStringId(folderEntryId));
			}
			string arg = EasSourceMailbox.ClientIdFromItemId(itemEntryId);
			string syncKey = base.GetPersistedSyncState(folderEntryId).SyncKey;
			return executeSync(arg, syncKey, add.ServerId);
		}

		private void UpdateCalendarEvent(byte[] calendarEventId, byte[] folderEntryId, Event theEvent, IList<Event> exceptionalEvents, IList<string> deletedOccurrences)
		{
			this.UpdateItem(calendarEventId, folderEntryId, delegate(string messageId, string syncKey, string serverId)
			{
				this.EasConnectionWrapper.UpdateCalendarEvent(messageId, syncKey, serverId, theEvent, exceptionalEvents, deletedOccurrences, this.EasAuthenticationParameters.UserSmtpAddress);
			});
		}

		private byte[] CreateCalendarEvent(byte[] calendarEventId, byte[] folderEntryId, Event theEvent, IList<Event> exceptionalEvents, IList<string> deletedOccurrences)
		{
			SyncContentsManifestState syncContentsManifestState = this.SyncState[folderEntryId];
			EasFolderSyncState persistedSyncState = base.GetPersistedSyncState(syncContentsManifestState);
			string newSyncKey = null;
			byte[] result = this.CreateItem(calendarEventId, folderEntryId, (string itemClientId, string syncKey, string serverId) => this.EasConnectionWrapper.CreateCalendarEvent(itemClientId, syncKey, out newSyncKey, serverId, theEvent, exceptionalEvents, deletedOccurrences, this.EasAuthenticationParameters.UserSmtpAddress));
			if (newSyncKey != null)
			{
				persistedSyncState.SyncKey = newSyncKey;
				syncContentsManifestState.Data = persistedSyncState.Serialize();
			}
			return result;
		}

		ResourceHealthTracker ISupportMime.get_RHTracker()
		{
			return base.RHTracker;
		}

		private static readonly PropTag[] EmptyPropTagArray = new PropTag[0];
	}
}
