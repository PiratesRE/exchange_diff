using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PagedDataMessage : MessageWithBuffer
	{
		public PagedDataMessage(byte[] data, bool isLastChunk) : base(data)
		{
			this.isLastChunk = isLastChunk;
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.PagedDataChunk,
					DataMessageOpcode.PagedLastDataChunk
				};
			}
		}

		public bool IsLastChunk
		{
			get
			{
				return this.isLastChunk;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new PagedDataMessage(data, opcode == DataMessageOpcode.PagedLastDataChunk);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = (this.isLastChunk ? DataMessageOpcode.PagedLastDataChunk : DataMessageOpcode.PagedDataChunk);
			data = base.Buffer;
		}

		private bool isLastChunk;
	}
}
