using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopTransportDeliverMessage2 : RopTransportDeliverMessageBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.TransportDeliverMessage2;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopTransportDeliverMessage2();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulTransportDeliverMessage2Result.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopTransportDeliverMessage2.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.TransportDeliverMessage2(serverObject, this.recipientType, RopTransportDeliverMessage2.resultFactory);
		}

		private const RopId RopType = RopId.TransportDeliverMessage2;

		private static TransportDeliverMessage2ResultFactory resultFactory = new TransportDeliverMessage2ResultFactory();
	}
}
