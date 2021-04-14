using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxUpdates
	{
		public MailboxUpdates()
		{
			this.folderData = new EntryIdMap<FolderUpdates>();
		}

		public EntryIdMap<FolderUpdates> FolderData
		{
			get
			{
				return this.folderData;
			}
			set
			{
				this.folderData = value;
			}
		}

		public FolderUpdates this[byte[] folderId]
		{
			get
			{
				FolderUpdates folderUpdates;
				if (!this.FolderData.TryGetValue(folderId, out folderUpdates))
				{
					folderUpdates = new FolderUpdates(folderId);
					this.folderData.Add(folderId, folderUpdates);
				}
				return folderUpdates;
			}
		}

		public void AddMessage(byte[] folderId, byte[] messageId, MessageUpdateType updateType)
		{
			FolderUpdates folderUpdates = this[folderId];
			folderUpdates.GetListForUpdateType(updateType, true).Add(messageId);
		}

		public void AddReadUnread(byte[] folderId, List<byte[]> readMessages, List<byte[]> unreadMessages)
		{
			FolderUpdates folderUpdates = this[folderId];
			folderUpdates.ReadMessages = readMessages;
			folderUpdates.UnreadMessages = unreadMessages;
		}

		public int GetUpdateCount(MessageUpdateType updateType)
		{
			int num = 0;
			foreach (FolderUpdates folderUpdates in this.folderData.Values)
			{
				num += folderUpdates.GetUpdateCount(updateType);
			}
			return num;
		}

		public bool IsEmpty()
		{
			List<byte[]> list = new List<byte[]>();
			foreach (FolderUpdates folderUpdates in this.folderData.Values)
			{
				if (folderUpdates.IsEmpty())
				{
					list.Add(folderUpdates.FolderId);
				}
			}
			foreach (byte[] key in list)
			{
				this.folderData.Remove(key);
			}
			return this.folderData.Count == 0;
		}

		private EntryIdMap<FolderUpdates> folderData;
	}
}
