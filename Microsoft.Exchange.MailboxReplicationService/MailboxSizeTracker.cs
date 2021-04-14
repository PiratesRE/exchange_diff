using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxSizeTracker
	{
		public MailboxSizeTracker()
		{
			this.totals = new MailboxSizeTracker.FolderSizeRecord(null, null, 0, 0UL);
			this.folderData = new EntryIdMap<MailboxSizeTracker.FolderSizeRecord>();
			this.IsFinishedEstimating = false;
		}

		public int MessageCount
		{
			get
			{
				return this.totals.MessageCount;
			}
		}

		public ulong TotalMessageSize
		{
			get
			{
				return this.totals.TotalMessageSize;
			}
		}

		public int DeletedMessageCount
		{
			get
			{
				return this.totals.DeletedMessageCount;
			}
		}

		public ulong TotalDeletedMessageSize
		{
			get
			{
				return this.totals.TotalDeletedMessageSize;
			}
		}

		public int AlreadyCopiedCount
		{
			get
			{
				return this.totals.AlreadyCopiedCount;
			}
		}

		public ulong AlreadyCopiedSize
		{
			get
			{
				return this.totals.AlreadyCopiedSize;
			}
		}

		public int TotalFolderCount { get; private set; }

		public int FoldersProcessed { get; private set; }

		public bool IsFinishedEstimating { get; set; }

		public void ResetFoldersProcessed(int totalFolders)
		{
			this.FoldersProcessed = 0;
			this.TotalFolderCount = totalFolders;
		}

		public void IncrementFoldersProcessed()
		{
			this.FoldersProcessed++;
		}

		public void TrackFolder(byte[] folderId, ICollection<MessageRec> folderMessages, int alreadyCopiedCount, ulong alreadyCopiedSize)
		{
			this.UpdateFolderData(new MailboxSizeTracker.FolderSizeRecord(folderId, folderMessages, alreadyCopiedCount, alreadyCopiedSize));
		}

		public void TrackFolder(byte[] folderId, int totalItemsCount, int alreadyCopiedCount, ulong alreadyCopiedSize)
		{
			this.UpdateFolderData(new MailboxSizeTracker.FolderSizeRecord(folderId, totalItemsCount, alreadyCopiedCount, alreadyCopiedSize));
		}

		public void TrackFolder(FolderRec fRec)
		{
			this.UpdateFolderData(new MailboxSizeTracker.FolderSizeRecord(fRec));
		}

		public void TrackFolder(FolderStateSnapshot folderStateSnaphot)
		{
			this.UpdateFolderData(new MailboxSizeTracker.FolderSizeRecord(folderStateSnaphot.FolderId, folderStateSnaphot.TotalMessages, folderStateSnaphot.TotalMessageByteSize, folderStateSnaphot.MessagesWritten, folderStateSnaphot.MessageByteSizeWritten, folderStateSnaphot.SoftDeletedMessageCount, folderStateSnaphot.TotalSoftDeletedMessageSize));
		}

		public void GetFolderSize(byte[] folderId, out int itemCount, out ulong totalItemSize)
		{
			MailboxSizeTracker.FolderSizeRecord folderSizeRecord;
			if (this.folderData.TryGetValue(folderId, out folderSizeRecord))
			{
				itemCount = folderSizeRecord.MessageCount;
				totalItemSize = folderSizeRecord.TotalMessageSize;
				return;
			}
			itemCount = 0;
			totalItemSize = 0UL;
		}

		private void UpdateFolderData(MailboxSizeTracker.FolderSizeRecord newRecord)
		{
			MailboxSizeTracker.FolderSizeRecord other;
			if (this.folderData.TryGetValue(newRecord.FolderId, out other))
			{
				this.totals.SubtractCounts(other);
			}
			this.totals.AddCounts(newRecord);
			this.folderData[newRecord.FolderId] = newRecord;
		}

		private MailboxSizeTracker.FolderSizeRecord totals;

		private EntryIdMap<MailboxSizeTracker.FolderSizeRecord> folderData;

		private class FolderSizeRecord
		{
			public FolderSizeRecord(byte[] folderId, int messageCount, ulong totalMessageSize, int alreadyCopiedCount, ulong alreadyCopiedSize, int deletedMessageCount, ulong totalDeletedMessageSize)
			{
				this.FolderId = folderId;
				this.MessageCount = messageCount;
				this.TotalMessageSize = totalMessageSize;
				this.AlreadyCopiedCount = alreadyCopiedCount;
				this.AlreadyCopiedSize = alreadyCopiedSize;
				this.DeletedMessageCount = deletedMessageCount;
				this.TotalDeletedMessageSize = totalDeletedMessageSize;
			}

			public FolderSizeRecord(byte[] folderId, ICollection<MessageRec> messages, int alreadyCopiedCount, ulong alreadyCopiedSize)
			{
				this.FolderId = folderId;
				this.MessageCount = alreadyCopiedCount;
				this.TotalMessageSize = alreadyCopiedSize;
				this.DeletedMessageCount = 0;
				this.TotalDeletedMessageSize = 0UL;
				this.AlreadyCopiedCount = alreadyCopiedCount;
				this.AlreadyCopiedSize = alreadyCopiedSize;
				if (messages != null)
				{
					this.TrackMessages(messages);
				}
			}

			public FolderSizeRecord(byte[] folderId, int totalItemsCount, int alreadyCopiedCount, ulong alreadyCopiedSize)
			{
				this.FolderId = folderId;
				this.MessageCount = totalItemsCount;
				this.TotalMessageSize = (ulong)(totalItemsCount * 100 * 1024);
				this.DeletedMessageCount = 0;
				this.TotalDeletedMessageSize = 0UL;
				this.AlreadyCopiedCount = alreadyCopiedCount;
				this.AlreadyCopiedSize = alreadyCopiedSize;
			}

			public FolderSizeRecord(FolderRec fRec)
			{
				this.FolderId = fRec.EntryId;
				this.MessageCount = 0;
				this.TotalMessageSize = 0UL;
				this.DeletedMessageCount = 0;
				this.TotalDeletedMessageSize = 0UL;
				object obj = fRec[PropTag.ContentCount];
				if (obj != null)
				{
					this.MessageCount += (int)obj;
				}
				object obj2 = fRec[PropTag.MessageSizeExtended];
				if (obj2 != null)
				{
					this.TotalMessageSize += (ulong)((long)obj2);
				}
				obj = fRec[PropTag.AssocContentCount];
				if (obj != null)
				{
					this.MessageCount += (int)obj;
				}
				obj2 = fRec[PropTag.AssocMessageSizeExtended];
				if (obj2 != null)
				{
					this.TotalMessageSize += (ulong)((long)obj2);
				}
			}

			public byte[] FolderId { get; private set; }

			public int MessageCount { get; private set; }

			public ulong TotalMessageSize { get; private set; }

			public int DeletedMessageCount { get; private set; }

			public ulong TotalDeletedMessageSize { get; private set; }

			public int AlreadyCopiedCount { get; private set; }

			public ulong AlreadyCopiedSize { get; private set; }

			public void AddCounts(MailboxSizeTracker.FolderSizeRecord other)
			{
				this.MessageCount += other.MessageCount;
				this.TotalMessageSize += other.TotalMessageSize;
				this.DeletedMessageCount += other.DeletedMessageCount;
				this.TotalDeletedMessageSize += other.TotalDeletedMessageSize;
				this.AlreadyCopiedCount += other.AlreadyCopiedCount;
				this.AlreadyCopiedSize += other.AlreadyCopiedSize;
			}

			public void SubtractCounts(MailboxSizeTracker.FolderSizeRecord other)
			{
				this.MessageCount -= other.MessageCount;
				this.TotalMessageSize -= other.TotalMessageSize;
				this.DeletedMessageCount -= other.DeletedMessageCount;
				this.TotalDeletedMessageSize -= other.TotalDeletedMessageSize;
				this.AlreadyCopiedCount -= other.AlreadyCopiedCount;
				this.AlreadyCopiedSize -= other.AlreadyCopiedSize;
			}

			private void TrackMessage(MessageRec msgRec)
			{
				if (msgRec.IsDeleted)
				{
					this.DeletedMessageCount++;
					this.TotalDeletedMessageSize += (ulong)((long)msgRec.MessageSize);
					return;
				}
				this.MessageCount++;
				this.TotalMessageSize += (ulong)((long)msgRec.MessageSize);
			}

			private void TrackMessages(ICollection<MessageRec> messages)
			{
				foreach (MessageRec msgRec in messages)
				{
					this.TrackMessage(msgRec);
				}
			}
		}
	}
}
