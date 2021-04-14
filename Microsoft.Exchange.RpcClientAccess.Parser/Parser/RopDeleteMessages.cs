using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopDeleteMessages : RopDeleteMessagesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.DeleteMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopDeleteMessages();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = DeleteMessagesResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			DeleteMessagesResultFactory resultFactory = new DeleteMessagesResultFactory(base.LogonIndex);
			this.result = ropHandler.DeleteMessages(serverObject, base.ReportProgress, base.IsOkToSendNonReadNotification, base.MessageIds, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new DeleteMessagesResultFactory(base.LogonIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.DeleteMessages;
	}
}
