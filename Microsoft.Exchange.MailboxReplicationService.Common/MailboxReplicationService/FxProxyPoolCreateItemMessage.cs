using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolCreateItemMessage : DataMessageBase
	{
		private FxProxyPoolCreateItemMessage(bool createFAI)
		{
			this.createFAI = createFAI;
		}

		public static FxProxyPoolCreateItemMessage Regular
		{
			get
			{
				return FxProxyPoolCreateItemMessage.instanceRegular;
			}
		}

		public static FxProxyPoolCreateItemMessage FAI
		{
			get
			{
				return FxProxyPoolCreateItemMessage.instanceFAI;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolCreateFAIItem,
					DataMessageOpcode.FxProxyPoolCreateItem
				};
			}
		}

		public bool CreateFAI
		{
			get
			{
				return this.createFAI;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			if (opcode != DataMessageOpcode.FxProxyPoolCreateFAIItem)
			{
				return FxProxyPoolCreateItemMessage.Regular;
			}
			return FxProxyPoolCreateItemMessage.FAI;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = (this.createFAI ? DataMessageOpcode.FxProxyPoolCreateFAIItem : DataMessageOpcode.FxProxyPoolCreateItem);
			data = null;
		}

		private static FxProxyPoolCreateItemMessage instanceRegular = new FxProxyPoolCreateItemMessage(false);

		private static FxProxyPoolCreateItemMessage instanceFAI = new FxProxyPoolCreateItemMessage(true);

		private bool createFAI;
	}
}
