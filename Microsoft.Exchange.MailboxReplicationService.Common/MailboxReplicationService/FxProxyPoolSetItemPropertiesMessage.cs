using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolSetItemPropertiesMessage : DataMessageBase
	{
		public FxProxyPoolSetItemPropertiesMessage(ItemPropertiesBase props)
		{
			this.Props = props;
		}

		private FxProxyPoolSetItemPropertiesMessage(byte[] blob)
		{
			this.Props = CommonUtils.DataContractDeserialize<ItemPropertiesBase>(blob);
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolSetItemProperties
				};
			}
		}

		public ItemPropertiesBase Props { get; private set; }

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolSetItemPropertiesMessage(data);
		}

		protected override int GetSizeInternal()
		{
			byte[] array = CommonUtils.DataContractSerialize<ItemPropertiesBase>(this.Props);
			return array.Length;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolSetItemProperties;
			data = CommonUtils.DataContractSerialize<ItemPropertiesBase>(this.Props);
		}
	}
}
