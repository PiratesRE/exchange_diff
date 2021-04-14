using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopMoveCopyMessages : RopMoveCopyMessagesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.MoveCopyMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopMoveCopyMessages();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = MoveCopyMessagesResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			MoveCopyMessagesResultFactory resultFactory = new MoveCopyMessagesResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.MoveCopyMessages(sourceServerObject, destinationServerObject, base.MessageIds, base.ReportProgress, base.IsCopy, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new MoveCopyMessagesResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.MoveCopyMessages;
	}
}
