using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ImapSourceFolder : ImapFolder, ISourceFolder, IFolder, IDisposable
	{
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
			if (!base.Folder.IsSelectable)
			{
				return folderChangesManifest;
			}
			SyncContentsManifestState syncContentsManifestState = base.Mailbox.SyncState[base.FolderId];
			ImapFolderState imapFolderState;
			if (syncContentsManifestState.Data != null)
			{
				imapFolderState = ImapFolderState.Deserialize(syncContentsManifestState.Data);
				if (imapFolderState.UidValidity != base.Folder.UidValidity)
				{
					syncContentsManifestState.Data = ImapFolderState.CreateNew(base.Folder).Serialize();
					this.nextSeqNumCrawl = null;
					folderChangesManifest.FolderRecoverySync = true;
					return folderChangesManifest;
				}
			}
			else
			{
				imapFolderState = ImapFolderState.CreateNew(base.Folder);
			}
			List<MessageRec> messages = base.EnumerateMessages(FetchMessagesFlags.None, null, null);
			ImapFolderState imapFolderState2 = ImapFolderState.Create(messages, imapFolderState.SeqNumCrawl, base.Folder.UidNext, base.Folder.UidValidity);
			this.EnumerateIncrementalChanges(imapFolderState2, imapFolderState, folderChangesManifest, messages);
			syncContentsManifestState.Data = imapFolderState2.Serialize();
			return folderChangesManifest;
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			if (!base.Folder.IsSelectable)
			{
				return null;
			}
			SyncContentsManifestState syncContentsManifestState = base.Mailbox.SyncState[base.FolderId];
			ImapFolderState imapFolderState = (syncContentsManifestState.Data != null) ? ImapFolderState.Deserialize(syncContentsManifestState.Data) : ImapFolderState.CreateNew(base.Folder);
			if (this.nextSeqNumCrawl == null)
			{
				this.nextSeqNumCrawl = new int?((imapFolderState.SeqNumCrawl == int.MaxValue) ? (base.Folder.NumberOfMessages ?? 0) : imapFolderState.SeqNumCrawl);
			}
			else
			{
				imapFolderState.SeqNumCrawl = this.nextSeqNumCrawl.Value;
				syncContentsManifestState.Data = imapFolderState.Serialize();
			}
			if (this.nextSeqNumCrawl == 0)
			{
				return null;
			}
			int num = Math.Max(1, (this.nextSeqNumCrawl - maxPageSize + 1).Value);
			List<MessageRec> result = base.EnumerateMessages(FetchMessagesFlags.IncludeExtendedData, new int?(this.nextSeqNumCrawl.Value), new int?(num));
			this.nextSeqNumCrawl = new int?(Math.Max(0, num - 1));
			return result;
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			int? numberOfMessages = base.Folder.NumberOfMessages;
			if (numberOfMessages == null)
			{
				return 0;
			}
			return numberOfMessages.GetValueOrDefault();
		}

		private void EnumerateIncrementalChanges(ImapFolderState currentState, ImapFolderState lastSyncedState, FolderChangesManifest changes, IEnumerable<MessageRec> messages)
		{
			Dictionary<uint, MessageRec> dictionary = new Dictionary<uint, MessageRec>();
			foreach (MessageRec messageRec in messages)
			{
				uint key = ImapEntryId.ParseUid(messageRec.EntryId);
				dictionary.Add(key, messageRec);
			}
			this.EnumerateNewMessages(currentState, lastSyncedState, changes, dictionary);
			this.EnumerateReadUnreadFlagChanges(currentState, lastSyncedState, changes, dictionary);
			this.EnumerateMessageDeletes(currentState, lastSyncedState, changes, dictionary);
		}

		private void EnumerateNewMessages(ImapFolderState currentState, ImapFolderState lastSyncedState, FolderChangesManifest changes, Dictionary<uint, MessageRec> lookup)
		{
			if (lastSyncedState.UidNext == 0U || lastSyncedState.UidNext == 1U)
			{
				foreach (MessageRec messageRec in lookup.Values)
				{
					messageRec.Flags |= MsgRecFlags.New;
					changes.ChangedMessages.Add(messageRec);
				}
				return;
			}
			for (uint num = currentState.UidNext - 1U; num > lastSyncedState.UidNext - 1U; num -= 1U)
			{
				MessageRec messageRec2 = null;
				if (lookup.TryGetValue(num, out messageRec2))
				{
					messageRec2.Flags |= MsgRecFlags.New;
					changes.ChangedMessages.Add(messageRec2);
				}
			}
		}

		private void EnumerateMessageDeletes(ImapFolderState currentState, ImapFolderState lastSyncedState, FolderChangesManifest changes, Dictionary<uint, MessageRec> lookup)
		{
			Action<uint> uidInclusionAction = delegate(uint uid)
			{
				MessageRec item = new MessageRec(ImapEntryId.CreateMessageEntryId(uid, this.Folder.UidValidity, this.Folder.Name, this.Mailbox.ImapConnection.ConnectionContext.UserName), this.FolderId, CommonUtils.DefaultLastModificationTime, 0, MsgRecFlags.Deleted, Array<PropValueData>.Empty);
				changes.ChangedMessages.Add(item);
			};
			Action<uint> uidExclusionAction = delegate(uint uid)
			{
				MessageRec item = null;
				if (lookup.TryGetValue(uid, out item))
				{
					changes.ChangedMessages.Add(item);
				}
			};
			ImapFolderState.EnumerateMessageDeletes(currentState, lastSyncedState, uidInclusionAction, uidExclusionAction);
		}

		private void EnumerateReadUnreadFlagChanges(ImapFolderState currentState, ImapFolderState lastSyncedState, FolderChangesManifest changes, Dictionary<uint, MessageRec> lookup)
		{
			Action<uint> uidInclusionAction = delegate(uint uid)
			{
				MessageRec messageRec = null;
				if (lookup.TryGetValue(uid, out messageRec))
				{
					changes.ReadMessages.Add(messageRec.EntryId);
				}
			};
			Action<uint> uidExclusionAction = delegate(uint uid)
			{
				MessageRec messageRec = null;
				if (lookup.TryGetValue(uid, out messageRec))
				{
					changes.UnreadMessages.Add(messageRec.EntryId);
				}
			};
			ImapFolderState.EnumerateReadUnreadFlagChanges(currentState, lastSyncedState, uidInclusionAction, uidExclusionAction);
		}

		private int? nextSeqNumCrawl;
	}
}
