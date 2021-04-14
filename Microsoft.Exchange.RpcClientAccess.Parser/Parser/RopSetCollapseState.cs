using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetCollapseState : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetCollapseState;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetCollapseState();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] collapseState)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.collapseState = collapseState;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.collapseState);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSetCollapseStateResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetCollapseState.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.collapseState = reader.ReadSizeAndByteArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetCollapseState(serverObject, this.collapseState, RopSetCollapseState.resultFactory);
		}

		private const RopId RopType = RopId.SetCollapseState;

		private static SetCollapseStateResultFactory resultFactory = new SetCollapseStateResultFactory();

		private byte[] collapseState;
	}
}
