using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RopGetPerUserLongTermIds : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetPerUserLongTermIds;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetPerUserLongTermIds();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, Guid databaseGuid)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.databaseGuid = databaseGuid;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteGuid(this.databaseGuid);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetPerUserLongTermIdsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetPerUserLongTermIds.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.databaseGuid = reader.ReadGuid();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetPerUserLongTermIds(serverObject, this.databaseGuid, RopGetPerUserLongTermIds.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" DatabaseGuid=").Append(this.databaseGuid);
		}

		private const RopId RopType = RopId.GetPerUserLongTermIds;

		private static readonly GetPerUserLongTermIdsResultFactory resultFactory = new GetPerUserLongTermIdsResultFactory();

		private Guid databaseGuid;
	}
}
