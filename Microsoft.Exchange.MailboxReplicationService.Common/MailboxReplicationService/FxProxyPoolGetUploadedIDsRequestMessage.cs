using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolGetUploadedIDsRequestMessage : DataMessageBase
	{
		private FxProxyPoolGetUploadedIDsRequestMessage()
		{
		}

		public static FxProxyPoolGetUploadedIDsRequestMessage Instance
		{
			get
			{
				return FxProxyPoolGetUploadedIDsRequestMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolGetUploadedIDsRequest
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FxProxyPoolGetUploadedIDsRequestMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolGetUploadedIDsRequest;
			data = null;
		}

		private static FxProxyPoolGetUploadedIDsRequestMessage instance = new FxProxyPoolGetUploadedIDsRequestMessage();
	}
}
