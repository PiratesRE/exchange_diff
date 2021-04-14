using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetAllPerUserLongTermIds : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetAllPerUserLongTermIds;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetAllPerUserLongTermIds();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreLongTermId startId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.startId = startId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.startId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetAllPerUserLongTermIdsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new GetAllPerUserLongTermIdsResultFactory(outputBuffer.Count);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.startId = StoreLongTermId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			GetAllPerUserLongTermIdsResultFactory resultFactory = new GetAllPerUserLongTermIdsResultFactory(outputBuffer.Count);
			this.result = ropHandler.GetAllPerUserLongTermIds(serverObject, this.startId, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" StartId=").Append(this.startId);
		}

		private const RopId RopType = RopId.GetAllPerUserLongTermIds;

		private StoreLongTermId startId;
	}
}
