using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolSaveChangesMessage : DataMessageBase
	{
		private FxProxyPoolSaveChangesMessage()
		{
		}

		public static FxProxyPoolSaveChangesMessage Instance
		{
			get
			{
				return FxProxyPoolSaveChangesMessage.instance;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolSaveChanges
				};
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return FxProxyPoolSaveChangesMessage.Instance;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolSaveChanges;
			data = null;
		}

		private static FxProxyPoolSaveChangesMessage instance = new FxProxyPoolSaveChangesMessage();
	}
}
