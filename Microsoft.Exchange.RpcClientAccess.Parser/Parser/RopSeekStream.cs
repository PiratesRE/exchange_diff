using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSeekStream : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SeekStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSeekStream();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StreamSeekOrigin streamSeekOrigin, long offset)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.streamSeekOrigin = streamSeekOrigin;
			this.offset = offset;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.streamSeekOrigin);
			writer.WriteInt64(this.offset);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSeekStreamResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSeekStream.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.streamSeekOrigin = (StreamSeekOrigin)reader.ReadByte();
			this.offset = reader.ReadInt64();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SeekStream(serverObject, this.streamSeekOrigin, this.offset, RopSeekStream.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Origin=").Append(this.streamSeekOrigin);
			stringBuilder.Append(" Offset=0x").Append(this.offset.ToString("X16"));
		}

		private const RopId RopType = RopId.SeekStream;

		private static SeekStreamResultFactory resultFactory = new SeekStreamResultFactory();

		private StreamSeekOrigin streamSeekOrigin;

		private long offset;
	}
}
