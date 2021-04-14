using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MoveCopyMessagesExtendedResult : MoveCopyMessagesExtendedResultBase
	{
		internal MoveCopyMessagesExtendedResult(ErrorCode errorCode, bool isPartiallyCompleted, uint destinationObjectHandleIndex) : base(RopId.MoveCopyMessagesExtended, errorCode, isPartiallyCompleted, destinationObjectHandleIndex)
		{
		}

		internal MoveCopyMessagesExtendedResult(Reader reader) : base(reader)
		{
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
		}
	}
}
