using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FlushMessage : DataMessageBase
	{
		private FlushMessage()
		{
		}

		public static FlushMessage Instance
		{
			get
			{
				return FlushMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.Flush
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FlushMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.Flush;
			data = null;
		}

		private static FlushMessage instance = new FlushMessage();
	}
}
