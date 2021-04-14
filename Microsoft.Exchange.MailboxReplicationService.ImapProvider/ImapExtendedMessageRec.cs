using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Imap;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class ImapExtendedMessageRec : ImapMessageRec
	{
		private ImapExtendedMessageRec(ImapMailbox folder, string uidString, string messageId, ImapMailFlags imapMailFlags, long messageSize, int messageSeqNum) : base(uidString, imapMailFlags)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				messageId = this.ConstructMessageId(folder);
			}
			this.MessageId = messageId;
			this.MessageSize = messageSize;
			this.MessageSeqNum = messageSeqNum;
		}

		public string MessageId { get; private set; }

		public long MessageSize { get; private set; }

		public int MessageSeqNum { get; private set; }

		public static List<ImapMessageRec> FromImapResultData(ImapMailbox folder, ImapResultData resultData)
		{
			IList<string> messageUids = resultData.MessageUids;
			List<ImapMessageRec> list = new List<ImapMessageRec>(messageUids.Count);
			for (int i = 0; i < messageUids.Count; i++)
			{
				list.Add(new ImapExtendedMessageRec(folder, resultData.MessageUids[i], resultData.MessageIds[i], resultData.MessageFlags[i], resultData.MessageSizes[i], resultData.MessageSeqNums[i]));
			}
			return list;
		}

		private string ConstructMessageId(ImapMailbox folder)
		{
			if (folder.UidValidity == null)
			{
				throw new CannotCreateMessageIdException((long)((ulong)base.Uid), folder.Name);
			}
			string text = string.Format("{0}.{1}", folder.UidValidity.Value, base.Uid);
			MrsTracer.Provider.Debug(string.Format("MessageID missing, using <uidvalidity><uid> instead.  Folder = {0}.  UID = {1}.  MessageId = {2}.", folder.Name, base.Uid, text), new object[0]);
			return text;
		}
	}
}
