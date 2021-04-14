using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PopSourceFolder : PopFolder, ISourceFolder, IFolder, IDisposable
	{
		public PopSourceFolder()
		{
			this.crawlerCopiedMessages = new PopBookmark(string.Empty);
		}

		void ISourceFolder.CopyTo(IFxProxy fxFolderProxy, CopyPropertiesFlags flags, PropTag[] propTagsToExclude)
		{
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			FolderChangesManifest folderChangesManifest = new FolderChangesManifest(base.FolderId);
			folderChangesManifest.ChangedMessages = new List<MessageRec>();
			folderChangesManifest.ReadMessages = new List<byte[]>();
			folderChangesManifest.UnreadMessages = new List<byte[]>();
			SyncContentsManifestState syncContentsManifestState = base.Mailbox.SyncState[base.FolderId];
			PopFolderState lastSyncedState;
			if (syncContentsManifestState.Data != null)
			{
				lastSyncedState = PopFolderState.Deserialize(syncContentsManifestState.Data);
			}
			else
			{
				lastSyncedState = PopFolderState.CreateNew();
			}
			List<MessageRec> messages = base.EnumerateMessagesOnPopConnection(maxChanges);
			PopFolderState popFolderState = PopFolderState.Create(messages);
			this.EnumerateIncrementalChanges(popFolderState, lastSyncedState, folderChangesManifest, messages);
			syncContentsManifestState.Data = popFolderState.Serialize();
			return folderChangesManifest;
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			SyncContentsManifestState syncContentsManifestState = base.Mailbox.SyncState[base.FolderId];
			PopFolderState popFolderState = (syncContentsManifestState.Data != null) ? PopFolderState.Deserialize(syncContentsManifestState.Data) : PopFolderState.CreateNew();
			foreach (string item in this.crawlerCopiedMessages.Values)
			{
				popFolderState.MessageList.Add(item);
				this.crawlerCopiedMessages.Remove(item);
			}
			syncContentsManifestState.Data = popFolderState.Serialize();
			List<MessageRec> list = base.EnumerateMessagesOnPopConnection(0);
			List<MessageRec> list2 = new List<MessageRec>();
			foreach (MessageRec messageRec in list)
			{
				string item2 = PopEntryId.ParseUid(messageRec.EntryId);
				if (!popFolderState.MessageList.Contains(item2))
				{
					list2.Add(messageRec);
					if (list2.Count == maxPageSize)
					{
						break;
					}
				}
			}
			if (list2.Count == 0)
			{
				return null;
			}
			foreach (MessageRec messageRec2 in list2)
			{
				string item3 = PopEntryId.ParseUid(messageRec2.EntryId);
				this.crawlerCopiedMessages.Add(item3);
			}
			return list2;
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			if (base.FolderId != PopMailbox.InboxEntryId)
			{
				return 0;
			}
			if (base.Mailbox.UniqueIdMap == null)
			{
				return 0;
			}
			return base.Mailbox.UniqueIdMap.Count;
		}

		private void EnumerateIncrementalChanges(PopFolderState currentState, PopFolderState lastSyncedState, FolderChangesManifest changes, IEnumerable<MessageRec> messages)
		{
			Dictionary<string, MessageRec> dictionary = new Dictionary<string, MessageRec>();
			foreach (MessageRec messageRec in messages)
			{
				string key = PopEntryId.ParseUid(messageRec.EntryId);
				dictionary.Add(key, messageRec);
			}
			this.EnumerateNewMessages(currentState, lastSyncedState, changes, dictionary);
			this.EnumerateMessageDeletes(currentState, lastSyncedState, changes, dictionary);
		}

		private void EnumerateNewMessages(PopFolderState currentState, PopFolderState lastSyncedState, FolderChangesManifest changes, Dictionary<string, MessageRec> lookup)
		{
			foreach (string text in currentState.MessageList.Values)
			{
				if (!lastSyncedState.MessageList.Contains(text))
				{
					MessageRec messageRec = null;
					if (lookup.TryGetValue(text, out messageRec))
					{
						messageRec.Flags |= MsgRecFlags.New;
						changes.ChangedMessages.Add(messageRec);
					}
				}
			}
		}

		private void EnumerateMessageDeletes(PopFolderState currentState, PopFolderState lastSyncedState, FolderChangesManifest changes, Dictionary<string, MessageRec> lookup)
		{
			foreach (string text in lastSyncedState.MessageList.Values)
			{
				if (!currentState.MessageList.Contains(text))
				{
					MessageRec messageRec = base.CreateMessageRec(text, 0L);
					messageRec.Flags |= MsgRecFlags.Deleted;
					changes.ChangedMessages.Add(messageRec);
				}
			}
		}

		private PopBookmark crawlerCopiedMessages;
	}
}
