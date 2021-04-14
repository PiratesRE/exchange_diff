using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSeekRow : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SeekRow;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSeekRow();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.bookmarkOrigin = bookmarkOrigin;
			this.rowCount = rowCount;
			this.wantMoveCount = wantMoveCount;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.bookmarkOrigin);
			writer.WriteInt32(this.rowCount);
			writer.WriteBool(this.wantMoveCount, 1);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSeekRowResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSeekRow.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.bookmarkOrigin = (BookmarkOrigin)reader.ReadByte();
			this.rowCount = reader.ReadInt32();
			this.wantMoveCount = reader.ReadBool();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SeekRow(serverObject, this.bookmarkOrigin, this.rowCount, this.wantMoveCount, RopSeekRow.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Origin=").Append(this.bookmarkOrigin);
			stringBuilder.Append(" Count=").Append(this.rowCount);
			stringBuilder.Append(" WantCount=").Append(this.wantMoveCount);
		}

		private const RopId RopType = RopId.SeekRow;

		private static SeekRowResultFactory resultFactory = new SeekRowResultFactory();

		private BookmarkOrigin bookmarkOrigin;

		private int rowCount;

		private bool wantMoveCount;
	}
}
