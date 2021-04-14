using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopHardDeleteMessages : RopDeleteMessagesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.HardDeleteMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopHardDeleteMessages();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = HardDeleteMessagesResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			HardDeleteMessagesResultFactory resultFactory = new HardDeleteMessagesResultFactory(base.LogonIndex);
			this.result = ropHandler.HardDeleteMessages(serverObject, base.ReportProgress, base.IsOkToSendNonReadNotification, base.MessageIds, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new HardDeleteMessagesResultFactory(base.LogonIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.HardDeleteMessages;
	}
}
