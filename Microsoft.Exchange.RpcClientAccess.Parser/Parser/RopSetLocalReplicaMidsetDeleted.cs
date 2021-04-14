using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetLocalReplicaMidsetDeleted : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetLocalReplicaMidsetDeleted;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetLocalReplicaMidsetDeleted();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, LongTermIdRange[] longTermIdRanges)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.longTermIdRanges = longTermIdRanges;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedLongTermIdRanges(this.longTermIdRanges);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetLocalReplicaMidsetDeleted(serverObject, this.longTermIdRanges, RopSetLocalReplicaMidsetDeleted.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetLocalReplicaMidsetDeleted.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.longTermIdRanges = reader.ReadSizeAndLongTermIdRangeArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.SetLocalReplicaMidsetDeleted;

		private static SetLocalReplicaMidsetDeletedResultFactory resultFactory = new SetLocalReplicaMidsetDeletedResultFactory();

		private LongTermIdRange[] longTermIdRanges;
	}
}
