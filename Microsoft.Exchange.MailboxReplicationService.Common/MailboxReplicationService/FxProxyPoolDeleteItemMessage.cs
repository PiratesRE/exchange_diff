using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolDeleteItemMessage : MessageWithBuffer
	{
		public FxProxyPoolDeleteItemMessage(byte[] entryID) : base(entryID)
		{
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolDeleteItem
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolDeleteItemMessage(data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolDeleteItem;
			data = base.Buffer;
		}
	}
}
