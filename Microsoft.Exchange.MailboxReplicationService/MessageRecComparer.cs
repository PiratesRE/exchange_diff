using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MessageRecComparer : IComparer<MessageRec>
	{
		private MessageRecComparer(MessageRecSortBy sortBy)
		{
			this.sortBy = sortBy;
		}

		public static IComparer<MessageRec> Comparer
		{
			get
			{
				return MessageRecComparer.normalInstance;
			}
		}

		public static IComparer<MessageRec> DescendingComparer
		{
			get
			{
				return MessageRecComparer.descendingInstance;
			}
		}

		public int Compare(MessageRec msg1, MessageRec msg2)
		{
			return msg1.CompareTo(this.sortBy, msg2.CreationTimestamp, msg2.FolderId, msg2.EntryId);
		}

		private static MessageRecComparer normalInstance = new MessageRecComparer(MessageRecSortBy.AscendingTimeStamp);

		private static MessageRecComparer descendingInstance = new MessageRecComparer(MessageRecSortBy.DescendingTimeStamp);

		private readonly MessageRecSortBy sortBy;
	}
}
