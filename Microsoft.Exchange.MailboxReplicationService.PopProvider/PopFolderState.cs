using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class PopFolderState : XMLSerializableBase
	{
		public PopFolderState()
		{
			this.messageList = new PopBookmark(string.Empty);
		}

		[XmlElement(ElementName = "MessageListString")]
		public string MessageListString { get; set; }

		[XmlIgnore]
		internal PopBookmark MessageList
		{
			get
			{
				return this.messageList;
			}
			private set
			{
				this.messageList = value;
			}
		}

		public static PopFolderState Deserialize(byte[] compressedXml)
		{
			byte[] bytes = CommonUtils.DecompressData(compressedXml);
			string @string = Encoding.UTF7.GetString(bytes);
			if (string.IsNullOrEmpty(@string))
			{
				throw new CorruptSyncStateException(new ArgumentNullException("data", "Cannot deserialize null or empty data"));
			}
			PopFolderState popFolderState = XMLSerializableBase.Deserialize<PopFolderState>(@string, true);
			popFolderState.MessageList = PopBookmark.Parse(popFolderState.MessageListString);
			return popFolderState;
		}

		public byte[] Serialize()
		{
			this.MessageListString = this.MessageList.ToString();
			return CommonUtils.CompressData(Encoding.UTF7.GetBytes(base.Serialize(false)));
		}

		internal static PopFolderState CreateNew()
		{
			return new PopFolderState();
		}

		internal static PopFolderState Create(List<MessageRec> messages)
		{
			PopFolderState popFolderState = new PopFolderState();
			foreach (MessageRec messageRec in messages)
			{
				popFolderState.MessageList.Add(Encoding.UTF8.GetString(messageRec.EntryId));
			}
			return popFolderState;
		}

		private PopBookmark messageList;
	}
}
