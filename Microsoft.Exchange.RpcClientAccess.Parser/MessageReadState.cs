using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct MessageReadState
	{
		public byte[] MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public bool MarkAsRead
		{
			get
			{
				return this.markAsRead;
			}
		}

		public MessageReadState(byte[] messageId, bool markAsRead)
		{
			this.messageId = messageId;
			this.markAsRead = markAsRead;
		}

		internal static MessageReadState Parse(Reader reader)
		{
			return new MessageReadState(reader.ReadSizeAndByteArray(), reader.ReadBool());
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteSizedBytes(this.MessageId);
			writer.WriteBool(this.MarkAsRead);
		}

		public override string ToString()
		{
			string arg = this.markAsRead ? "Read" : "Unread";
			if (this.messageId.Length != 22)
			{
				return string.Format("{0}: Foreign({1}b)", arg, this.messageId.Length);
			}
			return string.Format("{0}: {1}", arg, StoreLongTermId.Parse(this.messageId, false));
		}

		internal static readonly uint MinimumSize = 3U;

		private readonly byte[] messageId;

		private readonly bool markAsRead;
	}
}
