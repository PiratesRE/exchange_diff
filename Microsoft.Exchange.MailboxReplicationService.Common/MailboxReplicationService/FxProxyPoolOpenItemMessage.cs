using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolOpenItemMessage : MessageWithBuffer
	{
		public FxProxyPoolOpenItemMessage(byte[] entryID) : base(entryID)
		{
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolOpenItem
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolOpenItemMessage(data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolOpenItem;
			data = base.Buffer;
		}
	}
}
