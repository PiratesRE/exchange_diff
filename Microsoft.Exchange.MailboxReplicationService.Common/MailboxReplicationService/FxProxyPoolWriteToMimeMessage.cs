using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolWriteToMimeMessage : MessageWithBuffer
	{
		public FxProxyPoolWriteToMimeMessage(byte[] buffer) : base(buffer)
		{
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolWriteToMime
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolWriteToMimeMessage(data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolWriteToMime;
			data = base.Buffer;
		}
	}
}
