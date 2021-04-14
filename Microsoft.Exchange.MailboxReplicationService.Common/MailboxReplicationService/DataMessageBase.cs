using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class DataMessageBase : IDataMessage
	{
		public DataMessageBase()
		{
		}

		int IDataMessage.GetSize()
		{
			return this.GetSizeInternal();
		}

		void IDataMessage.Serialize(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			this.SerializeInternal(useCompression, out opcode, out data);
		}

		protected virtual int GetSizeInternal()
		{
			return 0;
		}

		protected abstract void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data);
	}
}
