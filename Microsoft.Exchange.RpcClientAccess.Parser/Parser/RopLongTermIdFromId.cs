using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopLongTermIdFromId : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.LongTermIdFromId;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopLongTermIdFromId();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId storeId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.storeId = storeId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.storeId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulLongTermIdFromIdResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopLongTermIdFromId.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.storeId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.LongTermIdFromId(serverObject, this.storeId, RopLongTermIdFromId.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ID=").Append(this.storeId.ToString());
		}

		private const RopId RopType = RopId.LongTermIdFromId;

		private static LongTermIdFromIdResultFactory resultFactory = new LongTermIdFromIdResultFactory();

		private StoreId storeId;
	}
}
