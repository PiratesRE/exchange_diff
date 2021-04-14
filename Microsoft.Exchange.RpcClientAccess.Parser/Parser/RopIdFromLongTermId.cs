using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopIdFromLongTermId : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.IdFromLongTermId;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopIdFromLongTermId();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreLongTermId longTermId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.longTermId = longTermId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.longTermId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulIdFromLongTermIdResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopIdFromLongTermId.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.longTermId = StoreLongTermId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.IdFromLongTermId(serverObject, this.longTermId, RopIdFromLongTermId.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
		}

		private const RopId RopType = RopId.IdFromLongTermId;

		private static IdFromLongTermIdResultFactory resultFactory = new IdFromLongTermIdResultFactory();

		private StoreLongTermId longTermId;
	}
}
