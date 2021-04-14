using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct CopyMessagesCount
	{
		public CopyMessagesCount(int newMessages, int changed, int deleted, int read, int unread, int skipped)
		{
			this.NewMessages = newMessages;
			this.Changed = changed;
			this.Deleted = deleted;
			this.Read = read;
			this.Unread = unread;
			this.Skipped = skipped;
		}

		public int TotalContentCopied
		{
			get
			{
				return this.NewMessages + this.Changed + this.Deleted + this.Read + this.Unread;
			}
		}

		public static CopyMessagesCount operator +(CopyMessagesCount left, CopyMessagesCount right)
		{
			return new CopyMessagesCount(left.NewMessages + right.NewMessages, left.Changed + right.Changed, left.Deleted + right.Deleted, left.Read + right.Read, left.Unread + right.Unread, left.Skipped + right.Skipped);
		}

		public int NewMessages;

		public int Changed;

		public int Deleted;

		public int Read;

		public int Unread;

		public int Skipped;
	}
}
