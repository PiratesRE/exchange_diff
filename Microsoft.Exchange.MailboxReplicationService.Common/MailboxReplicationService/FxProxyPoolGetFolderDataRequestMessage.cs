using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolGetFolderDataRequestMessage : DataMessageBase
	{
		private FxProxyPoolGetFolderDataRequestMessage()
		{
		}

		public static FxProxyPoolGetFolderDataRequestMessage Instance
		{
			get
			{
				return FxProxyPoolGetFolderDataRequestMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolGetFolderDataRequest
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FxProxyPoolGetFolderDataRequestMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolGetFolderDataRequest;
			data = null;
		}

		private static FxProxyPoolGetFolderDataRequestMessage instance = new FxProxyPoolGetFolderDataRequestMessage();
	}
}
