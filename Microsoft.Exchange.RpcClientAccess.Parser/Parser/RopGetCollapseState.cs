using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetCollapseState : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetCollapseState;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetCollapseState();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" RowId=").Append(this.rowId.ToString());
			stringBuilder.Append(" RowInstanceNumber=").Append(this.rowInstanceNumber);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId rowId, uint rowInstanceNumber)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.rowId = rowId;
			this.rowInstanceNumber = rowInstanceNumber;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.rowId.Serialize(writer);
			writer.WriteUInt32(this.rowInstanceNumber);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetCollapseStateResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetCollapseState.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.rowId = StoreId.Parse(reader);
			this.rowInstanceNumber = reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetCollapseState(serverObject, this.rowId, this.rowInstanceNumber, RopGetCollapseState.resultFactory);
		}

		private const RopId RopType = RopId.GetCollapseState;

		private static GetCollapseStateResultFactory resultFactory = new GetCollapseStateResultFactory();

		private StoreId rowId;

		private uint rowInstanceNumber;
	}
}
