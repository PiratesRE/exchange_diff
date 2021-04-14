using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolSetPropsMessage : DataMessageBase
	{
		public FxProxyPoolSetPropsMessage(PropValueData[] pvda)
		{
			this.pvda = pvda;
		}

		private FxProxyPoolSetPropsMessage(byte[] blob)
		{
			this.pvda = CommonUtils.DataContractDeserialize<PropValueData[]>(blob);
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolSetProps
				};
			}
		}

		public PropValueData[] PropValues
		{
			get
			{
				return this.pvda;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolSetPropsMessage(data);
		}

		protected override int GetSizeInternal()
		{
			int num = 0;
			foreach (PropValueData propValueData in this.pvda)
			{
				num += propValueData.GetApproximateSize();
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolSetProps;
			data = CommonUtils.DataContractSerialize<PropValueData[]>(this.pvda);
		}

		private PropValueData[] pvda;
	}
}
