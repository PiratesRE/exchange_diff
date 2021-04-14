using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCollapseRow : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CollapseRow;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCollapseRow();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId categoryId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.categoryId = categoryId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.categoryId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulCollapseRowResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopCollapseRow.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.categoryId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.CollapseRow(serverObject, this.categoryId, RopCollapseRow.resultFactory);
		}

		private const RopId RopType = RopId.CollapseRow;

		private static CollapseRowResultFactory resultFactory = new CollapseRowResultFactory();

		private StoreId categoryId;
	}
}
