using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopMoveCopyMessagesExtendedWithEntryIds : RopMoveCopyMessagesExtendedBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.MoveCopyMessagesExtendedWithEntryIds;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopMoveCopyMessagesExtendedWithEntryIds();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = MoveCopyMessagesExtendedWithEntryIdsResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory = new MoveCopyMessagesExtendedWithEntryIdsResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.MoveCopyMessagesExtendedWithEntryIds(sourceServerObject, destinationServerObject, base.MessageIds, base.ReportProgress, base.IsCopy, this.propertyValues, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new MoveCopyMessagesExtendedWithEntryIdsResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
		}

		private const RopId RopType = RopId.MoveCopyMessagesExtendedWithEntryIds;
	}
}
