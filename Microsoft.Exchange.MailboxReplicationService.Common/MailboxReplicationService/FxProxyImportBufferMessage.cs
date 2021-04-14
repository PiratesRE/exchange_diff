using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyImportBufferMessage : MessageWithBuffer
	{
		public FxProxyImportBufferMessage(FxOpcodes opcode, byte[] data) : base(data)
		{
			this.opcode = opcode;
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.MapiFxConfig,
					DataMessageOpcode.MapiFxTransferBuffer,
					DataMessageOpcode.MapiFxIsInterfaceOk,
					DataMessageOpcode.MapiFxTellPartnerVersion,
					DataMessageOpcode.MapiFxStartMdbEventsImport,
					DataMessageOpcode.MapiFxFinishMdbEventsImport,
					DataMessageOpcode.MapiFxAddMdbEvents,
					DataMessageOpcode.MapiFxSetWatermarks,
					DataMessageOpcode.MapiFxSetReceiveFolder,
					DataMessageOpcode.MapiFxSetPerUser,
					DataMessageOpcode.MapiFxSetProps
				};
			}
		}

		public FxOpcodes Opcode
		{
			get
			{
				return this.opcode;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyImportBufferMessage((FxOpcodes)opcode, data);
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = (DataMessageOpcode)this.opcode;
			data = base.Buffer;
		}

		private FxOpcodes opcode;
	}
}
