using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetStatus : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetStatus;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetStatus();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetStatusResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetStatus.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetStatus(serverObject, RopGetStatus.resultFactory);
		}

		private const RopId RopType = RopId.GetStatus;

		private static GetStatusResultFactory resultFactory = new GetStatusResultFactory();
	}
}
