using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbNotificationWaitResponse : MapiHttpOperationResponse
	{
		public EmsmdbNotificationWaitResponse(uint returnCode, uint flags, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.flags = flags;
		}

		public EmsmdbNotificationWaitResponse(Reader reader) : base(reader)
		{
			this.flags = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.flags);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint flags;
	}
}
