using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyGetObjectDataResponseMessage : MessageWithBuffer
	{
		public FxProxyGetObjectDataResponseMessage(byte[] data) : base(data)
		{
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyGetObjectDataResponse
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyGetObjectDataResponseMessage(data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyGetObjectDataResponse;
			data = base.Buffer;
		}
	}
}
