using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSeekRowBookmark : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SeekRowBookmark;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSeekRowBookmark();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] bookmark, int rowCount, bool wantMoveCount)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.bookmark = bookmark;
			this.rowCount = rowCount;
			this.wantMoveCount = wantMoveCount;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.bookmark);
			writer.WriteInt32(this.rowCount);
			writer.WriteBool(this.wantMoveCount, 1);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSeekRowBookmarkResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSeekRowBookmark.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.bookmark = reader.ReadSizeAndByteArray();
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
			this.result = ropHandler.SeekRowBookmark(serverObject, this.bookmark, this.rowCount, this.wantMoveCount, RopSeekRowBookmark.resultFactory);
		}

		private const RopId RopType = RopId.SeekRowBookmark;

		private static SeekRowBookmarkResultFactory resultFactory = new SeekRowBookmarkResultFactory();

		private byte[] bookmark;

		private int rowCount;

		private bool wantMoveCount;
	}
}
