using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MessageExportResultsMessage : DataMessageBase
	{
		public MessageExportResultsMessage(List<MessageRec> missingMessages, List<BadMessageRec> badMessages)
		{
			this.MissingMessages = missingMessages;
			this.BadMessages = badMessages;
		}

		private MessageExportResultsMessage(bool useCompression, byte[] blob)
		{
			this.BadMessages = new List<BadMessageRec>();
			this.MissingMessages = new List<MessageRec>();
			string serializedXML = CommonUtils.UnpackString(blob, useCompression);
			MessageExportResults messageExportResults = XMLSerializableBase.Deserialize<MessageExportResults>(serializedXML, false);
			if (messageExportResults != null)
			{
				this.MissingMessages = messageExportResults.GetMissingMessages();
				this.BadMessages.AddRange(messageExportResults.BadMessages);
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.MessageExportResults
				};
			}
		}

		public List<MessageRec> MissingMessages { get; private set; }

		public List<BadMessageRec> BadMessages { get; private set; }

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new MessageExportResultsMessage(useCompression, data);
		}

		protected override int GetSizeInternal()
		{
			int num = 0;
			foreach (MessageRec messageRec in this.MissingMessages)
			{
				if (messageRec.EntryId != null && messageRec.FolderId != null)
				{
					num += messageRec.EntryId.Length + messageRec.FolderId.Length;
				}
			}
			foreach (BadMessageRec badMessageRec in this.BadMessages)
			{
				if (badMessageRec.EntryId != null && badMessageRec.XmlData != null)
				{
					num += badMessageRec.EntryId.Length + badMessageRec.XmlData.Length;
				}
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.MessageExportResults;
			MessageExportResults messageExportResults = new MessageExportResults(this.MissingMessages, this.BadMessages);
			string data2 = messageExportResults.Serialize(false);
			data = CommonUtils.PackString(data2, useCompression);
		}
	}
}
