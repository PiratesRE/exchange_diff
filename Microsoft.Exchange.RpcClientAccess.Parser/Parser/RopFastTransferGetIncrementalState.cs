using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferGetIncrementalState : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferGetIncrementalState;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferGetIncrementalState();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferGetIncrementalStateResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferGetIncrementalState.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferGetIncrementalState(serverObject, RopFastTransferGetIncrementalState.resultFactory);
		}

		private const RopId RopType = RopId.FastTransferGetIncrementalState;

		private static FastTransferGetIncrementalStateResultFactory resultFactory = new FastTransferGetIncrementalStateResultFactory();
	}
}
