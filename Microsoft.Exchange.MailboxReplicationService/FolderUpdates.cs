using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderUpdates
	{
		public FolderUpdates()
		{
		}

		internal FolderUpdates(byte[] folderId)
		{
			this.folderId = folderId;
			this.deletedMessages = null;
			this.readMessages = null;
			this.unreadMessages = null;
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public List<byte[]> DeletedMessages
		{
			get
			{
				return this.deletedMessages;
			}
			set
			{
				this.deletedMessages = value;
			}
		}

		public List<byte[]> ReadMessages
		{
			get
			{
				return this.readMessages;
			}
			set
			{
				this.readMessages = value;
			}
		}

		public List<byte[]> UnreadMessages
		{
			get
			{
				return this.unreadMessages;
			}
			set
			{
				this.unreadMessages = value;
			}
		}

		public bool IsEmpty()
		{
			return (this.deletedMessages == null || this.deletedMessages.Count == 0) && (this.readMessages == null || this.readMessages.Count == 0) && (this.unreadMessages == null || this.unreadMessages.Count == 0);
		}

		public List<byte[]> GetListForUpdateType(MessageUpdateType updateType, bool createIfNeeded)
		{
			List<byte[]> result = null;
			switch (updateType)
			{
			case MessageUpdateType.Delete:
				result = this.GetOrCreateList(ref this.deletedMessages, createIfNeeded);
				break;
			case MessageUpdateType.SetRead:
				result = this.GetOrCreateList(ref this.readMessages, createIfNeeded);
				break;
			case MessageUpdateType.SetUnread:
				result = this.GetOrCreateList(ref this.unreadMessages, createIfNeeded);
				break;
			}
			return result;
		}

		internal int GetUpdateCount(MessageUpdateType updateType)
		{
			List<byte[]> listForUpdateType = this.GetListForUpdateType(updateType, false);
			if (listForUpdateType == null)
			{
				return 0;
			}
			return listForUpdateType.Count;
		}

		private List<byte[]> GetOrCreateList(ref List<byte[]> list, bool createIfNeeded)
		{
			if (list == null && createIfNeeded)
			{
				list = new List<byte[]>(1);
			}
			return list;
		}

		private byte[] folderId;

		private List<byte[]> deletedMessages;

		private List<byte[]> readMessages;

		private List<byte[]> unreadMessages;
	}
}
