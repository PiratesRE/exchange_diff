using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbExecuteRequest : MapiHttpRequest
	{
		public EmsmdbExecuteRequest(uint flags, ArraySegment<byte> ropBuffer, uint maxOutputBufferSize, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.ropBuffer = ropBuffer;
			this.maxOutputBufferSize = maxOutputBufferSize;
		}

		public EmsmdbExecuteRequest(Reader reader) : base(reader)
		{
			this.flags = reader.ReadUInt32();
			this.ropBuffer = reader.ReadSizeAndByteArraySegment(FieldLength.DWordSize);
			this.maxOutputBufferSize = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public ArraySegment<byte> RopBuffer
		{
			get
			{
				return this.ropBuffer;
			}
		}

		public uint MaxOutputBufferSize
		{
			get
			{
				return this.maxOutputBufferSize;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32(this.flags);
			writer.WriteSizedBytesSegment(this.ropBuffer, FieldLength.DWordSize);
			writer.WriteUInt32(this.maxOutputBufferSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint flags;

		private readonly ArraySegment<byte> ropBuffer;

		private readonly uint maxOutputBufferSize;
	}
}
