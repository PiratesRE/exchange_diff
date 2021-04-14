using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyGetObjectDataRequestMessage : DataMessageBase
	{
		private FxProxyGetObjectDataRequestMessage()
		{
		}

		public static FxProxyGetObjectDataRequestMessage Instance
		{
			get
			{
				return FxProxyGetObjectDataRequestMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyGetObjectDataRequest
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FxProxyGetObjectDataRequestMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyGetObjectDataRequest;
			data = null;
		}

		private static FxProxyGetObjectDataRequestMessage instance = new FxProxyGetObjectDataRequestMessage();
	}
}
