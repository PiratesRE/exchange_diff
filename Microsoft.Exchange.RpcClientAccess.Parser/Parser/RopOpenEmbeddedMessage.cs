using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenEmbeddedMessage : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenEmbeddedMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenEmbeddedMessage();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ushort codePageId, OpenMode openMode)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.codePageId = codePageId;
			this.openMode = openMode;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.codePageId);
			writer.WriteByte((byte)this.openMode);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulOpenEmbeddedMessageResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new OpenEmbeddedMessageResultFactory(outputBuffer.Count);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.codePageId = reader.ReadUInt16();
			this.openMode = (OpenMode)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			OpenEmbeddedMessageResultFactory resultFactory = new OpenEmbeddedMessageResultFactory(outputBuffer.Count);
			this.result = ropHandler.OpenEmbeddedMessage(serverObject, this.codePageId, this.openMode, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" CPID=").Append(this.codePageId);
			stringBuilder.Append(" OpenMode=").Append(this.openMode);
		}

		private const RopId RopType = RopId.OpenEmbeddedMessage;

		private ushort codePageId;

		private OpenMode openMode;
	}
}
