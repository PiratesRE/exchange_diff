using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class MapiHttpRequest
	{
		protected MapiHttpRequest(ArraySegment<byte> auxiliaryBuffer)
		{
			this.auxiliaryBuffer = auxiliaryBuffer;
		}

		protected MapiHttpRequest(Reader reader)
		{
		}

		public ArraySegment<byte> AuxiliaryBuffer
		{
			get
			{
				return this.auxiliaryBuffer;
			}
		}

		protected void ParseAuxiliaryBuffer(Reader reader)
		{
			this.auxiliaryBuffer = reader.ReadSizeAndByteArraySegment(FieldLength.DWordSize);
		}

		protected void SerializeAuxiliaryBuffer(Writer writer)
		{
			writer.WriteSizedBytesSegment(this.auxiliaryBuffer, FieldLength.DWordSize);
		}

		public abstract void Serialize(Writer writer);

		private ArraySegment<byte> auxiliaryBuffer;
	}
}
