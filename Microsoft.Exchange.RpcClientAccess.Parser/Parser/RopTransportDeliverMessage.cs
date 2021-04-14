using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopTransportDeliverMessage : RopTransportDeliverMessageBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.TransportDeliverMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopTransportDeliverMessage();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopTransportDeliverMessage.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.TransportDeliverMessage(serverObject, this.recipientType, RopTransportDeliverMessage.resultFactory);
		}

		private const RopId RopType = RopId.TransportDeliverMessage;

		private static TransportDeliverMessageResultFactory resultFactory = new TransportDeliverMessageResultFactory();
	}
}
