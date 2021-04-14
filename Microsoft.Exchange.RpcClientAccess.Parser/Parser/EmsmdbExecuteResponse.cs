using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbExecuteResponse : MapiHttpOperationResponse
	{
		public EmsmdbExecuteResponse(uint returnCode, uint reserved, ArraySegment<byte> ropBuffer, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.reserved = reserved;
			this.ropBuffer = ropBuffer;
		}

		public EmsmdbExecuteResponse(Reader reader) : base(reader)
		{
			this.reserved = reader.ReadUInt32();
			this.ropBuffer = reader.ReadSizeAndByteArraySegment(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public ArraySegment<byte> RopBuffer
		{
			get
			{
				return this.ropBuffer;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.reserved);
			writer.WriteSizedBytesSegment(this.ropBuffer, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint reserved;

		private readonly ArraySegment<byte> ropBuffer;
	}
}
