using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolOpenFolderMessage : MessageWithBuffer
	{
		public FxProxyPoolOpenFolderMessage(byte[] folderID) : base(folderID)
		{
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolOpenFolder
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolOpenFolderMessage(data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolOpenFolder;
			data = base.Buffer;
		}
	}
}
