using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class FolderStateSnapshot : XMLSerializableBase
	{
		[XmlElement(ElementName = "FolderId")]
		public byte[] FolderId { get; set; }

		[XmlElement(ElementName = "LocalCommitTimeMax")]
		public DateTime LocalCommitTimeMax { get; set; }

		[XmlElement(ElementName = "DeletedCountTotal")]
		public int DeletedCountTotal { get; set; }

		[XmlElement(ElementName = "CopyPropertiesTimestamp")]
		public DateTime CopyPropertiesTimestamp { get; set; }

		[XmlElement(ElementName = "State")]
		public FolderState State { get; set; }

		[XmlElement(ElementName = "LastSeedTimestamp")]
		public DateTime LastSeedTimestamp { get; set; }

		[XmlElement(ElementName = "LastSeedEntryId")]
		public byte[] LastSeedEntryId { get; set; }

		[XmlElement(ElementName = "TotalMessages")]
		public int TotalMessages { get; set; }

		[XmlElement(ElementName = "TotalMessageByteSize")]
		public ulong TotalMessageByteSize { get; set; }

		[XmlElement(ElementName = "MessagesWritten")]
		public int MessagesWritten { get; set; }

		[XmlElement(ElementName = "MessageByteSizeWritten")]
		public ulong MessageByteSizeWritten { get; set; }

		[XmlElement(ElementName = "SoftDeletedMessageCount")]
		public int SoftDeletedMessageCount { get; set; }

		[XmlElement(ElementName = "TotalSoftDeletedMessageSize")]
		public ulong TotalSoftDeletedMessageSize { get; set; }

		internal void UpdateMessageCopyWatermark(MessageRec msgRec)
		{
			this.LastSeedTimestamp = msgRec.CreationTimestamp;
			this.LastSeedEntryId = msgRec.EntryId;
		}

		internal List<MessageRec> GetMessagesToCopy(List<MessageRec> inputList, MessageRecSortBy sortBy, out int messagesWritten, out ulong totalMessageSizeWritten, out ulong totalMessageByteSize)
		{
			messagesWritten = 0;
			totalMessageSizeWritten = 0UL;
			totalMessageByteSize = 0UL;
			List<MessageRec> list = new List<MessageRec>(inputList.Count);
			foreach (MessageRec messageRec in inputList)
			{
				totalMessageByteSize += (ulong)((long)messageRec.MessageSize);
				if (!this.ShouldCopy(messageRec, sortBy))
				{
					messagesWritten++;
					totalMessageSizeWritten += (ulong)((long)messageRec.MessageSize);
				}
				else
				{
					list.Add(messageRec);
				}
			}
			return list;
		}

		internal ContentChangeResult VerifyContentsChanged(FolderRec folderRec)
		{
			if (this.State.HasFlag(FolderState.IsGhosted))
			{
				return ContentChangeResult.Ghosted;
			}
			if (!this.IsFolderChanged(folderRec, this.LocalCommitTimeMax))
			{
				return ContentChangeResult.NotChanged;
			}
			return ContentChangeResult.Changed;
		}

		internal bool IsFolderChanged(FolderRec folderRec)
		{
			return this.IsFolderChanged(folderRec, this.LocalCommitTimeMax);
		}

		internal bool IsFolderDataChanged(FolderRec folderRec)
		{
			return this.IsFolderChanged(folderRec, this.CopyPropertiesTimestamp);
		}

		internal void UpdateContentsCopied(FolderRec folderRec)
		{
			this.LocalCommitTimeMax = folderRec.LocalCommitTimeMax;
			this.DeletedCountTotal = folderRec.DeletedCountTotal;
		}

		internal void UpdateFolderDataCopied(FolderRec folderRec)
		{
			this.CopyPropertiesTimestamp = ((folderRec.FolderType == FolderType.Search) ? folderRec.LastModifyTimestamp : folderRec.LocalCommitTimeMax);
			this.State &= ~FolderState.PropertiesNotCopied;
		}

		private bool IsFolderChanged(FolderRec folderRec, DateTime timestamp)
		{
			bool flag = folderRec.FolderType == FolderType.Search;
			DateTime d = flag ? folderRec.LastModifyTimestamp : folderRec.LocalCommitTimeMax;
			if (d == DateTime.MinValue || d != timestamp)
			{
				return true;
			}
			if (!flag)
			{
				object obj = folderRec[PropTag.DeletedCountTotal];
				if (obj == null || (int)obj != this.DeletedCountTotal)
				{
					return true;
				}
			}
			return false;
		}

		private bool ShouldCopy(MessageRec msgRec, MessageRecSortBy sortBy)
		{
			return sortBy == MessageRecSortBy.SkipSort || 1 == msgRec.CompareTo(sortBy, this.LastSeedTimestamp, this.FolderId, this.LastSeedEntryId);
		}
	}
}
