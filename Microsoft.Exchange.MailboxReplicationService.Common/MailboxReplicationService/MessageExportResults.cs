using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class MessageExportResults : XMLSerializableBase
	{
		public MessageExportResults()
		{
		}

		internal MessageExportResults(List<MessageRec> missingMessages, List<BadMessageRec> badMessages)
		{
			if (missingMessages != null)
			{
				this.MissingMessages = new MissingMessageRec[missingMessages.Count];
				for (int i = 0; i < missingMessages.Count; i++)
				{
					this.MissingMessages[i] = new MissingMessageRec(missingMessages[i]);
				}
			}
			else
			{
				this.MissingMessages = Array<MissingMessageRec>.Empty;
			}
			if (badMessages != null)
			{
				this.BadMessages = new BadMessageRec[badMessages.Count];
				badMessages.CopyTo(this.BadMessages, 0);
				return;
			}
			this.BadMessages = Array<BadMessageRec>.Empty;
		}

		[XmlArrayItem("MissingMessage")]
		[XmlArray("MissingMessages")]
		public MissingMessageRec[] MissingMessages { get; set; }

		[XmlArrayItem("BadMessage")]
		[XmlArray("BadMessages")]
		public BadMessageRec[] BadMessages { get; set; }

		internal List<MessageRec> GetMissingMessages()
		{
			List<MessageRec> list = new List<MessageRec>();
			foreach (MissingMessageRec missingMessageRec in this.MissingMessages)
			{
				list.Add(new MessageRec
				{
					EntryId = missingMessageRec.EntryId,
					FolderId = missingMessageRec.FolderId,
					Flags = (MsgRecFlags)missingMessageRec.Flags
				});
			}
			return list;
		}
	}
}
