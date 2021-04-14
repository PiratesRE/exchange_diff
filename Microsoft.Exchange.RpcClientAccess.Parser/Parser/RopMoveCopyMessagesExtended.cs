using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopMoveCopyMessagesExtended : RopMoveCopyMessagesExtendedBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.MoveCopyMessagesExtended;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopMoveCopyMessagesExtended();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = MoveCopyMessagesExtendedResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			MoveCopyMessagesExtendedResultFactory resultFactory = new MoveCopyMessagesExtendedResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.MoveCopyMessagesExtended(sourceServerObject, destinationServerObject, base.MessageIds, base.ReportProgress, base.IsCopy, this.propertyValues, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new MoveCopyMessagesExtendedResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
		}

		private const RopId RopType = RopId.MoveCopyMessagesExtended;
	}
}
