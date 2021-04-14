using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BufferBatchMessage : MessageWithBuffer
	{
		public BufferBatchMessage(byte[] data, bool flushAfterImport) : base(data)
		{
			this.flushAfterImport = flushAfterImport;
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.BufferBatch,
					DataMessageOpcode.BufferBatchWithFlush
				};
			}
		}

		public bool FlushAfterImport
		{
			get
			{
				return this.flushAfterImport;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new BufferBatchMessage(data, opcode == DataMessageOpcode.BufferBatchWithFlush);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = (this.flushAfterImport ? DataMessageOpcode.BufferBatchWithFlush : DataMessageOpcode.BufferBatch);
			data = base.Buffer;
		}

		private bool flushAfterImport;
	}
}
