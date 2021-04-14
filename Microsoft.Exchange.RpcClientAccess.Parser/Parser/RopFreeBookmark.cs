using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFreeBookmark : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FreeBookmark;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFreeBookmark();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] bookmark)
		{
			Util.ThrowOnNullArgument(bookmark, "bookmark");
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.bookmark = bookmark;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.bookmark);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFreeBookmark.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.bookmark = reader.ReadSizeAndByteArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FreeBookmark(serverObject, this.bookmark, RopFreeBookmark.resultFactory);
		}

		private const RopId RopType = RopId.FreeBookmark;

		private static FreeBookmarkResultFactory resultFactory = new FreeBookmarkResultFactory();

		private byte[] bookmark;
	}
}
