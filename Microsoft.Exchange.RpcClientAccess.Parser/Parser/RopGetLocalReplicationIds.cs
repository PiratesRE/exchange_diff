using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetLocalReplicationIds : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetLocalReplicationIds;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetLocalReplicationIds();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, uint idCount)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.idCount = idCount;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt32(this.idCount);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetLocalReplicationIdsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetLocalReplicationIds.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.idCount = reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetLocalReplicationIds(serverObject, this.idCount, RopGetLocalReplicationIds.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Count=").Append(this.idCount);
		}

		private const RopId RopType = RopId.GetLocalReplicationIds;

		private static GetLocalReplicationIdsResultFactory resultFactory = new GetLocalReplicationIdsResultFactory();

		private uint idCount;
	}
}
