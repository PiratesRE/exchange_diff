using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopWriteCommitStream : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.WriteCommitStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopWriteCommitStream();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] data)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.data = data;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.data);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = WriteCommitStreamResult.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopWriteCommitStream.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.data = reader.ReadSizeAndByteArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			if ((long)outputBuffer.Count < RopWriteCommitStream.resultFactory.SuccessfulResultMinimalSize)
			{
				throw new BufferTooSmallException();
			}
			this.result = ropHandler.WriteCommitStream(serverObject, this.data, RopWriteCommitStream.resultFactory);
		}

		private const RopId RopType = RopId.WriteCommitStream;

		private static WriteCommitStreamResultFactory resultFactory = new WriteCommitStreamResultFactory();

		private byte[] data;
	}
}
