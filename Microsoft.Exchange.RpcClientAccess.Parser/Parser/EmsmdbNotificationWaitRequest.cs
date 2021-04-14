using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbNotificationWaitRequest : MapiHttpRequest
	{
		public EmsmdbNotificationWaitRequest(uint reserved, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.reserved = reserved;
		}

		public EmsmdbNotificationWaitRequest(Reader reader) : base(reader)
		{
			this.reserved = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32(this.reserved);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint reserved;
	}
}
