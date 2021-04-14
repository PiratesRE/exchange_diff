using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class MapiHttpResponse
	{
		protected MapiHttpResponse(uint statusCode, ArraySegment<byte> auxiliaryBuffer)
		{
			this.statusCode = statusCode;
			this.auxiliaryBuffer = auxiliaryBuffer;
		}

		protected MapiHttpResponse(Reader reader)
		{
			this.statusCode = reader.ReadUInt32();
		}

		public uint StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public ArraySegment<byte> AuxiliaryBuffer
		{
			get
			{
				return this.auxiliaryBuffer;
			}
		}

		public virtual void Serialize(Writer writer)
		{
			writer.WriteUInt32(this.statusCode);
		}

		public long GetSerializedSize()
		{
			long position;
			using (CountWriter countWriter = new CountWriter())
			{
				this.Serialize(countWriter);
				position = countWriter.Position;
			}
			return position;
		}

		public virtual void AppendLogString(StringBuilder stringBuilder)
		{
			if (this.statusCode == 0U)
			{
				stringBuilder.Append(";SC:0");
				return;
			}
			stringBuilder.Append(";SC:");
			stringBuilder.Append(this.statusCode);
		}

		protected void ParseAuxiliaryBuffer(Reader reader)
		{
			this.auxiliaryBuffer = reader.ReadSizeAndByteArraySegment(FieldLength.DWordSize);
		}

		protected void SerializeAuxiliaryBuffer(Writer writer)
		{
			writer.WriteSizedBytesSegment(this.auxiliaryBuffer, FieldLength.DWordSize);
		}

		private readonly uint statusCode;

		private ArraySegment<byte> auxiliaryBuffer;
	}
}
