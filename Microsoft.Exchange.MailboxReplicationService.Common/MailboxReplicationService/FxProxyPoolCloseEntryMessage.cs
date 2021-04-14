using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolCloseEntryMessage : DataMessageBase
	{
		private FxProxyPoolCloseEntryMessage()
		{
		}

		public static FxProxyPoolCloseEntryMessage Instance
		{
			get
			{
				return FxProxyPoolCloseEntryMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolCloseEntry
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FxProxyPoolCloseEntryMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolCloseEntry;
			data = null;
		}

		private static FxProxyPoolCloseEntryMessage instance = new FxProxyPoolCloseEntryMessage();
	}
}
