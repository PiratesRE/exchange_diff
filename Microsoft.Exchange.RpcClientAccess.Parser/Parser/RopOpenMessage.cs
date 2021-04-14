using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenMessage : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenMessage();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ushort codePageId, StoreId folderId, OpenMode openMode, StoreId messageId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.codePageId = codePageId;
			this.folderId = folderId;
			this.openMode = openMode;
			this.messageId = messageId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.codePageId);
			this.folderId.Serialize(writer);
			writer.WriteByte((byte)this.openMode);
			this.messageId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulOpenMessageResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new OpenMessageResultFactory(outputBuffer.Count);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.codePageId = reader.ReadUInt16();
			this.folderId = StoreId.Parse(reader);
			this.openMode = (OpenMode)reader.ReadByte();
			this.messageId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			OpenMessageResultFactory resultFactory = new OpenMessageResultFactory(outputBuffer.Count);
			this.result = ropHandler.OpenMessage(serverObject, this.codePageId, this.folderId, this.openMode, this.messageId, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" CPID=").Append(this.codePageId);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
			stringBuilder.Append(" OpenMode=").Append(this.openMode);
		}

		private const RopId RopType = RopId.OpenMessage;

		private ushort codePageId;

		private StoreId folderId;

		private OpenMode openMode;

		private StoreId messageId;
	}
}
