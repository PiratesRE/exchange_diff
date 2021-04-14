using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopWriteStream : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.WriteStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopWriteStream();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ArraySegment<byte> data)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.data = data;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytesSegment(this.data, FieldLength.WordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = WriteStreamResult.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopWriteStream.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.data = reader.ReadSizeAndByteArraySegment();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			if ((long)outputBuffer.Count < RopWriteStream.resultFactory.SuccessfulResultMinimalSize)
			{
				throw new BufferTooSmallException();
			}
			this.result = ropHandler.WriteStream(serverObject, this.data, RopWriteStream.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Size=").Append(this.data.Count);
		}

		private const RopId RopType = RopId.WriteStream;

		private static WriteStreamResultFactory resultFactory = new WriteStreamResultFactory();

		private ArraySegment<byte> data;
	}
}
