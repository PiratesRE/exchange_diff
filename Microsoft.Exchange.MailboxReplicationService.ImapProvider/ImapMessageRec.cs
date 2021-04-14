using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Imap;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ImapMessageRec : IComparable<ImapMessageRec>
	{
		protected ImapMessageRec(string uidString, ImapMailFlags imapMailFlags)
		{
			uint uid;
			if (!uint.TryParse(uidString, out uid))
			{
				throw new InvalidUidException(uidString);
			}
			this.Uid = uid;
			this.ImapMailFlags = imapMailFlags;
		}

		public uint Uid { get; private set; }

		public ImapMailFlags ImapMailFlags { get; private set; }

		public static List<ImapMessageRec> FromImapResultData(ImapResultData resultData)
		{
			IList<string> messageUids = resultData.MessageUids;
			List<ImapMessageRec> list = new List<ImapMessageRec>(messageUids.Count);
			for (int i = 0; i < messageUids.Count; i++)
			{
				list.Add(new ImapMessageRec(resultData.MessageUids[i], resultData.MessageFlags[i]));
			}
			return list;
		}

		int IComparable<ImapMessageRec>.CompareTo(ImapMessageRec other)
		{
			return -this.Uid.CompareTo(other.Uid);
		}
	}
}
